using SpreadsheetUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        //ss is a represents the spreadsheet cells' names and contents
        Dictionary<string, Cell> ss;
        //dg is a dependency graph that will map dependencies of the cells
        private DependencyGraph dg;
        /// <summary>
        /// This is a helper class cell to be used in the Spreadsheet class.  
        /// The Cell's purpose is to store the formula and hold data to perform the operations.
        /// </summary>
        private class Cell
        {
            //The cell name
            private string name;
            //The formula of the cell stored as a Formula object, string, or double
            private object formulaContent;
            //The value of the formula. Can be double or FormulaError object
            private object value;

            /// <summary>
            /// Constructor for Cell class with Formula content
            /// </summary>
            /// <param name="name"></param>
            /// <param name="formula"></param>
            public Cell(string name, Formula formula)
            {
                this.name = name;
                formulaContent = formula;
            }
            /// <summary>
            /// Constructor for Cell class with double content
            /// </summary>
            /// <param name="name"></param>
            /// <param name="formula"></param>
            public Cell(string name, double formula)
            {
                this.name = name;
                formulaContent = formula;
            }
            /// <summary>
            /// Constructor for Cell class with string content
            /// </summary>
            /// <param name="name"></param>
            /// <param name="formula"></param>
            public Cell(string name, string formula)
            {
                this.name = name;
                formulaContent = formula;
            }

            /// <summary>
            /// Getter method to retrieve Formula Object
            /// </summary>
            public object GetFormulaContent
            {
                get => formulaContent;
            }
        }

        /// <summary>
        /// Empty Constructor of spreadsheet class
        /// </summary>
        public Spreadsheet()
        {
            ss = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            //Checks if the cell name is a valid variable or is null
            //If so, throws InvalidNameException
            if (!IsValid(name) || name == null)
                throw new InvalidNameException();
            //if the cell does not exist, return an empty string
            if (!(ss.ContainsKey(name)))
                return "";
            //Return the content of the cell
            else
                return ss[name].GetFormulaContent;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //Check if spreadsheet is empty
            if (ss.Count == 0)
                return new List<string>();
            List<string> cells = new List<string>();
            //For each cell, add to the list and return
            foreach (string cellNames in ss.Keys)
            {
                cells.Add(cellNames);
            }
            return cells;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetCellContents(string name, double number)
        {
            if (!(IsValid(name)) || name == null)
                throw new InvalidNameException();
            //If cell name is not in the spreadsheet
            if (!ss.ContainsKey(name))
                ss.Add(name, new Cell(name, number));
            //Else get the cell, update its contents and print the list
            else
                ss[name] = new Cell(name, number);
            List<string> dependents = dg.GetDependents(name).ToList();
           //Name no longer depends on cells because its content is a double, so remove
            foreach (string dependee in dg.GetDependees(name))
                dg.RemoveDependency(dependee, name);
            //Insert name of cell to beginning of dependent list
            dependents.Insert(0, name);
            return dependents;

        }
        
        public override IList<string> SetCellContents(string name, string text)
        {
            if (!(IsValid(name)) || name == null)
                throw new InvalidNameException();
            //If cell name is not in the spreadsheet and add to list
            if (!ss.ContainsKey(name))
                ss.Add(name, new Cell(name, text));

            //Else get the cell, update its contents and print the list
            else
                ss[name] = new Cell(name, text);
            //Get the variables from the text
            List<string> dependents =




        }

        public override IList<string> SetCellContents(string name, Formula formula)
        {
            //Check if name is null or valid otherwise throw exception
            if (!(IsValid(name)) || name == null)
                throw new InvalidNameException();
            //If cell name is not in the spreadsheet, add name to spreadsheet
            if (!ss.ContainsKey(name))
                ss.Add(name, new Cell(name, formula));

            //Else get the cell, update its contents and print the list
            else
                ss[name] = new Cell(name, formula);

            //Take out all the variables in the contents
            List<string> dependents = formula.GetVariables().ToList();
            //Build the dependency graph with the cell name
            foreach (string depen in dependents)
                dg.ReplaceDependents(name, dependents);
            //Add Cell name to the beginning of the List as per method contract
            dependents.Insert(0, name);
            return dependents;
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Helper method to tell whether a cell name is valid or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsValid(string name)
        {
            string basicVarPattern = "^[a-zA-Z_][0-9a-zA-Z_]+$";
            Regex varPattern = new Regex(basicVarPattern);
            if (varPattern.IsMatch(name))
                return true;
            return false;
        }

        /// <summary>
        /// Helper method to pick out variables in a cell with 
        /// string context
        /// </summary>
        /// <param name="formulatext"></param>
        private IList<string> GetCellDependencies(string formulatext)
        {

            foreach (string token in formulatext)
            {

            }

        }

    }

}
