/*
 * Nkosi Notes:
 * 1. UI elements that you are making use of should always have a useful name. "button1, button2, button3" etc do not
 * tell you what the purpose of the control is.
 * 2. C# has a different style than Java. Don't forget that you do not camel case name functions. Each first letter of
 * a word in a function should be capital.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace bakkup
{
    static class Program
    {
        /// <summary>
        /// Copies save files to and from a Google Drive folder to keep saves up to date and accessable from anywhere.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*
             * Nkosi Note: Application Run should only be called once in the application. Use properties
             * in the ServicePickerForm class to detect which service was chosen instead of application
             * wide singleton variables (public static variables any part of the application can access.)
             * Singletons should generally be avoided when possible.
             */

            ServicePickerForm form = new ServicePickerForm();
            form.ShowDialog();
            if (form.SelectedStorageHandler == null)
                return; //Form was closed and no storage handler was successfully set, so just exit.
            //TODO: Pass the successfully initialized storage handler to the MainForm and do stuff with it.
            //Application.Run(new MainForm(args, form.SelectedStorageHandler));
            Application.Run(new MainForm(args));

            //while (GD == false && OD == false && DB == false)
            //{
            //    DialogResult answer = MessageBox.Show("You must select a cloud provider", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            //    if (answer == DialogResult.Cancel)
            //        return;

            //    Application.Run(new ServicePickerForm());
            //}


        }
    }
}



