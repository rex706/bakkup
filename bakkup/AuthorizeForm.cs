using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotNetOpenAuth.OAuth2;

namespace bakkup
{
    /// <summary>
    /// Represents a window that allows a user to login to a specific website.
    /// </summary>
    public partial class AuthorizeForm : Form
    {
        private ClientAuthorizationView _loginControl;

        public AuthorizeForm()
        {
            InitializeComponent();

            //Add the DotNetOpenAuth authorization control.
            _loginControl = new ClientAuthorizationView();
            _loginControl.Dock = DockStyle.Fill;
            Controls.Add(_loginControl);
        }

        public ClientAuthorizationView LoginControl => _loginControl;
    }
}
