using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using DotNetOpenAuth.OAuth2;

namespace bakkup.Clients
{
    public abstract class OAuth2Client
    {
        public abstract string AuthorizationEndpoint { get; }

        public abstract string TokenEndpoint { get; }

        public abstract string ClientId { get; }

        public abstract string ClientSecret { get; }

        public abstract string ProviderName { get; }

        public abstract Task PerformLogin();

        private Form _parentWindow;

        protected OAuth2Client(Form parentWindow)
        {
            _parentWindow = parentWindow;
        }

        protected async Task<string> RequestAuthorizationUrl(List<String> scopes, 
            List<KeyValuePair<string, string>> extraParams)
        {
            var description = new AuthorizationServerDescription();
            description.ProtocolVersion = ProtocolVersion.V20;
            description.AuthorizationEndpoint = new Uri(AuthorizationEndpoint);
            description.TokenEndpoint = new Uri(TokenEndpoint);

            if (extraParams.Count > 0)
            {
                var builder = new UriBuilder(description.AuthorizationEndpoint);
                var query = HttpUtility.ParseQueryString(builder.Query);
                //Add the extra parameters to the server description's authorization endpoint url. The @ symbol here
                //just lets me use a C# reserved keyword as a variable name within this context.
                foreach (var @param in extraParams)
                {
                    query[param.Key] = param.Value;
                }
                builder.Query = query.ToString();
                description.AuthorizationEndpoint = new Uri(builder.ToString());
            }
            

            var client = new UserAgentClient(description, ClientId, ClientSecret);
            var userAuthUrl = client.RequestUserAuthorization(scopes);
            
            //Use DotNetOpenAuth's built in web browser window to allow the user to verify authorization.
            var loginWindow = new AuthorizeForm();
            loginWindow.Text = "Login To " + ProviderName;
            loginWindow.LoginControl.Client = client;
            loginWindow.ShowDialog(_parentWindow);
            

            return null;
        }

        protected async Task<bool> RequestAccessToken(string authorizationCode)
        {
            return true;
        }

        protected async Task<bool> RequestAccessTokenWithRefreshToken(string refreshToken)
        {
            return true;
        }
    }
}
