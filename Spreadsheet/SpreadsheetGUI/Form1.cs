using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// This is the class for the spreadsheet form
        /// It contains methods that fire in response to cell
        /// changes and to other user inputs like button pushes
        /// </summary>
        SpreadsheetPanel spreadsheetPanel1 = new SpreadsheetPanel();
        //Controller that acts as liaison between spreadsheet logic and UI
        Controller cont;

        public Form1()
        {
            InitializeComponent();
            spreadsheetUI.SelectionChanged += OnSelectionChange;
            spreadsheetUI.Scroll += scrollV;
            cont = new Controller();
            //Allows the enter button to push the insert button
            AcceptButton = enterButton;
            //Selects cell A1 as the first cell
            spreadsheetUI.SetSelection(0, 0);
            //Sets the control to the formula field
            ActiveControl = formulaField;
            //Sets cell indicator
            cellSelected.Text = ("A1");
            //Show user no changes have been made
            savedBox.Text = "No changes made.";

        }
        /// <summary>
        /// Triggers when the cell selected changes
        /// </summary>
        /// <param name="ssp"></param>
        private void OnSelectionChange(SpreadsheetPanel ssp)
        {
            spreadsheetUI.GetSelection(out int col, out int row);
            //Console.WriteLine(col + ", " + row);
            //Get value from cell
            spreadsheetUI.GetValue(col, row, out string value);
            cellSelected.Text = cont.DigitToVar(col, row);
            valueBox.Text = value;
            //If the value of the cell is empty, clear the formula field and be
            //prepared to receive input
            if (value == "")
            {
                formulaField.Clear();
                formulaField.Focus();
            }
            //If there is already data in the cell, display its contents in formula bar
            else
            {
                //Notify the user what the error is for each cell with a formula error
                if (value == "Error")
                {
                    FormulaError fe = new FormulaError();
                    fe = (FormulaError)cont.GetCellValue(col, row);
                    MessageBox.Show("Formula Error in " + cont.DigitToVar(col, row)
                        + " has the following error:\n" + fe.Reason);
                }
                //Puts formula with = into the formula bar
                if (cont.GetCellContents(col, row) is Formula)
                {
                    StringBuilder s = new StringBuilder();
                    s.Append("=" + cont.GetCellContents(col, row).ToString());
                    formulaField.Text = s.ToString();
                }
                //Otherwise justs puts contents in formula bar
                else
                    formulaField.Text = cont.GetCellContents(col, row).ToString();
                formulaField.Focus();
            }
        }
        /// <summary>
        /// Triggered by the enter button on the spreadsheet
        /// When the user submits valid data into the  formula bar
        /// the rest of the spreadsheet should change in accordance 
        /// to the new data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void enterButton_Click(object sender, EventArgs e)
        {
            //Gets the cell selected
            spreadsheetUI.GetSelection(out int col, out int row);
            //Sets the cell to the value in the formula bar
            //try here for error in construction
            try
            {
                List<string> cells = cont.SetCellContents(col, row, formulaField.Text);
                foreach (string cell in cells)
                {
                    cont.VarToDigit(cell, out int c, out int r);
                    //Console.WriteLine(c + ", " + r);
                    //Display Error if value is a FormulaError
                    if (cont.GetCellValue(c, r).ToString() == "SpreadsheetUtilities.FormulaError")
                        spreadsheetUI.SetValue(c, r, "Error");
                    //Else display the value as a string
                    else
                        spreadsheetUI.SetValue(c, r, cont.GetCellValue(c, r).ToString());
                }

            }
            //If the formula was invalid, display a message to user
            catch (Exception)
            {
                MessageBox.Show("Your formula contained invalid characters.  Please double check" +
                    " and try again.");
            }

            //Put this feature in the saved
            savedBox.Text = "Changes not saved.";
            //This moves the selected box automatically after hitting the button and moves to next row if 
            //at the end of a row
            if (!spreadsheetUI.SetSelection(col + 1, row))
                spreadsheetUI.SetSelection(0, row + 1);
            OnSelectionChange(spreadsheetUI);
        }
        /// <summary>
        /// When the user clicks on save from the pull down menu, save file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        /// <summary>
        /// Helper method to save file
        /// </summary>
        private void SaveFile()
        {
            
            
                //Open save file dialog box
                saveFileWindow.ShowDialog();
                string s = saveFileWindow.FileName;
                //If file name ends with correct extension, save
                if (Regex.IsMatch(s, ".sprd$"))
                    cont.Save(s);
                //Else add correct extension and save
                else
                {
                    s = s.Insert(s.Length, ".sprd");
                    cont.Save(s);
                }
                //Display to user that changes were saved.
                savedBox.Text = "Changes saved";          
        }

        /// <summary>
        /// This opens the open file dialog box for the user to open a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile.ShowDialog();

        }

        /// <summary>
        /// Opens up a new window with the loaded spreadsheet from the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFile_FileOk(object sender, CancelEventArgs e)
        {
            //Check if the user wants to save the file first

            //Check if the file is of the correct type
            if (!Regex.IsMatch(openFile.FileName, ".sprd$"))
            {
                //Tell user that file is not of correct type
                MessageBox.Show("File must be of type .sprd");

            }
            else
            {
                //Open new window wth the spreadsheet           
                cont = new Controller(openFile.FileName);
                //Clear the spreadsheet
                spreadsheetUI.Clear();
                //For each coordinate in the spreadsheet, display the value
                foreach (Tuple<int, int> coor in cont.GetNonEmptyCells())
                {
                    //Display Error if value is a FormulaError
                    if (cont.GetCellValue(coor.Item1, coor.Item2).ToString() == "SpreadsheetUtilities.FormulaError")
                        spreadsheetUI.SetValue(coor.Item1, coor.Item2, "Error");
                    //Display the values 
                    else
                        spreadsheetUI.SetValue(coor.Item1, coor.Item2,
                            cont.GetCellValue(coor.Item1, coor.Item2).ToString());
                }
            }
        }

        /// <summary>
        /// This is the method that allows for multiple forms to be up and running.
        /// Program will terminate when the last window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PS6ApplicationContext.getAppContext().RunForm(new Form1());
        }
        /// <summary>
        /// Runs the message to ask if user wants to save before exiting or changing the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cont.GetChanged())
            {
                DialogResult saver = MessageBox.Show
                    ("Do you want to save this file before exiting?", "Save File", MessageBoxButtons.YesNoCancel);
                if (saver == DialogResult.Yes)
                    SaveFile();
                else if (saver == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            //Else file is already saved and its ok to close
        }

        /// <summary>
        /// If close is selected from the drop down menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult save = MessageBox.Show
                   ("Do you want to save this file before exiting?", "Save File", MessageBoxButtons.YesNoCancel);
            if (cont.GetChanged())
            {
                if (save == DialogResult.Yes)
                    SaveFile();
                else if (save == DialogResult.Cancel)
                    return;
            }
            //No changes in document occured, so we are safe to close. User confirms that no saving required and closes
            Close();
        }

        private void fAQsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void rEADMEDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string readmefile = "Debug\\bin\\SpreadsheetGUI\\Spreadsheet\\README";
            Process.Start(readmefile); 
        }

        private void scrollV(Object sender, ScrollEventArgs args)
        {
            spreadsheetUI.ScrollControlIntoView(ActiveControl);
        }
        /// <summary>
        /// Code found on how to override arrowkeys for selecting cells.
        /// Removed the message boxes that were in the original code and
        /// added my spreadsheetUI commands for desired result.
        /// 
        /// Code Found: http://net-informations.com/q/faq/arrowkeys.html
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            spreadsheetUI.GetSelection(out int col, out int row);
            //capture up arrow key
            if (keyData == Keys.Up)
            {
                spreadsheetUI.SetSelection(col, row - 1);
                OnSelectionChange(spreadsheetUI);
                
                return true;
            }
            //capture down arrow key
            if (keyData == Keys.Down)
            {
                spreadsheetUI.SetSelection(col, row + 1); 
                OnSelectionChange(spreadsheetUI);
                return true;
            }
            //capture left arrow key
            if (keyData == Keys.Left)
            {
                spreadsheetUI.SetSelection(col - 1 , row);
                OnSelectionChange(spreadsheetUI);                
                return true;
            }
            //capture right arrow key
            if (keyData == Keys.Right)
            {
                spreadsheetUI.SetSelection(col + 1, row);
                OnSelectionChange(spreadsheetUI);               
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
