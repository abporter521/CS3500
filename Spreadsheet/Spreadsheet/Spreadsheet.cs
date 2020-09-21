using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{
    class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// This is a helper class cell to be used in the Spreadsheet class.  
        /// The Cell's purpose is to store the formula and hold data to perform the operations.
        /// </summary>
        private class Cell
        {
            private string name;
            private Formula formula;
            private object value;
            public Cell(string name, string formula)
            {
                this.name = name;
                this.formula = new Formula(formula);
            }
        }
        /// <summary>
        /// Empty Constructor of spreadsheet class
        /// </summary>
        public Spreadsheet()
        {
        }
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            throw new NotImplementedException();
        }

        public override IList<string> SetCellContents(string name, double number)
        {
            throw new NotImplementedException();
        }

        public override IList<string> SetCellContents(string name, string text)
        {
            throw new NotImplementedException();
        }

        public override IList<string> SetCellContents(string name, Formula formula)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            throw new NotImplementedException();
        }
    }

}
