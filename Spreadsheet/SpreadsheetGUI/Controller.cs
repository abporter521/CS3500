using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
    /// <summary>
    /// This is a class that will act as a controller between the UI of PS6
    /// and the spreadsheet that is behind it.
    /// </summary>
    class Controller
    {
        //Holds the spreadsheet logic
        AbstractSpreadsheet ss;
        
        /// <summary>
        /// This is the basic constructor for the controller.  It sets up a spreadsheet where the validator
        /// is a variable name with 1 letter followed by 1 or 2 numbers.  The normalizer puts all the variables
        /// to uppercase for easier computations.  Version name is ps6 as per instruction
        /// </summary>
        public Controller()
        {
            ss = new Spreadsheet(s => Regex.IsMatch(s, "^[A-Z][1-9][0-9]?$"), s => s.ToUpper(), "ps6");
        }
        /// <summary>
        /// Second constructor in order to open and replace the existing spreadsheet.
        /// Other arguments are same as the default constructor
        /// </summary>
        /// <param name="filename"></param>
        public Controller(string filename)
        {
            ss = new Spreadsheet(filename, s => Regex.IsMatch(s, "^[A-Z][1-9][0-9]?$"), s => s.ToUpper(), "ps6");
        }

        /// <summary>
        /// Gets the changed boolean from spreadsheet
        /// </summary>
        /// <returns></returns>
        public bool GetChanged()
        {
            return ss.Changed;
        }
        /// <summary>
        /// Caller method to set cells
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="contents"></param>
        public List<string> SetCellContents (int col, int row, string contents)
        {
            return ss.SetContentsOfCell(DigitToVar(col, row), contents).ToList();
        }

        /// <summary>
        /// Gets the cell contents.  Cell contents are either a formula, double or string
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public object GetCellContents (int col, int row)
        {
           return ss.GetCellContents(DigitToVar(col, row));
        }

        /// <summary>
        /// Gets the cell value
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public object GetCellValue (int col, int row)
        {
            FormulaError fe = new FormulaError();
            if (ss.GetCellValue(DigitToVar(col, row)) is FormulaError)
            {
                fe = (FormulaError)ss.GetCellValue(DigitToVar(col,row));
                return fe;
            }
            else
                return ss.GetCellValue(DigitToVar(col, row));
        }

        /// <summary>
        /// Calls the spreadsheet to save
        /// </summary>
        /// <param name="filename"></param>
        public void Save (string filename)
        {
            ss.Save(filename);
        }

        /// <summary>
        /// Returns the coordinates of all the nonempty cells in a spreadsheet
        /// </summary>
        /// <returns></returns>
        public HashSet<Tuple<int,int>> GetNonEmptyCells()
        {
            HashSet<Tuple<int, int>> coordinates = new HashSet<Tuple<int, int>>();    
            LinkedList<string> cellNames = (LinkedList<string>) ss.GetNamesOfAllNonemptyCells();
            foreach(string cell in cellNames)
            {
                VarToDigit(cell, out int col, out int row);
                coordinates.Add(new Tuple<int, int> (col, row ));
            }
            return coordinates;
        }

        /// <summary>
        /// Helper method to convert the numbers refering to cell location, col and row
        /// to a variable name that can be inputted in the spreadsheet.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public string DigitToVar (int col, int row)
        {
            StringBuilder s = new StringBuilder();
            //change the col variable to letter
            char c = Convert.ToChar(col+65);
            s.Append(c);
            //Add to the string the row number
            s.Append(row+1);
            //Console.WriteLine(s.ToString());
            return s.ToString();
        }

        
        /// <summary>
        /// Helper method to convert the variables to a cell location, col and row
        /// to a variable name that can be inputted in the spreadsheet.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public void VarToDigit(string variable, out int col, out int row)
        {
            col = variable.ElementAt(0)-65;
            row = Int32.Parse(variable.Substring(1)) -1;
            //Console.WriteLine(col + ", " + row);
        }
    }
}
