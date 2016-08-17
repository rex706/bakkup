/*
 * Nkosi Notes:
 * 1. UI elements that you are making use of should always have a useful name. "button1, button2, button3" etc do not
 * tell you what the purpose of the control is.
 * 2. C# has a different style than Java. Don't forget that you do not camel case name functions. Each first letter of
 * a word in a function should be capital.
 */

using System;
<<<<<<< HEAD
=======
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
>>>>>>> origin/master
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace bakkup
{
    internal static class Program
    {

        public static bool GD = false;
        public static bool DB = false;
        public static bool OD = false;

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
<<<<<<< HEAD
            Application.Run(new ServicePickerForm());
=======

            /*
             * Nkosi Note: Application Run should only be called once in the application. Use properties
             * in the ServicePickerForm class to detect which service was chosen instead of application
             * wide singleton variables (public static variables any part of the application can access.)
             * Singletons should generally be avoided when possible.
             */

>>>>>>> origin/master
            Application.Run(new MainForm(args));
        }
    }
}



