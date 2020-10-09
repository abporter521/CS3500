using SpreadsheetGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace SS
{
    /// <summary>
    /// Keeps track of how many top-level forms are running
    /// Used from demo
    /// 
    /// Andrew Porter
    /// </summary>
    class PS6ApplicationContext : ApplicationContext
    {
        // Number of open forms
        private int formCount = 0;

        // Singleton ApplicationContext
        private static PS6ApplicationContext appContext;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private PS6ApplicationContext()
        {
        }

        /// <summary>
        /// Returns the one DemoApplicationContext.
        /// </summary>
        public static PS6ApplicationContext getAppContext()
        {
            if (appContext == null)
            {
                appContext = new PS6ApplicationContext();
            }
            return appContext;
        }

        /// <summary>
        /// Runs the form
        /// </summary>
        public void RunForm(Form form)
        {
            // One more form is running
            formCount++;

            // When this form closes, we want to find out
            form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

            // Run the form
            form.Show();
        }

    }


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
         static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start an application context and run one form inside it
            PS6ApplicationContext appContext = PS6ApplicationContext.getAppContext();
            appContext.RunForm(new Form1());
            Application.Run(appContext);
        }
    }
}
