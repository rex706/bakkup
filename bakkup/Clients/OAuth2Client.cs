using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace bakkup.Clients
{
    /// <summary>
    /// Represents an OAuth2 client. This should be extended by classes to implement the OAuth2 login flow for
    /// different websites, such as Google Drive, OneDrive, DropBox, etc.
    /// </summary>
    public abstract class OAuth2Client
    {
        private readonly Form _parentWindow;

        #region Protected Properties

        /// <summary>
        /// The authorization endpoint url.
        /// </summary>
        protected abstract string AuthorizationEndpoint { get; }

        /// <summary>
        /// The token endpoint url.
        /// </summary>
        protected abstract string TokenEndpoint { get; }

        /// <summary>
        /// The client ID for the app registered with the web service API.
        /// </summary>
        protected abstract string ClientId { get; }

        /// <summary>
        /// The client secret for the app provided by the web service API.
        /// </summary>
        protected abstract string ClientSecret { get; }

        /// <summary>
        /// The name of the provider.
        /// </summary>
        protected abstract string ProviderName { get; }

        /// <summary>
        /// The url to redirect to after the user logins to the website and gives permission to this app.
        /// </summary>
        protected abstract string RedirectUri { get; }

        /// <summary>
        /// A list of url parameters AuthorizeForm should look for so it can close once the user has finished
        /// the login process.
        /// </summary>
        protected abstract List<string> AuthorizationFormCloseParams { get; }

        /// <summary>
        /// Gets the access token if one is available.
        /// </summary>
        protected string AccessToken { get; private set; }

        /// <summary>
        /// Gets the refresh token if one is available.
        /// </summary>
        protected string RefreshToken { get; private set; }

        /// <summary>
        /// Gets the time that the current access token expires.
        /// </summary>
        protected DateTime AccessTokenExpireTime { get; private set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Constructor for the OAuth2Client. Takes in the parent window so the AuthorizeForm window can be placed
        /// on top of the main window.
        /// </summary>
        /// <param name="parentWindow">The parent window the AuthorizeForm should be placed on top of.</param>
        protected OAuth2Client(Form parentWindow)
        {
            _parentWindow = parentWindow;
        }

        /// <summary>
        /// Using the provided usage scopes and parameters, shows a window asking user to grant the application access
        /// to their account. Returns the authorization code if the user grants access to this app.
        /// </summary>
        /// <param name="scopes">The scopes that the app has requested.</param>
        /// <param name="extraParams">Any extra parameters that are needed for authorization.</param>
        /// <returns></returns>
        protected string RequestAuthorization(List<string> scopes, NameValueCollection extraParams)
        {
            //Represents the collection that will contain all parameters of the request url. Start by adding the
            //required stuff to it.
            NameValueCollection parameters = new NameValueCollection()
            {
                {"response_type", "code"},
                {"client_id", ClientId},
                {"redirect_uri", RedirectUri}
            };
            //Add scopes if any are provided.
            if (scopes != null && scopes.Count > 0)
            {
                string scopesStr = "";
                //Add the scope parameter and then the list of scopes, separated by spaces.
                for (int i = 0; i < scopes.Count; i++)
                {
                    if (i != scopes.Count - 1)
                        scopesStr += scopes[i] + " ";
                    else
                        scopesStr += scopes[i];
                }

                parameters["scope"] = scopesStr;
            }
            //Add any extra parameters.
            if (extraParams != null && extraParams.Count > 0)
                parameters.Add(extraParams);          

            //Create the authorization request url.
            string requestUrl = ConstructUri(AuthorizationEndpoint, parameters).ToString();

            //Create the authorization window that will prompt the user to login to their account. The user will then
            //grant or deny access to this app.
            var loginWindow = new AuthorizeForm();
            loginWindow.Text = "Login To " + ProviderName;
            loginWindow.RequestAuthorization(requestUrl, AuthorizationFormCloseParams);
            //Show the window as a dialog to block application use until the user gives access to this app, denies
            //access to this app, or chooses to just close the window (also denying access to this app).
            loginWindow.ShowDialog(_parentWindow);

            //Return the authorization redirect url. Handling of parameters in this url is done by classes that
            //extend this class.
            return loginWindow.AuthorizationRedirectUrl;
        }

        /// <summary>
        /// Requests an access token using the collection of parameters to send to the server. This should include
        /// a "code" parameter that was retrieved in the RequestAuthorization method.
        /// </summary>
        /// <param name="parameters">The parameters the remote server requires to issue an access token.</param>
        /// <returns>A value indicating if the function succeeded or failed to obtain an access token.</returns>
        protected async Task<bool> RequestAccessToken(NameValueCollection parameters)
        {
            //Create the url parameter data using some LINQ.
            string postData = string.Join("&",
                parameters.AllKeys.Select(k => k + "=" + HttpUtility.UrlEncode(parameters[k])));
            
            //Convert the data into a byte array.
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);

            //Create the HTTP request for the access token information.
            var webRequest = (HttpWebRequest)WebRequest.Create(TokenEndpoint);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postBytes.Length;

            var requestStream = await webRequest.GetRequestStreamAsync();
            await requestStream.WriteAsync(postBytes, 0, postBytes.Length);
            await requestStream.FlushAsync();

            try
            {
                //Send the request and wait for a response.
                using (var response = (HttpWebResponse)await webRequest.GetResponseAsync())
                {
                    //Read the response. The response will be in JSON and contain the access token and other parameters.
                    string data = "";
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        data = await streamReader.ReadToEndAsync();
                    }

                    JObject jsonData = JObject.Parse(data);
                    /*
                     * There are 3 objects that must be returned here.
                     * access_token: The access token to use in all API calls.
                     * expires_in: The amount of time, in seconds, the application can use the access token until 
                     * it expires.
                     * refresh_token Used to get a new access token when the access token expires.
                     * 
                     * Store these values. If a value is missing, return false.
                     */
                    JToken value;
                    if (!jsonData.TryGetValue("access_token", out value))
                    {
                        //access_token missing.
                        LastError = "Invalid Access Token Response: Missing access_token variable.";
                        return false;
                    }
                    AccessToken = (string)value;
                    if (!jsonData.TryGetValue("refresh_token", out value))
                    {
                        //refresh_token missing.
                        LastError = "Invalid Access Token Response: Missing refresh_token variable.";
                        return false;
                    }
                    RefreshToken = (string)value;
                    if (!jsonData.TryGetValue("expires_in", out value))
                    {
                        //expires_in missing.
                        LastError = "Invalid Access Token Response: Missing expires_in variable.";
                        return false;
                    }
                    AccessTokenExpireTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, (int)value));
                }
            }
            catch (WebException ex)
            {
                LastError = "Failed to get an access token with the following error:\n" +
                            ex.GetType().Name + ": " + ex.Message;


                //Read the exception WebResponse response stream.
                using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    string data = "";
                    data = await streamReader.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(data))
                        LastError += "\nResponse Body: " + data;
                }

                Console.WriteLine(LastError);
                return false;
            }

            //Return true because all access token values were found.
            return true;
        }

        /// <summary>
        /// Requests a new access token using a refresh token. A collection of parameters are used and should
        /// include a "refresh_token" parameter that is stored by the application for getting new access tokens.
        /// </summary>
        /// <param name="parameters">The parameters the remote server requires to issue a new access token.</param>
        /// <returns>A value indicating whether or not a new access token was obtained.</returns>
        protected async Task<bool> RequestAccessTokenUsingRefreshToken(NameValueCollection parameters)
        {
            //Create the url parameter data using some LINQ.
            string postData = string.Join("&",
                parameters.AllKeys.Select(k => k + "=" + HttpUtility.UrlEncode(parameters[k])));
            //Convert that post data into a UTF8 string.
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);

            //Create an HTTP Request.
            var webRequest = (HttpWebRequest)WebRequest.CreateHttp(TokenEndpoint);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postBytes.Length;

            //Write in the HTTP request data.
            var requestStream = await webRequest.GetRequestStreamAsync();
            await requestStream.WriteAsync(postBytes, 0, postBytes.Length);
            await requestStream.FlushAsync();

            //Send the request and wait for the response.
            try
            {
                using (var response = (HttpWebResponse)await webRequest.GetResponseAsync())
                {
                    //Read the response. The response will be in JSON and contain the access token and other parameters.
                    string data = "";
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        data = await streamReader.ReadToEndAsync();
                    }

                    JObject jsonData = JObject.Parse(data);
                    /*
                     * There are 2 objects that must be returned here.
                     * access_token: The access token to use in all API calls.
                     * expires_in: The amount of time, in seconds, the application can use the access token until 
                     * it expires.
                     * 
                     * Store these values. If a value is missing, return false.
                     */
                    JToken value;
                    if (!jsonData.TryGetValue("access_token", out value))
                    {
                        //access_token missing.
                        LastError = "Invalid Access Token Response: Missing access_token variable.";
                        return false;
                    }
                    AccessToken = (string)value;
                    if (!jsonData.TryGetValue("expires_in", out value))
                    {
                        //expires_in missing.
                        LastError = "Invalid Access Token Response: Missing expires_in variable.";
                        return false;
                    }
                    AccessTokenExpireTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, (int)value));
                }
            }
            catch (WebException ex)
            {
                LastError = "Failed to get an access token using refresh token with the following error:\n" +
                            ex.GetType().Name + ": " + ex.Message;

                //Read the exception WebResponse response stream.
                using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    string data = "";
                    data = await streamReader.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(data))
                        LastError += "\nResponse Body: " + data;
                }

                Console.WriteLine(LastError);
            }

            return true;
        }

        /// <summary>
        /// Generates a cryptographically strong random string.
        /// </summary>
        /// <returns>A random string.</returns>
        protected static string GenerateRandomString()
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[64];
                rng.GetBytes(data);

                return Convert.ToBase64String(data);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the error message of the last error that occurred in the client.
        /// </summary>
        public string LastError { get; protected set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs the login process for this OAuth2Client instance.
        /// </summary>
        /// <returns>A value indicating whether or not the login process was successful.</returns>
        public abstract Task<bool> PerformLogin();

        #endregion

        #region Private Methods

        private Uri ConstructUri(string baseUri, NameValueCollection parameters)
        {
            //Construct the parameter part of the uri making some good use of LINQ.
            var keyValuePairs =
                parameters.AllKeys.Select(k => HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(parameters[k]));
            var queryPart = string.Join("&", keyValuePairs);

            return new UriBuilder(baseUri) {Query = queryPart}.Uri;
        }

        #endregion
    }
}
