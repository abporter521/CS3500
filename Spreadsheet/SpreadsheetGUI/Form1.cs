using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        //Controller that acts as liaison between spreadsheet logic and UI
        Controller cont;
        //Bool for saved before
        bool savedBefore = false;
        //string with name
        string fileName;


        public Form1()
        {
            InitializeComponent();
            spreadsheetUI.SelectionChanged += OnSelectionChange;
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
            //Get value from cell
            spreadsheetUI.GetValue(col, row, out string value);
            //Display in the cell selected, the cell name
            cellSelected.Text = cont.DigitToVar(col, row);
            //Display to the user the value of the selected cell
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
                    //Message displaying specific error when cell is clicked on
                    MessageBox.Show("Formula Error in " + cont.DigitToVar(col, row)
                        + " has the following error:\n" + fe.Reason);
                }
                //Puts formula with = into the formula bar
                if (cont.GetCellContents(col, row) is Formula)
                {
                    //Adds the equal sign to the contents
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
            //cell coordinates
            int col, row;
            //If the user wants to input or jump to a cell
            if (cellSetterBox.MaskCompleted)
            {
                //Get the coordinates of the cell inputted by user
                cont.VarToDigit(cellSetterBox.Text, out col, out row);
                //Gets the selection
                spreadsheetUI.SetSelection(col - 1, row);
                //Changes the cell contents of indicated cell
                ChangeCell(col, row);
                //Resets the cell box
                cellSetterBox.Clear();
            }
            //Just input formula into currently selected cell     
            else
            {
                //Gets the cell selected
                spreadsheetUI.GetSelection(out col, out row);
                ChangeCell(col, row);
            }

            //Put this feature in the saved
            savedBox.Text = "Changes not saved.";
            //Checks if the user wants the autosaver enabled
            if (autosaverEnabled.Checked)
            {
                //Disable Enter button
                enterButton.Enabled = false;
                //Run Autosaver
                autoSaver.RunWorkerAsync();
            }
            //This moves the selected box automatically after hitting the button and moves to next row if 
            //at the end of a row
            if (!spreadsheetUI.SetSelection(col + 1, row))
                spreadsheetUI.SetSelection(0, row + 1);
            OnSelectionChange(spreadsheetUI);
        }
        
        /// <summary>
        /// Helper method that changes the content/value of a cell depending
        /// on what is inside the formula bar
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void ChangeCell(int col, int row)
        {
            //Sets the cell to the value in the formula bar
            //try here for error in construction
            try
            {
                //Gets the list of dependent cells of the selected cell
                List<string> cells = cont.SetCellContents(col, row, formulaField.Text);
                foreach (string cell in cells)
                {
                    //Convert each cell name to coordinates 
                    cont.VarToDigit(cell, out int c, out int r);
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
                MessageBox.Show("Your formula contained invalid characters or operator order. " +
                    " Please double check and try again.");
            }
        }
        
        /// <summary>
        /// When the user clicks on save from the pull down menu, save file
        /// Name of method was a misnomer by accident. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Run the save file method
            SaveFile();
        }

        /// <summary>
        /// Helper method to save file
        /// </summary>
        private void SaveFile()
        {
            if (!savedBefore)
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
                //This file has been saved before
                savedBefore = true;
                //Save filename
                fileName = s;
                
            }
            //Save without having to open dialog box and "overwriting" each time.
            else
                cont.Save(fileName);
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
            //If the spreadsheet has changed according to the controller, verify saving before closing
            if (cont.GetChanged())
            {
                //Display verification message
                DialogResult saver = MessageBox.Show
                    ("Do you want to save this file before exiting?", "Save File", MessageBoxButtons.YesNoCancel);
                //If user clicks yes, save
                if (saver == DialogResult.Yes)
                    SaveFile();
                //If user clicks cancels, stops the closing form operation and returns to spreadsheet
                else if (saver == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                //Else form closes
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
            //Asks if user wants to save before exiting
            DialogResult save = MessageBox.Show
                   ("Do you want to save this file before exiting?", "Save File", MessageBoxButtons.YesNoCancel);
            //If the spreadsheet has changed according to the controller object
            if (cont.GetChanged())
            {
                //If yes, then save, if cancel, return to spreadsheet
                if (save == DialogResult.Yes)
                    SaveFile();
                else if (save == DialogResult.Cancel)
                    return;
            }
            //No changes in document occured, so we are safe to close. User confirms that no saving required and closes
            Close();
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
                //Change selection
                spreadsheetUI.SetSelection(col, row - 1);
                OnSelectionChange(spreadsheetUI);

                return true;
            }
            //capture down arrow key
            if (keyData == Keys.Down)
            {
                //Change selection
                spreadsheetUI.SetSelection(col, row + 1);
                OnSelectionChange(spreadsheetUI);
                return true;
            }
            //capture left arrow key
            if (keyData == Keys.Left)
            {
                //Change selection
                spreadsheetUI.SetSelection(col - 1, row);
                OnSelectionChange(spreadsheetUI);
                return true;
            }
            //capture right arrow key
            if (keyData == Keys.Right)
            {
                //Change selection
                spreadsheetUI.SetSelection(col + 1, row);
                OnSelectionChange(spreadsheetUI);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        /// <summary>
        /// Help message for user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void whereIsTheHorizontalScrollBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Readjusting the window will make the horizontal bar appear");
        }
        
        /// <summary>
        /// General how-to for the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rEADMEDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Cells can be selected by mouse, arrow keys, or the Go To Cell box. " +
                "When the user inputs data into the formula bar, the value will be moved to that cell" +
                " or alternately into the cell designated in the Go To box. Formulas are inputted " +
                "when the user hits the enter button on the form or the enter key. Error cells can be clicked " +
                "on to read the specific error of the cell. Opening, Saving, Closing and New can be accessed" +
                " from the file menu or selected by shortcut keys.");
        }
        
        /// <summary>
        /// Answer to FAQ question
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void didTheCreatorHaveEyeSurgeryThisWeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Yes he did and because of it, lost a day to work on this assignment.");
        }
        
        /// <summary>
        /// Message to the user about saved bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savedStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The bar at the bottom displays the saved status of the document. " +
                "If changes are saved, the program will close without asking to save, otherwise " +
                "a save option will appear.  The user will always be able to see the status of their" +
                "document");
        }
        
        /// <summary>
        /// Message for the user about the go to cell box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goToCellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature allows the user to input data into a cell without having " +
                "to select the cell manually.  If there is a formula in the formula bar and nothing " +
                "in the Go To Cell box, the formula box contents will enter directly into the currently " +
                "selected cell. Inputs into the Go To Cell box are limited to valid cell names which are " +
                "1 letter followed by 1 or 2 digits. If formula bar is empty, Go To Cell will input an empty " +
                "string into that cell and move to the next.");
        }

        /// <summary>
        /// Message to the user about Enter key movement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enterKeyMovementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("After a user inputs the formula, hitting the enter key or button will " +
                "automatically select the cell to the right of the current cell so that the user does not" +
                " have to manually change.  Hitting the enter key or button multiple times will move the " +
                "selected cell down the spreadsheet until the end of the row.  If the action occurs on the final " +
                "cell of the row, it will loop back to the first cell of the next row.");
        }
       
        /// <summary>
        /// Do Work that autosaves a spreadsheet for the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoSaver_DoWork(object sender, DoWorkEventArgs e)
        {
            if (savedBefore)
            {
                cont.Save(fileName);
            }
        }
        
        ///Fires when the autosaver is complete
        private void autoSaver_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (savedBefore)
                //Display to user that changes were saved.
                savedBox.Text = "Changes saved";
            enterButton.Enabled = true;
        }
       
        /// <summary>
        /// Message that describes the use of autosaver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autosaverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The autosave feature is enabled as soon as the spreadsheet " +
                "is saved for the first time.  Subsequent changes will be automatically saved " +
                "with a background worker. User will see a constant, \"changes saved\" at the " +
                "bottom of the screen.  Spreadsheet will close without confirmation because the " +
                "spreadsheet is already saved. Can be disabled or enabled by checkbox at top");
        }

        private void howDoIEnterAFormulaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Place an \"=\" before your formula. This designates your expression " +
                "into a formula.");
        }
    }
}
