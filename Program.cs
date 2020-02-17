using JT_Suite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Windows.Forms;

namespace MultiFaceRec
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            try
            {
                Application.Run(new Form1());
            }
            catch (Exception ex) {
                // Catch all exceptions
                MessageBox.Show(ex.Message);
            }
        }
    }
}
