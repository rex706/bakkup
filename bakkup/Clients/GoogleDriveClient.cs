using System;
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
        private const string Client = "315452663633-0onrc7415g98gupp59gbld9c1s8p60h3.apps.googleusercontent.com";
        private const string ClientSecretId = "tlPLlN7xpndsZUZc0Oo0FDfc";
        private const string AuthUri = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenUri = "https://www.googleapis.com/oauth2/v4/token";
        private const string Redirect = "http://localhost/";

        protected override string AuthorizationEndpoint => AuthUri;

        protected override string ClientId => Client;

        protected override string ClientSecret => ClientSecretId;

        protected override string TokenEndpoint => TokenUri;

        protected override string ProviderName => "Google Drive";

        protected override string RedirectUri => Redirect;

        protected override List<string> AuthorizationFormCloseParams
        {
            get { return new List<string>() { "error_code", "code" }; }
        }

        public GoogleDriveClient(Form parentWindow) : base(parentWindow)
        {
            
        }

        public override async Task<bool> PerformLogin()
        {
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
                    LastError = "User denied access.";
                    return false;
                }
            }
            if (query["code"] == null)
            {
                //Something went wrong because there is no "code" parameter.
                LastError = "Google Drive did not provide \"code\" parameter in response.";
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
    }
}
