using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bakkup.Clients
{
    public class GoogleDriveOAuthClient : OAuth2Client
    {
        private const string Client = "315452663633-0onrc7415g98gupp59gbld9c1s8p60h3.apps.googleusercontent.com";
        private const string ClientSecretId = "tlPLlN7xpndsZUZc0Oo0FDfc";
        private const string AuthUri = "https://accounts.google.com/o/oauth2/auth";
        private const string TokenUri = "https://accounts.google.com/o/oauth2/token";

        public override string AuthorizationEndpoint => AuthUri;

        public override string ClientId => Client;

        public override string ClientSecret => ClientSecretId;

        public override string TokenEndpoint => TokenUri;

        public override string ProviderName => "Google Drive";

        public GoogleDriveOAuthClient(Form parentWindow) : base(parentWindow)
        {
            
        }

        public override async Task PerformLogin()
        {
            List<KeyValuePair<string, string>> authorizationParameters = new List<KeyValuePair<string, string>>();
            List<String> scopes = new List<string>();
            
            //Google drive requires the full access scope.
            scopes.Add("https://www.googleapis.com/auth/drive");

            //Initiate a login request.
            var authorizeUrl = await this.RequestAuthorizationUrl(scopes, authorizationParameters);
        }
    }
}
