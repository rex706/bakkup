using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using bakkup.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace bakkup.Clients
{
    /// <summary>
    /// Represents different kinds of errors that can occur in the OAuth2Client.
    /// </summary>
    public enum OAuthClientResult
    {
        /// <summary>
        /// The operation was successful.
        /// </summary>
        Success = 0,
        /// <summary>
        /// An error related to authorization occurred.
        /// </summary>
        Unauthorized = 1,
        /// <summary>
        /// There was some HTTP error.
        /// </summary>
        HttpError = 2,
        /// <summary>
        /// The user cancelled the operation.
        /// </summary>
        UserCancelled = 3,
        /// <summary>
        /// An error has occurred that is not explicitly handled in code.
        /// </summary>
        UnhandledError = 4,
        /// <summary>
        /// An unexpected error has occurred. This usually means the server being contacted did something
        /// strange.
        /// </summary>
        UnexpectedError = 5,
        /// <summary>
        /// An error occurred while parsing JSON. This is a sign that the API may have changed and the application
        /// should be updated.
        /// </summary>
        Parser = 6
    }

    /// <summary>
    /// Represents an OAuth2 web client. This should be extended by classes to implement the OAuth2 login flow for
    /// different websites, such as Google Drive, OneDrive, DropBox, etc.
    /// </summary>
    public abstract class OAuth2Client
    {
        private readonly ProgressMessageHandler _clientProgress;

        #region Protected Abstract Properties

        /// <summary>
        /// The authorization endpoint url.
        /// </summary>
        protected abstract string AuthorizationEndpoint { get; }

        /// <summary>
        /// The token endpoint url.
        /// </summary>
        protected abstract string TokenEndpoint { get; }

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

        #region Protected Properties

        /// <summary>
        /// Gets an instance of the WebClient that is being used to send and receive requests.
        /// </summary>
        protected HttpClient WebClient { get; }

        /// <summary>
        /// The client ID for the app registered with the web service API. This is loaded from the ClientData.json
        /// file.
        /// </summary>
        protected string ClientId { get; private set; }

        /// <summary>
        /// The client secret for the app provided by the web service API. This is loaded from the ClientData.json
        /// file.
        /// </summary>
        protected string ClientSecret { get; private set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Protecetd default constructor.
        /// </summary>
        protected OAuth2Client()
        {
            _clientProgress = new ProgressMessageHandler(new HttpClientHandler());
            WebClient = new HttpClient(_clientProgress);
            IsLoggedIn = false;
        }

        /// <summary>
        /// Using the provided usage scopes and parameters, shows a window asking user to grant the application access
        /// to their account. Returns the authorization code if the user grants access to this app.
        /// </summary>
        /// <param name="scopes">The scopes that the app has requested.</param>
        /// <param name="extraParams">Any extra parameters that are needed for authorization.</param>
        /// <returns>The authorization code for getting the initial access token, or null on cancel or error.</returns>
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
            var requestUrl = ConstructUri(AuthorizationEndpoint, parameters).ToString();

            //Create the authorization window that will prompt the user to login to their account. The user will then
            //grant or deny access to this app.
            var loginWindow = new AuthorizeForm();
            loginWindow.Text = "Login To " + ProviderName;
            loginWindow.RequestAuthorization(requestUrl, AuthorizationFormCloseParams);
            //Show the window as a dialog to block application use until the user gives access to this app, denies
            //access to this app, or chooses to just close the window (also denying access to this app).
            loginWindow.ShowDialog();

            //Check if authorization was cancelled by closing the window early.
            if (loginWindow.WasAuthorizationCancelled)
            {
                LastError = OAuthClientResult.UserCancelled;
                LastErrorMessage = "Authorization process cancelled by closing login window.";
                return null;
            }
            //The redirect url should not be empty.
            if (string.IsNullOrEmpty(loginWindow.AuthorizationRedirectUrl))
            {
                LastError = OAuthClientResult.UnexpectedError;
                LastErrorMessage = "Authorization url is null or empty.";
                return null;
            }

            //Success.
            LastError = OAuthClientResult.Success;
            LastErrorMessage = "Operation completed successfully.";

            //Return the authorization code. Subclasses will handle this value, including if it is null or empty.
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
            HttpRequestMessage accessTokenRequest = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint);
            HttpContent postContent = new ByteArrayContent(postBytes);
            postContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            postContent.Headers.ContentLength = postBytes.Length;
            accessTokenRequest.Content = postContent;

            var data = await SendRequestMessage(accessTokenRequest);

            //Make sure invalid data was not returned.
            if (string.IsNullOrEmpty(data))
            {
                LastErrorMessage = "Server did not return any data.";
                LastError = OAuthClientResult.UnhandledError;
                return false;
            }

            /*
             * There are 3 objects that must be returned here.
             * access_token: The access token to use in all API calls.
             * expires_in: The amount of time, in seconds, the application can use the access token until 
             * it expires.
             * refresh_token Used to get a new access token when the access token expires.
             * 
             * Store these values. If a value is missing, return false.
             */
            JObject jsonData = JObject.Parse(data);

            JToken value;
            if (!jsonData.TryGetValue("access_token", out value))
            {
                //access_token missing.
                LastErrorMessage = "Invalid Access Token Response: Missing access_token variable.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            AccessToken = (string)value;
            if (!jsonData.TryGetValue("refresh_token", out value))
            {
                //refresh_token missing.
                LastErrorMessage = "Invalid Access Token Response: Missing refresh_token variable.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            RefreshToken = (string)value;
            if (!jsonData.TryGetValue("expires_in", out value))
            {
                //expires_in missing.
                LastErrorMessage = "Invalid Access Token Response: Missing expires_in variable.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            AccessTokenExpireTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, (int)value));

            //With the access token acquired, save the current client state to the client data file.
            var result = await SaveClientData();
            if (!result)
                return false;

            //If save was successful, the app is logged in.
            IsLoggedIn = true;
            //Success.
            LastError = OAuthClientResult.Success;
            LastErrorMessage = "Operation completed successfully.";

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

            //Create the HTTP request for the access token information.
            HttpRequestMessage accessTokenRequest = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint);
            HttpContent postContent = new ByteArrayContent(postBytes);
            postContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            postContent.Headers.ContentLength = postBytes.Length;
            accessTokenRequest.Content = postContent;

            //The request was successful.
            string data = await SendRequestMessage(accessTokenRequest);

            //Make sure invalid data was not returned.
            if (string.IsNullOrEmpty(data))
            {
                LastErrorMessage = "Server did not return any data.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }

            /*
             * There are 2 objects that must be returned here.
             * access_token: The access token to use in all API calls.
             * expires_in: The amount of time, in seconds, the application can use the access token until 
             * it expires.
             * 
             * Store these values. If a value is missing, return false.
             */
            JObject jsonData = JObject.Parse(data);

            JToken value;
            if (!jsonData.TryGetValue("access_token", out value))
            {
                //access_token missing.
                LastErrorMessage = "Invalid Access Token Response: Missing access_token variable.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            AccessToken = (string)value;
            if (!jsonData.TryGetValue("expires_in", out value))
            {
                //expires_in missing.
                LastErrorMessage = "Invalid Access Token Response: Missing expires_in variable.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            AccessTokenExpireTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, (int)value));

            //With the access token acquired, save the current client state to the client data file.
            var result = await SaveClientData();
            if (!result)
                return false;

            //If save was successful, the app is logged in.
            IsLoggedIn = true;
            //Success.
            LastError = OAuthClientResult.Success;
            LastErrorMessage = "Operation completed successfully.";
            return true;
        }

        /// <summary>
        /// Loads data for this client from the client data file.
        /// </summary>
        /// <returns>A value indicating success or failure of the operation.</returns>
        protected async Task<bool> LoadClientData()
        {
            //Load the data for this OAuth2Client from the ClientData.json file.

            try
            {
                string rawJson = null;

                //Read the json from the file.
                using (
                    var stream = new FileStream("ClientData.json", FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        rawJson = await reader.ReadToEndAsync();
                    }
                }

                //Load the JSON.
                JObject rootObj = JObject.Parse(rawJson);

                //Locate the current provider in the list of providers.
                JObject providerObj = (JObject)rootObj["Providers"][ProviderName];
                bool isEncrypted = Convert.ToBoolean(providerObj["Encrypted"]);
                if (isEncrypted)
                {
                    //Convert each value back to a plain string. For now, they are just base 64 strings.
                    ClientId = Encoding.UTF8.GetString(Convert.FromBase64String((string)providerObj["Client ID"]));
                    ClientSecret =
                        Encoding.UTF8.GetString(Convert.FromBase64String((string)providerObj["Client Secret"]));
                    AccessToken =
                        Encoding.UTF8.GetString(Convert.FromBase64String((string)providerObj["Access Token"]));
                    RefreshToken =
                        Encoding.UTF8.GetString(Convert.FromBase64String((string)providerObj["Refresh Token"]));
                    DateTime expireTimeTemp;
                    DateTime.TryParse(
                        Encoding.UTF8.GetString(Convert.FromBase64String((string)providerObj["Expire Time"])),
                        out expireTimeTemp);
                    AccessTokenExpireTime = expireTimeTemp;

                    //The user is logged in if a valid access token exists.
                    if (DateTime.Now < AccessTokenExpireTime && !string.IsNullOrEmpty(AccessToken))
                        IsLoggedIn = true;
#if DEBUG
                    Console.WriteLine("Access Token Value: " + AccessToken);
                    Console.WriteLine("Refresh Token Value: " + RefreshToken);
                    Console.WriteLine("Access Token Expire Time: " + AccessTokenExpireTime);
#endif

                }
                else
                {
                    //Get everything as a plain string. Call SaveClientData after reading in everything to ensure
                    //the data is resaved, but encrypted.
                    ClientId = (string)providerObj["Client ID"];
                    ClientSecret = (string)providerObj["Client Secret"];
                    AccessToken = (string)providerObj["Access Token"];
                    RefreshToken = (string)providerObj["Refresh Token"];
                    DateTime expireTimeTemp;
                    DateTime.TryParse((string)providerObj["Expire Time"], out expireTimeTemp);
                    AccessTokenExpireTime = expireTimeTemp;

                    //The user is logged in if a valid access token exists.
                    if (DateTime.Now < AccessTokenExpireTime && !string.IsNullOrEmpty(AccessToken))
                        IsLoggedIn = true;

#if DEBUG
                    Console.WriteLine("Access Token Value: " + AccessToken);
                    Console.WriteLine("Refresh Token Value: " + RefreshToken);
                    Console.WriteLine("Access Token Expire Time: " + AccessTokenExpireTime);
#endif
                    //Save the file again, this time making sure the data is encrypted.
                    return await SaveClientData();
                }
            }
            catch (FileNotFoundException)
            {
                LastErrorMessage = "\"ClientData.json\" does not exist in the executable directory.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            catch (IOException ex)
            {
                LastErrorMessage = "An IO error occurred while reading the ClientData.json file. \nMessage: "
                                   + ex.Message;
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            catch (JsonReaderException)
            {
                LastErrorMessage = "Unable to read the Client Data json file. Make sure it is in the correct format.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            catch (JsonException)
            {
                LastErrorMessage = "Unable to parse the Client Data json file. Make sure it is in the correct format.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }

            //Success.
            LastError = OAuthClientResult.Success;
            LastErrorMessage = "Operation completed successfully.";
            return true;
        }

        /// <summary>
        /// Saves the current data of this client to the client data file.
        /// </summary>
        /// <returns>A value indicating success or failure of the operation.</returns>
        protected async Task<bool> SaveClientData()
        {
            //Save the current client data to the file.
            try
            {
                string rawJson = null;

                using (
                    var stream = new FileStream("ClientData.json", FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        //First, open the file to get the JSON document object loaded into memory.
                        rawJson = await reader.ReadToEndAsync();
                    }
                }

                //Load the JSON.
                JObject rootObj = JObject.Parse(rawJson);

                //Locate the current provider in the list of providers.
                JObject providerObj = (JObject)rootObj["Providers"][ProviderName];

                //Encrypted should be marked as true. Saving to this file always saves the data encrypted.
                providerObj["Encrypted"] = 1;
                //Put and encode the client data.
                providerObj["Client ID"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId));
                providerObj["Client Secret"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientSecret));
                providerObj["Access Token"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(AccessToken));
                providerObj["Refresh Token"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(RefreshToken));
                providerObj["Expire Time"] =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(AccessTokenExpireTime.ToString()));

                //Write the new JSON data to the file.
                using (
                    var stream = new FileStream("ClientData.json", FileMode.Create,
                    FileAccess.ReadWrite, FileShare.None))
                {
                    using (var textWriter = new StreamWriter(stream))
                    {
                        using (var jsonWriter = new JsonTextWriter(textWriter))
                        {
                            rootObj.WriteTo(jsonWriter);
                            jsonWriter.Flush();
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                LastErrorMessage = "\"ClientData.json\" does not exist in the executable directory.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            catch (IOException ex)
            {
                LastErrorMessage = "An IO error occurred while reading or writing the ClientData.json file.\nMessage: "
                                   + ex.Message;
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            catch (JsonReaderException)
            {
                LastErrorMessage = "Unable to read the Client Data json file. Make sure it is in the correct format.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            catch (JsonWriterException)
            {
                LastErrorMessage = "Json error writing to the Client Data file.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }
            catch (JsonException)
            {
                LastErrorMessage = "Unable to parse the Client Data json file. Make sure it is in the correct format.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }

            //Success.
            LastError = OAuthClientResult.Success;
            LastErrorMessage = "Operation completed successfully.";
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
        public string LastErrorMessage { get; protected set; }

        /// <summary>
        /// Gets the last HTTP error that occurred in the client. This is only updated when LastError is an HttpError.
        /// </summary>
        public int LastHttpErrorCode { get; protected set; }

        /// <summary>
        /// Gets the type of the last error that occurred in the client.
        /// </summary>
        public OAuthClientResult LastError { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether or not the user is currently logged into the remote service.
        /// </summary>
        public bool IsLoggedIn { get; protected set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs the login process for this OAuth2Client instance.
        /// </summary>
        /// <returns>A value indicating whether or not the login process was successful.</returns>
        public abstract Task<bool> Login();

        /// <summary>
        /// Runs the logout process for this OAuth2Client instance. Logging out equates to clearing the access token
        /// and refresh token, and then saving this change to the client data file.
        /// </summary>
        public async Task<bool> Logout()
        {
            AccessToken = "";
            RefreshToken = "";
            return await SaveClientData();
        }

        /// <summary>
        /// Makes and sends a server an authorized message using the specified url.
        /// </summary>
        /// <param name="url">The url to send the post message to.</param>
        /// <param name="httpMethod">The HTTP method to use when sending the server a message.</param>
        /// <returns>The resulting data returned by the server.</returns>
        public virtual async Task<string> SendAuthorizedRequestMessage(string url, HttpMethod httpMethod)
        {
            HttpRequestMessage message = new HttpRequestMessage(httpMethod, url);
            return await SendAuthorizedRequestMessage(message);
        }

        /// <summary>
        /// Sends a server the specified message. Adds the missing authentication information to the message.
        /// </summary>
        /// <param name="message">The message to send to the server. The url and http method should be set.</param>
        /// <returns>The resulting data returned by the server.</returns>
        public virtual async Task<string> SendAuthorizedRequestMessage(HttpRequestMessage message)
        {
            return await SendAuthorizedRequestMessage(message, null);
        }

        /// <summary>
        /// Sends a server the specified message. Adds the missing authentication information
        /// to the message. This method also reports progress.
        /// </summary>
        /// <param name="message">The message to send to the server. The url and http method should be set.</param>
        /// <param name="progress">An IProgress progress reporter.</param>
        /// <returns></returns>
        public virtual async Task<string> SendAuthorizedRequestMessage(HttpRequestMessage message, 
            IProgress<int> progress)
        {
            if (!IsLoggedIn)
            {
                //Need to be logged in to perform a request that needs authorization.
                LastError = OAuthClientResult.Unauthorized;
                LastErrorMessage = "A user must be logged into the client in order send an authorized request.";
                return null;
            }

            //The request message was already provided. Provide the user agent and authentication header info.
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            message.Headers.UserAgent.Add(new ProductInfoHeaderValue(
                new ProductHeaderValue(Resources.ApplicationName, Resources.Version)));

            return await RequestDataAndCheckResult(message, progress);
        }

        /// <summary>
        /// Makes and sends a server a message using the specified url.
        /// </summary>
        /// <param name="url">The url to send the get message to.</param>
        /// <param name="httpMethod">The HTTP method to use when sending the server a request message.</param>
        /// <returns>The resulting data returned by the server.</returns>
        public virtual async Task<string> SendRequestMessage(string url, HttpMethod httpMethod)
        {
            HttpRequestMessage message = new HttpRequestMessage(httpMethod, url);
            return await SendRequestMessage(message);
        }

        /// <summary>
        /// Sends a server the specified message.
        /// </summary>
        /// <param name="message">The message to send to the server. The url and http method should be set.</param>
        /// <returns>The resulting data returned by the server.</returns>
        public virtual async Task<string> SendRequestMessage(HttpRequestMessage message)
        {
            //The request message was already provided, and authorization information doesn't need to be added.
            //Just add the product information.
            message.Headers.UserAgent.Add(new ProductInfoHeaderValue(
                new ProductHeaderValue(Resources.ApplicationName, Resources.Version)));

            return await RequestDataAndCheckResult(message, null);
        }

        #endregion

        #region Private Methods

        //General method that sends some request to the server and checks the response. Handles as many errors as
        //possible here so client data doesn't need to do as much.
        private async Task<string> RequestDataAndCheckResult(HttpRequestMessage mess, IProgress<int> progress)
        {
            HttpResponseMessage response = null;
            try
            {
                string result;
                if (progress != null)
                {
                    EventHandler<HttpProgressEventArgs> progressDelegate =
                        delegate (object sender, HttpProgressEventArgs e)
                        {
                            progress.Report(e.ProgressPercentage);
                            Console.WriteLine("Progress: " + e.ProgressPercentage);
                        };

                    //Subscrine the delegate to the receive or send progress events of the progress handler.
                    if (mess.Method == HttpMethod.Get)
                        _clientProgress.HttpReceiveProgress += progressDelegate;
                    else if (mess.Method == HttpMethod.Post || mess.Method == HttpMethod.Put)
                        _clientProgress.HttpSendProgress += progressDelegate;

                    //Send the request and store the response.
                    response = await WebClient.SendAsync(mess, HttpCompletionOption.ResponseContentRead);
                    result = await response.Content.ReadAsStringAsync();

                    //Unsubscribe from the corresponding events after downloading or uploading is complete.
                    if (mess.Method == HttpMethod.Get)
                        _clientProgress.HttpReceiveProgress -= progressDelegate;
                    else if (mess.Method == HttpMethod.Post || mess.Method == HttpMethod.Put)
                        _clientProgress.HttpSendProgress -= progressDelegate;
                }
                else
                {
                    //Send the request without reporting progress.
                    response = await WebClient.SendAsync(mess);
                    result = await response.Content.ReadAsStringAsync();
                }
                

                if (!response.IsSuccessStatusCode)
                {
                    //Server responded with an HTTP error. What kind of HTTP error is this?
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        //An unauthorized error means this app does not have permission to do something to the server.
                        //More than likely, the access token has expired. Try to request a new access token using a
                        //refresh token.
                        //TODO: This should automatically attempt to login using a refresh token.
                        LastErrorMessage = "Cannot access the requested resource. " +
                                           "The access token may have expired or access may have been revoked.";
                        LastError = OAuthClientResult.Unauthorized;
                        //Do not return null if the server still returned something, even though the request failed.
                        //The server's will often return data explaining what went wrong.
                        return result;
                    }
                    else
                    {
                        //Some other HTTP error occurred.
                        LastErrorMessage = "HTTP " + response.StatusCode.ToString() +
                                           " Error: " + response.ReasonPhrase;
                        LastHttpErrorCode = (int)response.StatusCode;
                        LastError = OAuthClientResult.HttpError;
                        //Do not return null if the server still returned something, even though the request failed.
                        //The server's will often return data explaining what went wrong.
                        return result;
                    }
                }

                //Everything should be good. Return whatever is is the server returned.

                //Success.
                LastError = OAuthClientResult.Success;
                LastErrorMessage = "Operation completed successfully.";
                return result;
            }
            catch (Exception ex)
            {
                LastError = OAuthClientResult.UnhandledError;
                LastErrorMessage = ex.GetType().Name + ": " + ex.Message;
                //Do not return null if the server still returned something, even though the request failed.
                //The server's will often return data explaining what went wrong.
                return null;
            }
            finally
            {
                mess.Dispose();
                //Cool new syntax that just means if not null, do something.
                response?.Dispose();
            }
        }

        private Uri ConstructUri(string baseUri, NameValueCollection parameters)
        {
            //Construct the parameter part of the uri making some good use of LINQ.
            var keyValuePairs =
                parameters.AllKeys.Select(k => HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(parameters[k]));
            var queryPart = string.Join("&", keyValuePairs);

            return new UriBuilder(baseUri) { Query = queryPart }.Uri;
        }

        #endregion
    }
}
