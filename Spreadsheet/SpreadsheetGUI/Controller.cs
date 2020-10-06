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
        AbstractSpreadsheet ss;
        public Controller()
        {
            ss = new Spreadsheet(s => Regex.IsMatch(s, "^[A-Z][1-9][1-9]?$"), s => s.ToUpper(), "ps6");
        }

        /// <summary>
        /// Caller method to set cells
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="contents"></param>
        public void SetCellContents (int col, int row, string contents)
        {
            ss.SetContentsOfCell(DigitToVar(col, row), contents);
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
            return ss.GetCellValue(DigitToVar(col, row));
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
            return s.ToString();
        }
    }
}
