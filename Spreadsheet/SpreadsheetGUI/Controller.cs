using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            ss = new Spreadsheet(s => true, s => s.ToUpper(), "ps6");
        }

        /// <summary>
        /// Helper method to convert the numbers refering to cell location, col and row
        /// to a variable name that can be inputted in the spreadsheet.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private string DigitToVar (int col, int row)
        {
            StringBuilder s = new StringBuilder();
            char c = Convert.ToChar(col);
            s.Append(c);
            s.Append(row.ToString());
            return s.ToString();
        }
    }
}
