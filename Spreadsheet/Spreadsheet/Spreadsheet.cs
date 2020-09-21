using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SS
{
    class Spreadsheet : AbstractSpreadsheet
    {
        Dictionary<string, Cell> ss;
        /// <summary>
        /// This is a helper class cell to be used in the Spreadsheet class.  
        /// The Cell's purpose is to store the formula and hold data to perform the operations.
        /// </summary>
        private class Cell
        {
            //The cell name
            private string name;
            //The formula of the cell stored as a Formula object
            private Formula formula;
            //The value of the formula. Can be double or FormulaError object
            private object value;

            /// <summary>
            /// Constructor for Cell class
            /// </summary>
            /// <param name="name"></param>
            /// <param name="formula"></param>
            public Cell(string name, string formula)
            {
                this.name = name;
                this.formula = new Formula(formula);
            }            

            /// <summary>
            /// Getter method to retrieve Formula Object
            /// </summary>
            public Formula GetFormula
            {
                get => formula;
            }
        }

        /// <summary>
        /// Empty Constructor of spreadsheet class
        /// </summary>
        public Spreadsheet()
        {
            ss = new Dictionary<string, Cell>();
        }
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            //Checks if the cell name is a valid variable or is null
            //If so, throws NotImplementedException
            if(!IsValid(name) || name == null)
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

        private bool IsValid(string name)
        {
            string basicVarPattern = "^[a-zA-Z_][0-9a-zA-Z_]+$";
            Regex varPattern = new Regex(basicVarPattern);
            if (varPattern.IsMatch(name))
                return true;
            return false;
        }

    }

}
