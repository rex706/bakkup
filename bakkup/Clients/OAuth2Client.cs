using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using DotNetOpenAuth.OAuth2;

namespace bakkup.Clients
{
    public abstract class OAuth2Client
    {
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
        /// Runs the login process for this OAuth2Client instance.
        /// </summary>
        /// <returns>A value indicating whether or not the login process was successful.</returns>
        public abstract Task<bool> PerformLogin();

        private readonly Form _parentWindow;
        private UserAgentClient _oAuthClient;

        protected OAuth2Client(Form parentWindow)
        {
            _parentWindow = parentWindow;
        }

        protected void InitializeClient()
        {
            _oAuthClient = new UserAgentClient(MakeServerDescription(), ClientId, ClientSecret);
        }

        protected string RequestAuthorizationUrl(List<String> scopes, 
            List<KeyValuePair<string, string>> extraParams)
        {
            var description = _oAuthClient.AuthorizationServer;

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
            

            var userAuthUrl = _oAuthClient.RequestUserAuthorization(scopes, null, new Uri(RedirectUri));
            
            //Use DotNetOpenAuth's built in web browser window to allow the user to verify authorization.
            var loginWindow = new AuthorizeForm();
            loginWindow.Text = "Login To " + ProviderName;
            loginWindow.RequestAuthorization(userAuthUrl.ToString(), AuthorizationFormCloseParams);
            loginWindow.ShowDialog(_parentWindow);

            //Return the authorization redirect url. Handling of parameters in this url is done by classes that
            //extend this class.
            return loginWindow.AuthorizationRedirectUrl;
        }

        protected async Task<bool> RequestAccessToken(string authorizationCode)
        {
            
            return true;
        }

        protected async Task<bool> RequestAccessTokenWithRefreshToken(string refreshToken)
        {
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

        private AuthorizationServerDescription MakeServerDescription()
        {
            var description = new AuthorizationServerDescription();
            description.ProtocolVersion = ProtocolVersion.V20;
            description.AuthorizationEndpoint = new Uri(AuthorizationEndpoint);
            description.TokenEndpoint = new Uri(TokenEndpoint);
            return description;
        }
    }
}
