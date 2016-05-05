﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

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
            //Before going through the whole login process, check if there is a not expired access token available.
            

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
                return false;
            }

            //Check the parameters of the url. Google Drive puts an "error_code" parameter upon failure, set to
            //access_denied if the user denies this app access to their Google Drive. Otherwise, it sets a 
            //"" parameter with the 
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

        public override void Logout()
        {
            
            throw new NotImplementedException();
        }
    }
}