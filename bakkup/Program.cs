﻿/*
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
            Application.Run(new TestForm());
            //Application.Run(new MainForm(args));
        }
    }
}