using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace bakkup.Clients
{
    /// <summary>
    /// Represents a Google Drive OAuth2 client.
    /// </summary>
    public class GoogleDriveClient : OAuth2Client
    {
        private const string AuthUri = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenUri = "https://www.googleapis.com/oauth2/v4/token";
        private const string Redirect = "http://localhost/";

        protected override string AuthorizationEndpoint => AuthUri;

        protected override string TokenEndpoint => TokenUri;

        protected override string ProviderName => "Google Drive";

        protected override string RedirectUri => Redirect;

        protected override List<string> AuthorizationFormCloseParams
        {
            get { return new List<string>() { "error_code", "code" }; }
        }

        /// <summary>
        /// Goes through the login process. This will either initialize the client with a stored access token, use
        /// a refresh token to get access, or ask the user to sign into their Google Drive account to give the app
        /// permission.
        /// </summary>
        /// <returns>A value indicating success or failure of the operation.</returns>
        public override async Task<bool> Login()
        {
            //Return true if already logged in.
            if (IsLoggedIn)
                return true;

            //Load up the current data for this client. If this fails, login cannot continue.
            var result = await LoadClientData();
            if (!result)
                return false;

            //Is there an access token available that has not yet expired?
            if (!string.IsNullOrEmpty(AccessToken) && DateTime.Now < AccessTokenExpireTime)
            {
                //Because there is a valid access token available, the authorization process is not necessary. The app
                //is logged in right now.
                return true;
            }
            //An access token isn't available or it has expired. Is a refresh token available to use?
            if (!string.IsNullOrEmpty(RefreshToken))
            {
                //Try to use the refresh token to load the data.
                var refreshTokenParams = new NameValueCollection()
                    {
                        {"refresh_token", RefreshToken },
                        {"client_id", ClientId },
                        {"client_secret", ClientSecret },
                        {"grant_type", "refresh_token" }
                    };
                result = await RequestAccessTokenUsingRefreshToken(refreshTokenParams);
                if (result)
                    return true; //Successfully used refresh token to log in.
                if (LastError != OAuthClientResult.Unauthorized)
                    return false; //An error other than Unauthorized occurred. Return false.
                //At this point, the user will need to go through the authorization process to log in.
            }

            var authorizationParameters = new NameValueCollection();
            var scopes = new List<string>();

            //Google drive requires the full access scope.
            scopes.Add("https://www.googleapis.com/auth/drive");

            //Google drive requires a parameter called "nonce" that is a cryptographically strong string.
            authorizationParameters.Add("nonce", GenerateRandomString());

            //Initiate a login request.
            var authorizeUrl = RequestAuthorization(scopes, authorizationParameters);

            //Make sure authorizeUrl is not null. If it is null then something went wrong.
            if (authorizeUrl == null)
            {
                LastError = OAuthClientResult.UnexpectedError;
                LastErrorMessage = "Authorization URL is null or empty.";
                return false;
            }

            //Check the parameters of the url. Google Drive puts an "error_code" parameter upon failure, set to
            //access_denied if the user denies this app access to their Google Drive. Otherwise, it sets a 
            //"code" parameter with the code to use to get the initial access token.
            var builder = new UriBuilder(authorizeUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);
            if (query["error_code"] != null)
            {
                //Error while trying to access user's account.
                if (query["error_code"].StartsWith("access_denied"))
                {
                    LastErrorMessage = "User denied access to their Google Drive.";
                    LastError = OAuthClientResult.UserCancelled;
                    return false;
                }
            }
            if (query["code"] == null)
            {
                //Something went wrong because there is no "code" parameter.
                LastErrorMessage = "Google Drive did not provide \"code\" parameter in response.";
                LastError = OAuthClientResult.UnexpectedError;
                return false;
            }

            //Construct the parameters for getting an access token.
            var accessTokenParams = new NameValueCollection()
            {
                {"grant_type", "authorization_code" },
                {"code", query["code"] },
                {"redirect_uri", RedirectUri },
                {"client_id", ClientId },
                {"client_secret", ClientSecret }
            };

            //Now, using the above parameters, request an access token.
            return await RequestAccessToken(accessTokenParams);
        }

        /// <summary>
        /// Sends a request message to Google Drive using a url and HTTP method. This implementation of 
        /// SendRequestMessage will check for errors returned as JSON.
        /// </summary>
        /// <param name="url">The url to send the get message to.</param>
        /// <param name="httpMethod">The HTTP method to use when sending the server a request message.</param>
        /// <returns>The resulting data returned by the server.</returns>
        public override async Task<string> SendRequestMessage(string url, HttpMethod httpMethod)
        {
            //Note this will call the base SendRequestMessage(string, HttpMethod), which will call the derived
            //SendRequestMessage(HttpRequestMessage).
            return await base.SendRequestMessage(url, httpMethod);
        }

        /// <summary>
        /// Sends a request message to Google Drive. This implementation of SendRequestMessage will check for errors
        /// returned as JSON.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The resulting data returned by the server.</returns>
        public override async Task<string> SendRequestMessage(HttpRequestMessage message)
        {
            string result = await base.SendRequestMessage(message);
            //Did the server return a successful response? If not, it probably returned the exact error as JSON.
            if (string.IsNullOrEmpty(result))
            {
                //Nothing to be done here, as the server did not return a body. Let the error message remain what
                //it is.
                return result;
            }

            if (LastError != OAuthClientResult.Success)
            {
                try
                {
                    //Parse the error json text.
                    JObject rootObj = JObject.Parse(result);
                    //Set last error message to the error Google Drive returned, which may be more descriptive.
                    LastErrorMessage = "Google Drive Error Message Response: " + (string)rootObj["error"]["message"];
                }
                catch (Exception)
                {
                    //Could not parse the error text returned by Google, so nothing to be done.
                    return null;
                }
                
            }

            //Return the data, having parsed it for possible error messages.
            return result;
        }

        /// <summary>
        /// Sends an authorized request message to Google Drive using a url and HTTP method. This implementation of 
        /// SendAuthorizedRequestMessage will check for errors returned as JSON.
        /// </summary>
        /// <param name="url">The url to send the get message to.</param>
        /// <param name="httpMethod">The HTTP method to use when sending the server a request message.</param>
        /// <returns>The resulting data returned by the server.</returns>
        public override async Task<string> SendAuthorizedRequestMessage(string url, HttpMethod httpMethod)
        {
            return await base.SendAuthorizedRequestMessage(url, httpMethod);
        }

        /// <summary>
        /// Sends an authorized request message to Google Drive. This implementation of SendAuthorizedRequestMessage
        /// will check for errors returned as JSON.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The resulting data returned by the server.</returns>
        public override async Task<string> SendAuthorizedRequestMessage(HttpRequestMessage message)
        {
            return await SendAuthorizedRequestMessage(message, null);
        }

        /// <summary>
        /// Sends an authorized request message to Google Drive. This implementation of SendAuthorizedRequestMessage
        /// will check for errrors returned as JSON. It will also report progress.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="progress">An IProgress progress reporter.</param>
        /// <returns>The resulting data returned by the server.</returns>
        public override async Task<string> SendAuthorizedRequestMessage(HttpRequestMessage message,
            IProgress<int> progress)
        {
            string result = await base.SendAuthorizedRequestMessage(message, progress);

            //Did the server return a successful response? If not, it probably returned the exact error as JSON.
            if (string.IsNullOrEmpty(result))
            {
                //Nothing to be done here, as the server did not return a body.
                return result;
            }

            if (LastError != OAuthClientResult.Success)
            {
                try
                {
                    //Parse the error json text.
                    JObject rootObj = JObject.Parse(result);
                    //Set last error message to the error Google Drive returned, which may be more descriptive.
                    LastErrorMessage = "Google Drive Error Message Response: " + (string)rootObj["error"]["message"];
                }
                catch (Exception)
                {
                    //Could not parse the error text returned by Google, so nothing to be done.
                    return null;
                }

            }

            //Return the data, having parsed it for possible error messages.
            return result;
        }
    }
}
