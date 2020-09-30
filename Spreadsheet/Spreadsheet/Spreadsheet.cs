﻿using SpreadsheetUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {   //YOU ARE IN THE PS5 BRANCH

        //ss is a represents the spreadsheet cells' names and contents
        Dictionary<string, Cell> ss;
        //dg is a dependency graph that will map dependencies of the cells
        private DependencyGraph dg;
        //Is the string containing the file path inputted by user in constructor
        private string filePath;
        //Storing my delgates        

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
            //Helper variable to see if cell is empty 
            private bool empty;

            /// <summary>
            /// Constructor for Cell class with Formula content
            /// </summary>
            /// <param name="name"></param>
            /// <param name="formula"></param>
            public Cell(string name, Formula formula)
            {
                this.name = name;
                formulaContent = formula;
                empty = false;
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
                empty = false;
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
                if (formula == "")
                    empty = true;
                else empty = false;
            }

            /// <summary>
            /// Getter method to retrieve Formula Object
            /// </summary>
            public object GetFormulaContent
            {
                get => formulaContent;
            }
            /// <summary>
            /// Getter/Setter method for a cell's value.  Cell value can be
            /// a double or a FormulaError
            /// </summary>
            public object CellValue
            {
                get => value;
                set
                {
                    this.value = value;
                }
            }
            
            /// <summary>
            /// Getter method to return the empty bool
            /// </summary>
            /// <returns></returns>
            public bool IsEmptyCell ()
            {
                return empty;
            }
        }

        //Constructors
        /// <summary>
        /// Constructor of spreadsheet class. Instantiates ss (spreadsheet) and dg (dependency graph)
        /// </summary>
        public Spreadsheet():base(s=>true, s=>s, "default")
        {
            ss = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            IsValid = s => true;
            Normalize = s => s;
            Version = "default";
        }
        /// <summary>
        /// Constructor of spreadsheet class. Gives the user a validator function, normalizer and version string arguments
        /// </summary>
        /// <param name="IsValid"></param> Validator function
        /// <param name="normalizer"></param> Normalizer Function
        /// <param name="version"></param> Spreadsheet version
        public Spreadsheet(Func<string, bool> IsValidFunc, Func<string, string> normalizer, string version):base(IsValidFunc, normalizer, version)
        {
            ss = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            IsValid = IsValidFunc;
            Normalize = normalizer;
            Version = version;
        }
        /// <summary>
        /// Constructor of spreadsheet class. Gives the user a filepath, validator function, normalizer and version string arguments
        /// </summary>
        /// <param name="IsValid"></param> Validator function
        /// <param name="normalizer"></param> Normalizer Function
        /// <param name="version"></param> Spreadsheet version
        public Spreadsheet(string filePath, Func<string, bool> IsValidFunc, Func<string, string> normalizer, string version)
            : base(IsValidFunc, normalizer, version)
        {
            ss = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            this.filePath = filePath;
            IsValid = IsValidFunc;
            Normalize = normalizer;
            Version = version;
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
            if (name == null || !IsValidVariable(Normalize(name)) || !IsValid(Normalize(name)))
                throw new InvalidNameException();
            else
            {
                //if the cell does not exist, return an empty string
                if (!ss.ContainsKey(Normalize(name)))
                    return "";
                //Check if the cell is empty
                else if (ss[Normalize(name)].IsEmptyCell())
                    return "";
                //Return the content of the cell
                else
                    return ss[Normalize(name)].GetFormulaContent;
            }
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// Assumes all variables in the spreadsheet are normalized
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //Check if spreadsheet is empty
            if (ss.Count == 0)
                return new List<string>();
            //List of all the non-empty cells
            LinkedList<string> cells = new LinkedList<string>();
            //For each cell, add to the list and return
            foreach (string cellNames in ss.Keys)
            {
                //If the cell contains an empty string, do not add to list
                if (ss[cellNames].IsEmptyCell())
                    continue;
                //Add to the list of non-empty cells
                else
                    cells.AddFirst(cellNames);
            }
            return cells;
        }

        public override bool Changed { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        // ADDED FOR PS5
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            //If content is null, throw exception
            if (content is null)
                throw new ArgumentNullException();
            //Check that name is valid variable and is not null
            if (name is null || !IsValid(Normalize(name)) || !IsValidVariable(Normalize(name)))
                throw new InvalidNameException();
            
            //Check if contents are double, if so, call double version of SetCellContents
            if (Double.TryParse(content, out double number))
                return SetCellContents(Normalize(name), number);
            //If content is a formula, designated with "=" at beginning, then try to add formula to spreadsheet
            if (content.StartsWith("="))
            {
                //Gets the string after the formula
                string possibleFormula = content.Substring(1);
                //Return the formula version of SetCellContents
                return SetCellContents(Normalize(name), new Formula(possibleFormula, Normalize, IsValid));
            }
            //Content must be of string type, run string version of SetCellContents
            else
                return SetCellContents(Normalize(name), content);
        }

        /// <summary>
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            //If cell name is not in the spreadsheet
            if (!ss.ContainsKey(name))
                ss.Add(name, new Cell(name, number));
           
            //Else get the cell, update its contents and print the list
            else
                ss[name] = new Cell(name, number);
           
            //Store value of cell as number
            ss[name].CellValue = number;
            //Name no longer depends on cells because its content is a double, so remove
            foreach (string dependee in dg.GetDependees(name).ToList())
                dg.RemoveDependency(dependee, name);
            //Use GetCellsToRecalculate to return list
            List<string> allDependents = GetCellsToRecalculate(name).ToList();
            
            return allDependents;
        }

        /// <summary>
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            //If cell name is not in the spreadsheet
            if (!ss.ContainsKey(name))
                ss.Add(name, new Cell(name, text));
            //Else get the cell, update its contents
            else
                ss[name] = new Cell(name, text);
            //Plug in value of string to cell
            ss[name].CellValue = text;
            //Name no longer depends on cells because its content is a string, so remove
            foreach (string dependee in dg.GetDependees(name).ToList())
                dg.RemoveDependency(dependee, name);
            //Use GetCellsToRecalculate to return list
            List<string> allDependents = GetCellsToRecalculate(name).ToList();
            //Return the dependents of the cell
            return allDependents;
        }
        /// <summary>
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>

        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            //Holds original dependencies of graph and content in case of circular exception
            List<string> originalDependees;
            Cell originalContent;
            Object originalValue;
            //If cell name is not in the spreadsheet, add name to spreadsheet
            if (!ss.ContainsKey(name))
            {
                //No original dependees, so new list
                originalDependees = new List<string>();
                //No original content, so cell is empty
                originalContent = new Cell(name,"");
                //No original content, so value is empty
                originalValue = "";
                //Add Cell to list
                ss.Add(name, new Cell(name, formula));
            }
            //Else get the cell, update its contents
            else
            {
                //Stores the original dependency list
                originalDependees = dg.GetDependees(name).ToList();
                //stores the original cell
                originalContent = ss[name];
                //stores original value of cell
                originalValue = ss[name].CellValue;
                //Set new formula as cell content
                ss[name] = new Cell(name, formula);                
            }
            //Take out all the variables in the contents
            IList<string> dependees = formula.GetVariables().ToList();
            //Build the dependency graph with the new dependencies of the formula
            dg.ReplaceDependees(name, dependees);
            //Use GetCellsToRecalculate to return list or detect CircularException
            try
            {
                //Detect Circular dependency first, this gets rid of bug if cell calls itself
                List<string> dependents = GetCellsToRecalculate(name).ToList();
                //Set new cell value
                ss[name].CellValue = formula.Evaluate(CellLookup);
                //return the list.
                return dependents; 
            }
            catch(CircularException)
            {
                //Revert to original dependency graph
                dg.ReplaceDependees(name, originalDependees);
                //Revert cell to original content
                ss[name] = originalContent;
                //Revert cell value to original value
                ss[name].CellValue = originalValue;
                //throw exception
                throw new CircularException();
            }
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            //Check if dependent graph contains the name of the cell
            if (dg.HasDependents(name))
                return dg.GetDependents(name);
            //Name does not have dependents so return empty list
            return new List<string>();
        }

        /// <summary>
        /// Requires that names be non-null.  Also requires that if names contains s,
        /// then s must be a valid non-null cell name.
        /// 
        /// If any of the named cells are involved in a circular dependency,
        /// throws a CircularException.
        /// 
        /// Otherwise, returns an enumeration of the names of all cells whose values must
        /// be recalculated, assuming that the contents of each cell named in names has changed.
        /// The names are enumerated in the order in which the calculations should be done.  
        /// 
        /// For example, suppose that 
        /// A1 contains 5
        /// B1 contains 7
        /// C1 contains the formula A1 + B1
        /// D1 contains the formula A1 * C1
        /// E1 contains 15
        /// 
        /// If A1 and B1 have changed, then A1, B1, and C1, and D1 must be recalculated,
        /// and they must be recalculated in either the order A1,B1,C1,D1 or B1,A1,C1,D1.
        /// The method will produce one of those enumerations.
        /// 
        /// PLEASE NOTE THAT THIS METHOD DEPENDS ON THE ABSTRACT METHOD GetDirectDependents.
        /// IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
        /// </summary>
        protected IEnumerable<String> GetCellsToRecalculate(ISet<String> names)
        {
            //Data Types to keep track of changed cells
            LinkedList<String> changed = new LinkedList<String>();
            HashSet<String> visited = new HashSet<String>();
            foreach (String name in names)
            {
                if (!visited.Contains(name))
                {
                    Visit(name, name, visited, changed);
                }
            }
            return changed;
        }


        /// <summary>
        /// A convenience method for invoking the other version of GetCellsToRecalculate
        /// with a singleton set of names.  See the other version for details.
        /// </summary>
        protected IEnumerable<String> GetCellsToRecalculate(String name)
        {
            return GetCellsToRecalculate(new HashSet<String>() { name });
        }


        /// <summary>
        /// A helper for the GetCellsToRecalculate method.
        /// 
        /// </summary>
        private void Visit(String start, String name, ISet<String> visited, LinkedList<String> changed)
        {
            //Adds the cell name to the list, signifying that it has been visited.
            visited.Add(name);
            //Runs a depth-first search on the current cell
            foreach (String n in GetDirectDependents(name))
            {
                //If the cell encountered is the same as the starting point, a cycle is detected and throws
                if (n.Equals(start))
                {
                    throw new CircularException();
                }
                //If the cell has not been visited, recursively run this method on the cell
                else if (!visited.Contains(n))
                {
                    Visit(start, n, visited, changed);
                }
            }
            //Add cell to the beginning of the changed list
            changed.AddFirst(name);
        }
        /// <summary>
        /// Lookup method used in evaluating formula objects.
        /// This method will check if the variable exists withing the spreadsheet.
        /// If the variable does not exist, it is undefined and will throw
        /// and ArgumentException.
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns></returns>
        private double CellLookup(string cellName)
        {
            //If the cell does not exist in our spreadsheet (because empty), throw error
            if (!ss.ContainsKey(cellName))
                throw new ArgumentException("Dependent Cell does not have value");
            //If the value of a cell is a FormulaError, we need to throw ArgumentException
            if (ss[cellName].CellValue is FormulaError)
                throw new ArgumentException("Dependent Cell has a FormulaError value");
           //If the cell is empty or value is a string, return ArgumentException
            if (ss[cellName].IsEmptyCell() || ss[cellName].CellValue is string)
                throw new ArgumentException("Dependent Cell does not have value");
            //Return the double value associated with cell
            else
                return (double)ss[cellName].CellValue;
        }
        /// <summary>
        /// Helper method to tell whether a cell name is valid or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsValidVariable(string name)
        {
            string basicVarPattern = "^[a-zA-Z][0-9]+$";
            Regex varPattern = new Regex(basicVarPattern);
            return varPattern.IsMatch(name);
        }

        public override string GetSavedVersion(string filename)
        {
            throw new NotImplementedException();
        }

        public override void Save(string filename)
        {
            throw new NotImplementedException();
        }

        public override object GetCellValue(string name)
        {
            throw new NotImplementedException();
        }
    }

}
