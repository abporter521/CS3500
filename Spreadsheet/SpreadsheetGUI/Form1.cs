using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// This is a private nested class that will control all the
        /// changes that will happen to a spreadsheet including
        /// method calls
        /// </summary>
        private class Controller
        {
            //Holds a spreadsheet object to keep track of the changes
            AbstractSpreadsheet ss;
            public Controller (Spreadsheet spreadsheet)
            {
                ss = spreadsheet;
            }

        }
        
        public Form1()
        {
            InitializeComponent();
            spreadsheetUI.SelectionChanged += OnSelectionChange;
            Controller cont = new Controller(new Spreadsheet());
            
        }

        private void OnSelectionChange (SpreadsheetPanel ssp)
        {
            ssp.GetSelection(out int col, out int row);

        }

        /// <summary>
        /// Have the status bar at the bottom display the 
        /// saved status of the spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //status.
        }
    }
}
