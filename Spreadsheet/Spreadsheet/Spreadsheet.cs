using SpreadsheetUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {   //YOU ARE IN THE MASTER BRANCH

        //ss is a represents the spreadsheet cells' names and contents
        Dictionary<string, Cell> ss;
        //dg is a dependency graph that will map dependencies of the cells
        private DependencyGraph dg;
        //Private bool for changed spreadsheet
        private bool changed;

        // ADDED FOR PS5
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get => this.changed; protected set => changed = value; }

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
            /// a double or a FormulaError, or a string
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
            public bool IsEmptyCell()
            {
                return empty;
            }
        }

        //Constructors
        /// <summary>
        /// Constructor of spreadsheet class. Instantiates ss (spreadsheet) and dg (dependency graph)
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            //Instantiate new ss and dg objects
            ss = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            //Set status of changed
            Changed = false;
        }
        /// <summary>
        /// Constructor of spreadsheet class. Gives the user a validator function, normalizer and version string arguments
        /// </summary>
        /// <param name="IsValid"></param> Validator function
        /// <param name="normalizer"></param> Normalizer Function
        /// <param name="version"></param> Spreadsheet version
        public Spreadsheet(Func<string, bool> IsValidFunc, Func<string, string> normalizer, string version) : base(IsValidFunc, normalizer, version)
        {
            //Quick Check to make sure none of the arguments are null
            if (IsValidFunc == null || normalizer == null || version == null)
                throw new ArgumentNullException();

            //Instantiate new ss and dg objects
            ss = new Dictionary<string, Cell>();
            dg = new DependencyGraph();

            //Set status of changed
            Changed = false;
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
            //Check that filePath is not null
            if (filePath == null)
                throw new SpreadsheetReadWriteException("File path cannot be null");
            if (File.Exists(filePath) && GetSavedVersion(filePath) != version)
                throw new SpreadsheetReadWriteException("Version names are not the same");
            //Quick Check to make sure none of the arguments are null
            if (IsValidFunc == null || normalizer == null || version == null)
                throw new ArgumentNullException();

            ss = new Dictionary<string, Cell>();
            dg = new DependencyGraph();

            //string that contains the cell name
            string name = "";
            //string to hold cell contents
            string content;

            //Create the spreadsheet from the saved file
            try
            {
                // Create an XmlReader inside this block, and automatically Dispose() it at the end.
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    //Read the element
                    while (reader.Read())
                    {
                        //If the element is a start element, move foward
                        if (reader.IsStartElement())
                        {
                            //Check the reader name
                            switch (reader.Name)
                            {
                                //case element is spreadsheet
                                case "spreadsheet":
                                    //if the spreadsheet has version attribute, retrieve the data
                                    if (reader.HasAttributes)
                                    {
                                        //Check if new spreadsheet is loading a previous one, if so, update version name                                        
                                        Version = reader.GetAttribute("version");
                                    }
                                    //Spreadsheet has no version data, so we throw
                                    else
                                        throw new SpreadsheetReadWriteException("Spreadsheet does not have a version name");
                                    break;
                                //Case where element is cell
                                case "cell":
                                    continue;
                                case "name":
                                    //Gets name of cell
                                    reader.Read();
                                    name = reader.Value;
                                    break;
                                case "contents":
                                    //If name hasn't been assigned by the time we get to content,
                                    //then the content is missing a cell name
                                    if (name != null && name != "")
                                    {
                                        reader.Read();
                                        //Checks if next sibling is content, otherwise throws exception
                                        content = reader.Value;
                                        //Sets contents of cell and checks for other error like cycles
                                        SetContentsOfCell(name, content);
                                        name = "";
                                    }
                                    //If the tags do not exist or contents does not come right after the name, throw exception
                                    else
                                        throw new SpreadsheetReadWriteException("Name of cell does not exist. Each content line must match with one name line" +
                                            "and the name must be within the cell elements");
                                    break;

                                //default is that the tag does not match what is needed for a spreadsheet class, so it throws.
                                default:
                                    throw new SpreadsheetReadWriteException("Contains tags that are not spreadsheet, cell, or content");
                            }
                        }
                        //If the element is not a start element, it must be an end element
                        else
                            continue;
                    }
                }
            }
            //If the file was not found
            catch (System.IO.FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Filepath could not be found");
            }
            //If the directory was not found
            catch (System.IO.DirectoryNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Directory could not be found");
            }
            catch (FormulaFormatException)
            {
                throw new SpreadsheetReadWriteException("File contained an invalid formula");
            }
            //If the read file contains circular dependency
            catch (CircularException)
            {
                throw new SpreadsheetReadWriteException("Circular dependency detected in file");
            }
            //Throws if there is mismatched tags in the file
            catch (System.Xml.XmlException)
            {
                throw new SpreadsheetReadWriteException("There were mismatched tags or nonXML content in the XML file");
            }

            Version = version;
            //Change status of Changed to false
            Changed = false;
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
                //Gets the string after the formula =
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
            //Recalculate the values of all the dependents
            RecalculateCells(number, allDependents);
            //Spreadsheet has changed, so Change is true;
            Changed = true;
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
            //Update value of dependent cells
            RecalculateCells(text, allDependents);
            //Change has occured, so Change is true
            Changed = true;
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
            //Holds original dependencies of graph, content, and value in case of circular exception
            //These values will be used to undo the changes made if circular dependency occurs
            List<string> originalDependees;
            Cell originalContent;
            Object originalValue;
            //If cell name is not in the spreadsheet, add name to spreadsheet
            if (!ss.ContainsKey(name))
            {
                //No original dependees, so new list
                originalDependees = new List<string>();
                //No original content, so cell is empty
                originalContent = new Cell(name, "");
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
                //Update values of dependent cells
                RecalculateCells(ss[name].CellValue, dependents);
                //Change has occured, so Changed is true
                Changed = true;
                //return the list.
                return dependents;
            }
            catch (CircularException)
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

        // ADDED FOR PS5
        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            //Create the spreadsheet from the saved file
            try
            {
                // Create an XmlReader inside this block, and automatically Dispose() it at the end.
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    //Read the element
                    while (reader.Read())
                    {
                        //If the element is a start element, move foward
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == "spreadsheet")
                                if (reader.HasAttributes)
                                {
                                    //Check if new spreadsheet is loading a previous one, if so, update version name                                        
                                    return reader.GetAttribute("version");
                                }
                                //Spreadsheet has no version data, so we throw
                                else
                                    throw new SpreadsheetReadWriteException("Spreadsheet does not have a version name");
                        }
                    }

                }
                throw new SpreadsheetReadWriteException("Does not have a version name");
            }
            //If the file does not have elements
            catch(System.Xml.XmlException)
            {
                throw new SpreadsheetReadWriteException("File does not contain proper XML Format");
            }
            //If the file was not found
            catch (System.IO.FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Filepath could not be found");
            }
            //If the directory was not found
            catch (System.IO.DirectoryNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Directory could not be found");
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            //Sets up the XML writer settings to indent and character to use when indenting
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "  ";

            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename, setting))
                {
                    //Begin document
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);
                    //For each of the cells in the spreadsheet
                    foreach (KeyValuePair<string, Cell> cells in ss)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", cells.Key);
                        //Gets the Cell from the KeyValuePair and if it's a formula, convert to string
                        if (cells.Value.GetFormulaContent is Formula)
                        {
                            //Get the formula
                            String s = cells.Value.GetFormulaContent.ToString();
                            //Add an equals to the front
                            s = s.Insert(0, "=");
                            //Write it to XML
                            writer.WriteElementString("contents", s);
                        }
                        //If the value of a cell is a double, and we know the content is not a formula by 
                        //if statement above, we can conclude the content is a double and write it
                        else if (cells.Value.CellValue is double)
                            writer.WriteElementString("contents", cells.Value.GetFormulaContent.ToString());
                        //The content must be a string
                        else
                            writer.WriteElementString("contents", (string)cells.Value.GetFormulaContent);

                        //closes cell
                        writer.WriteEndElement();
                    }

                    //Ends spreadsheet node
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (Exception)
            {

                throw new SpreadsheetReadWriteException("Filepath could not be found");
            }
            //File is saved, so now Changed is false
            Changed = false;
        }

        // ADDED FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            //If the name of the variable does not exist or is not a valid variable, throw exception
            if (name is null || !IsValid(Normalize(name)) || !IsValidVariable(Normalize(name)))
                throw new InvalidNameException();
            //Else if the spreadsheet does not contain the cell name => its an empty cell, return empty string
            else if (!ss.ContainsKey(Normalize(name)))
                return "";
            //Else return the contents of the cell
            else
                return ss[Normalize(name)].CellValue;

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
        /// This is a helper method that will recalculate all dependent cells values 
        /// based on dependees new value
        /// </summary>
        /// <param name="value"></param> Dependee's value
        /// <param name="dpendents"></param> All the dependents that need to update
        private void RecalculateCells(object value, List<string> dpendents)
        {
            //For each dependent in the list
            foreach (string dependentCell in dpendents)
            {
                //If the cell value is a string, it doesn't change so continue
                if (ss[dependentCell].CellValue is string)
                    continue;
                //If the value of a cell comes from a formula, update the value
                if (GetCellContents(dependentCell) is Formula)
                {
                    Formula f = (Formula)GetCellContents(dependentCell);
                    ss[dependentCell].CellValue = f.Evaluate(CellLookup);
                }
                //Cells whose contents are doubles will not change since they have no dependees
            }

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
        /// Helper method to tell whether a cell name follows basic variable convention
        /// One or more letters followed by one or more numbers
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsValidVariable(string name)
        {
            //Set pattern
            string basicVarPattern = "^[a-zA-Z]+[0-9]+$";
            Regex varPattern = new Regex(basicVarPattern);
            return varPattern.IsMatch(name);
        }
    }

}
