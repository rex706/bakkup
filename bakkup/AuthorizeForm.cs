using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace bakkup
{
    /// <summary>
    /// Represents a window that allows a user to login to a specific website.
    /// </summary>
    public partial class AuthorizeForm : Form
    {
        private readonly WebBrowser _loginBrowser;
        private List<string> _closingParamsHint;

        public AuthorizeForm()
        {
            InitializeComponent();

            _loginBrowser = new WebBrowser();
            _loginBrowser.Dock = DockStyle.Fill;
            _loginBrowser.IsWebBrowserContextMenuEnabled = false;
            _loginBrowser.Navigated += _loginBrowser_Navigated;
            Controls.Add(_loginBrowser);

            AuthorizationRedirectUrl = null;
            //Assume authorization was cancelled until the user actually completes the process, in which case this will
            //be set to false.
            WasAuthorizationCancelled = true;
        }

        public void RequestAuthorization(string authorizationUrl, List<string> closingParamsHint)
        {
            //Provide a list of url parameters this window should look for to let it know the user has completed the
            //login process and this window should close.
            this._closingParamsHint = closingParamsHint;
            //Navigate to the requested page.
            _loginBrowser.Navigate(authorizationUrl);
        }

        private void _loginBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //Check the parameters in the url. Do nothing if there is a response_type parameter.
            var query = HttpUtility.ParseQueryString(new UriBuilder(e.Url).Query);
            //Do nothing if the current url does not contain any parameters in the closingParamsHint list of strings.
            foreach (var param in _closingParamsHint)
            {
                if (query[param] != null)
                {
                    //The window should close. This url represents the completion of the login process.

                    //Set AuthorizationRedirectUrl to whatever the page has redirected to once the user has given access
                    //or denied access to this app.
                    AuthorizationRedirectUrl = e.Url.ToString();
                    //Since the user completed authorization, WasAuthorizationCancelled should be false.
                    WasAuthorizationCancelled = false;
                    //Close the window.
                    Close();
                    return;
                }
            }

            
        }

        /// <summary>
        /// The url containing the resulting access token or any other necessary parameter on redirect from the
        /// login page.
        /// </summary>
        public string AuthorizationRedirectUrl { get; private set; }

        /// <summary>
        /// If true, authorization was cancelled by closing the authorization window.
        /// </summary>
        public bool WasAuthorizationCancelled { get; private set; }
    }
}
