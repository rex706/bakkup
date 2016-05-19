/*
 * Nkosi Notes:
 * 1. UI elements that you are making use of should always have a useful name. "button1, button2, button3" etc do not
 * tell you what the purpose of the control is.
 * 2. C# has a different style than Java. Don't forget that you do not camel case name functions. Each first letter of
 * a word in a function should be capital.
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace bakkup
{
    internal static class Program
    {
        //Nkosi Note: Do not use program wide singletons when they aren't necessary.

        //Not necessary variables. ServicePickerForm.SelectedStorageHandler resolves these.
        //public static bool GD = false;
        //public static bool OD = false;
        //public static bool DB = false;

        //Not a necessary singleton variable. Simply re-open the ServicePickerForm window within the
        //MainForm code. Refer to MainForm.serviceLabel_Click method.
        //public static bool SwitchRequest = false;
        //No longer necessary because SwitchRequest has become unnecessary.
        //public static bool FirstStart = true;  

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*
             * Nkosi Note: Application Run should only be called once in the application. Use properties
             * in the ServicePickerForm class to detect which service was chosen instead of application
             * wide singleton variables (public static variables any part of the application can access.)
             * Singletons should generally be avoided when possible.
             */
            Application.Run(new MainForm(args));

            //while (GD == false && OD == false && DB == false)
            //{
            //    DialogResult answer = MessageBox.Show("You must select a cloud provider", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            //    if (answer == DialogResult.Cancel)
            //        return;

            //while (SwitchRequest == true || FirstStart == true)
            //{
            //    GD = false;
            //    OD = false;
            //    DB = false;

            //    SwitchRequest = false;

            //    //Application.Run(new TestForm());
            //    Application.Run(new ServicePickerForm());

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



