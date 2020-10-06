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
        ///// <summary>
        ///// This is a private nested class that will control all the
        ///// changes that will happen to a spreadsheet including
        ///// method calls
        ///// </summary>
        //private class Controller
        //{
        //    //Holds a spreadsheet object to keep track of the changes
        //    AbstractSpreadsheet ss;
        //    public Controller (Spreadsheet spreadsheet)
        //    {
        //        ss = spreadsheet;
        //    }

        //}
        SpreadsheetPanel SpreadsheetPanel1= new SpreadsheetPanel();
        
        public Form1()
        {
            InitializeComponent();
            spreadsheetUI.SelectionChanged += OnSelectionChange;
            AcceptButton = enterButton;
            //Sets cell indicator
            cellSelected.Text = ("A1");
            //Puts cursor on formula field
            formulaField.Focus();
            //Show user no changes have been made
            savedBox.Text = "No changes made.";
            
        }

        private void OnSelectionChange (SpreadsheetPanel ssp)
        {
            spreadsheetUI.GetSelection(out int col, out int row);
            spreadsheetUI.GetValue(col, row, out string value);
            
            if (value == "")
            {
                formulaField.Clear();
                formulaField.Focus();
            }
            else
            {
                formulaField.Text = value;
                formulaField.Focus();
            }
        }

        private void enterButton_Click(object sender, EventArgs e)
        {
            spreadsheetUI.GetSelection(out int col, out int row);
            spreadsheetUI.SetValue(col, row, formulaField.Text);
            //Put this feature in the saved
            savedBox.Text = "Changes not saved.";
            if (!spreadsheetUI.SetSelection(col+1, row))
                spreadsheetUI.SetSelection(0, row+1);
            OnSelectionChange(spreadsheetUI);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Controller to save the file
        }
    }
}
