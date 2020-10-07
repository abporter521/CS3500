using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetUtilities;
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
            //Console.WriteLine(col + ", " + row);
            spreadsheetUI.GetValue(col, row, out string value);
            cellSelected.Text = cont.DigitToVar(col, row);
            valueBox.Text = value;
            if (value == "")
            {
                formulaField.Clear();
                formulaField.Focus();
            }
            else
            {
                if (cont.GetCellContents(col, row) is Formula)
                {
                    StringBuilder s = new StringBuilder();
                    s.Append("=" + cont.GetCellContents(col, row).ToString());
                    formulaField.Text = s.ToString();
                }
                else
                    formulaField.Text = cont.GetCellContents(col, row).ToString();
                formulaField.Focus();
            }
        }
        /// <summary>
        /// When the user submits the formula into the bar
        /// the rest of the spreadsheet should change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void enterButton_Click(object sender, EventArgs e)
        {
            //Gets the cell selected
            spreadsheetUI.GetSelection(out int col, out int row);
            //Sets the cell to the value in the formula bar
            //spreadsheetUI.SetValue(col, row, formulaField.Text);
            //try here for error in construction
            try
            {
                List<string> cells = cont.SetCellContents(col, row, formulaField.Text);
                foreach (string cell in cells)
                {
                    cont.VarToDigit(cell, out int c, out int r);
                    //Console.WriteLine(c + ", " + r);
                    spreadsheetUI.SetValue(c, r, cont.GetCellValue(c, r).ToString());
                }

            }
            catch (Exception)
            {
                Console.WriteLine("ya messed up");
            }

            //Put this feature in the saved
            savedBox.Text = "Changes not saved.";
            //This moves the selected box automatically after hitting the button
            if (!spreadsheetUI.SetSelection(col + 1, row))
                spreadsheetUI.SetSelection(0, row + 1);
            OnSelectionChange(spreadsheetUI);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileWindow.ShowDialog();
            string s = saveFileWindow.FileName;
            if (Regex.IsMatch(s, ".sprd$"))
                cont.Save(s);
            else
            {
                s = s.Insert(s.Length, ".sprd");
                cont.Save(s);
            }
            savedBox.Text = "Changes saved";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile.ShowDialog();

        }
    }
}
