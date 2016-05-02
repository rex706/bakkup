using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace bakkup.Clients
{
    public class GoogleDriveOAuthClient : OAuth2Client
    {
        private const string Client = "315452663633-0onrc7415g98gupp59gbld9c1s8p60h3.apps.googleusercontent.com";
        private const string ClientSecretId = "tlPLlN7xpndsZUZc0Oo0FDfc";
        private const string AuthUri = "https://accounts.google.com/o/oauth2/auth";
        private const string TokenUri = "https://accounts.google.com/o/oauth2/token";
        private const string Redirect = "http://localhost";

        protected override string AuthorizationEndpoint => AuthUri;

        protected override string ClientId => Client;

        protected override string ClientSecret => ClientSecretId;

        protected override string TokenEndpoint => TokenUri;

        protected override string ProviderName => "Google Drive";

        protected override string RedirectUri => Redirect;

        protected override List<string> AuthorizationFormCloseParams
        {
            get { return new List<string>() {"error_code", "code"}; }
        }

        public GoogleDriveOAuthClient(Form parentWindow) : base(parentWindow)
        {
            
        }

        public override async Task<bool> PerformLogin()
        {
            var authorizationParameters = new List<KeyValuePair<string, string>>();
            var scopes = new List<string>();
            
            //Google drive requires the full access scope.
            scopes.Add("https://www.googleapis.com/auth/drive");

            //Google drive requires a parameter called "nonce" that is a cryptographically strong string.
            authorizationParameters.Add(new KeyValuePair<string, string>("nonce", GenerateRandomString()));

            //Initiate a login request.
            var authorizeUrl = RequestAuthorizationUrl(scopes, authorizationParameters);

            //Make sure authorizeUrl is not null. It is null if something went wrong.
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
                    return false;
                }
            }
            if (query["code"] == null)
            {
                //Something went wrong because there is no "code" parameter.
                return false;
            }

            //Get the authorization code.
            var code = query["code"];

            //Now, using this code, request an access token.
            await RequestAccessToken(code);

            return true;
        }
    }
}
