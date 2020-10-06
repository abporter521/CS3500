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
        
        SpreadsheetPanel spreadsheetPanel1 = new SpreadsheetPanel();
        Controller cont;
        public Form1()
        {
            InitializeComponent();
            spreadsheetUI.SelectionChanged += OnSelectionChange;
            cont = new Controller();
            //Allows the enter button to push the insert button
            AcceptButton = enterButton;
            //Sets cell indicator
            cellSelected.Text = ("A1");
            //Puts cursor on formula field
            formulaField.Focus();
            //Show user no changes have been made
            savedBox.Text = "No changes made.";

        }

        private void OnSelectionChange(SpreadsheetPanel ssp)
        {
            spreadsheetUI.GetSelection(out int col, out int row);
            spreadsheetUI.GetValue(col, row, out string value);
            cellSelected.Text = cont.DigitToVar(col, row);
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
            //This moves the selected box automatically after hitting the button
            if (!spreadsheetUI.SetSelection(col + 1, row))
                spreadsheetUI.SetSelection(0, row + 1);
            OnSelectionChange(spreadsheetUI);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Controller to save the file
        }
    }
}
