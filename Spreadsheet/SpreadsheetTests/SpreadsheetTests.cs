using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ss
{
    /// <summary>
    /// This is my test class for PS5
    /// Code coverage is 98.44% for my spreadsheet class
    /// Code Coverage also accounts for AbstractSpreadsheet 
    /// of which I have no way of accessing the getCellstoRecalculate method
    /// 
    /// Andrew Porter 29 Sept 20
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// Tests return is empty string for valid cell with nothing
        /// </summary>
        [TestMethod]
        public void GetCellContents()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A4"));
        }
        /// <summary>
        /// Tests return is double for valid cell
        /// </summary>
        [TestMethod]
        public void GetCellContentsValidDouble()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "32.0");
            Assert.AreEqual(32.0, s.GetCellContents("A4"));
        }
        /// <summary>
        /// Tests return throws for null arguments
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConstructorArguments()
        {
            AbstractSpreadsheet s = new Spreadsheet(null, s=>s, "default");
        }
        /// <summary>
        /// Tests return throws for null arguments
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConstructorArguments1()
        {
            AbstractSpreadsheet s = new Spreadsheet("test.xml", null, s => s, "default");
        }
        /// <summary>
        /// Tests return throws for null arguments
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void NullConstructorArguments2()
        {
            AbstractSpreadsheet s = new Spreadsheet(null, s=>true, s => s, "default");
        }
        /// <summary>
        /// Tests return is FormulaError object for valid cell 
        /// </summary>
        [TestMethod]
        public void GetCellContentsValidString()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "c7+ 2k");
            Assert.AreEqual("c7+ 2k", s.GetCellContents("A4"));
        }
        /// <summary>
        /// Tests if the method accepts cell name with whitespace
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void WhitespaceInCellName()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A5 ", "1+1");
        }
        /// <summary>
        /// Tests cell dependency is correctly created
        /// </summary>
        [TestMethod]
        public void ChangeDependencyDouble()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("A4", "=B4+7").Count());
            Assert.AreEqual(2, s.SetContentsOfCell("B4", "=C4 + 2").Count());
            Assert.AreEqual(3, s.SetContentsOfCell("C4", "=8+2").Count());
            s.SetContentsOfCell("B4", "3");
            Assert.AreEqual(1, s.SetContentsOfCell("C4", "=8+2").Count());
        }

        /// <summary>
        /// Tests cell dependency is correctly created
        /// </summary>
        [TestMethod]
        public void ChangeDependencyString()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("A4", "=B4+7").Count());
            Assert.AreEqual(2, s.SetContentsOfCell("B4", "=C4 + 2").Count());
            Assert.AreEqual(3, s.SetContentsOfCell("C4", "=8+2").Count());
            s.SetContentsOfCell("B4", "work please");
            Assert.AreEqual(1, s.SetContentsOfCell("C4", "=8+2").Count());
        }
        /// <summary>
        /// Tests if the method throws exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void DoubleInvalidName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "4");
        }

        /// <summary>
        /// Tests that return is empty list for new spreadsheet or spreadsheet
        /// with only empty cells
        /// </summary>
        [TestMethod]
        public void EmptyCellNewSpreadsheet()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual(0, s.GetNamesOfAllNonemptyCells().Count());
            s.SetContentsOfCell("A2", "");
            s.SetContentsOfCell("B2", "  ");
            s.SetContentsOfCell("C4", "");
            s.SetContentsOfCell("D4", "      ");
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
            s.SetContentsOfCell("C4", "Hope this Works");
            Assert.AreEqual(3, s.GetNamesOfAllNonemptyCells().Count());
        }
        /// <summary>
        /// Get the cell content of a cell that had something,
        /// then change to empty to be sure it returns empty
        /// </summary>
        [TestMethod]
        public void ChangeAndGetCellContents()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "32.0");
            Assert.AreEqual(32.0, s.GetCellContents("A4"));
            s.SetContentsOfCell("A4", " ");
            Assert.AreEqual(" ", s.GetCellContents("A4"));
        }
        /// <summary>
        /// Tests return is formula for valid cell 
        /// </summary>
        [TestMethod]
        public void GetCellContentsValidFormula()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "=B4+7");
            Assert.AreEqual(new Formula("B4+7"), s.GetCellContents("A4"));
        }
        /// <summary>
        /// Tests the 4 arugment constructor and if the xml file has no version name
        /// ErrorTest6 has no version name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void NoVersionName()
        {
            Spreadsheet s = new Spreadsheet("errortest6.xml", s=> true, s=>s, " " );

        }

        /// <summary>
        /// Tests the 4 arugment constructor and if the xml file has no version name
        /// ErrorTest7 has cell named outside the spreadsheet
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void MisplacedCell()
        {
            Spreadsheet s = new Spreadsheet("errortest7.xml", s => true, s => s, "version2");

        }
        /// <summary>
        /// Tests the 4 arugment constructor and if the xml file has no version name
        /// ErrorTest8 has cell named after the contents
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void MisplacedNamedCell()
        {
            Spreadsheet s = new Spreadsheet("errortest8.xml", s => true, s => s.ToLower(), "version2");

        }
        /// <summary>
        /// Tests the 4 arugment constructor and if the xml file has no version name
        /// ErrorTest10 has invalid formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void FileHasInvalidFormula()
        {
            Spreadsheet s = new Spreadsheet("errortest10.xml", s => true, s => s.ToLower(), "version2");
        }
        /// <summary>
        /// Tests the 4 arugment constructor and if the xml file has no version name
        /// ErrorTest50 does not exist
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void FileNonExistant()
        {
            Spreadsheet s = new Spreadsheet("errortest50.xml", s => true, s => s.ToLower(), "version2");
        }
        /// <summary>
        /// Tests return is CircularException for cell with circular dependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void GetCellContentsFormulaCircular()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "=A4+7");
        }
        /// <summary>
        /// Tests return is CircularException for spreadsheet originally not cyclical
        /// the contents are modified and a cycle is made
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void GetCellContentsFormulaCreateCircularException()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "=B4+7");
            s.SetContentsOfCell("B4", "=C4 -1");
            s.SetContentsOfCell("C4", "=8+2");
            Assert.AreEqual(3.0, s.GetNamesOfAllNonemptyCells().Count());
            s.SetContentsOfCell("B4", "=A4/2");
        }

        /// <summary>
        /// Tests return is CircularException for GetSavedVersion
        /// Errortest4 has a cell that is initialized and then renamed and causes a circular dependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLCauseCircularException()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetSavedVersion("errortest4.xml");
        }
        /// <summary>
        /// Tests if a cells values are changed when a new cell gets instantiated.
        /// 
        /// For Example cells A4 = B4 + 7 and when  the cell is set,
        /// with store a FormulaError object because B4 does not have value.
        /// When B4 is instantiated with 10, A4's value should update to 17
        /// </summary>
        [TestMethod]
        public void CellValueUpdatesTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "=B4+7");
            Assert.IsTrue(s.GetCellValue("A4") is FormulaError);
            s.SetContentsOfCell("B4", "5");
            Assert.AreEqual(12.0, s.GetCellValue("A4"));
        }

        /// <summary>
        /// Tests the save method of spreadsheet
        /// </summary>
        [TestMethod]
        public void BasicSaveTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "=B4+7");
            s.SetContentsOfCell("B4", "5");
            s.Save("test.xml");
        }

        /// <summary>
        /// Tests the save method of spreadsheet and if the method  throws
        /// when given a file to read with a different version name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionVersionTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "=B4+7");
            s.SetContentsOfCell("B4", "5");
            s.Save("test.xml");
            s.GetSavedVersion("errortest5.xml");
        }
        /// <summary>
        /// Tests the SavedVersion method of spreadsheet
        /// </summary>
        [TestMethod]
        public void BasicSavedVersionTest()
        {
            AbstractSpreadsheet s = new Spreadsheet(s=>true, s=>s.ToUpper(),"testerVersion");
            Assert.IsFalse(s.Changed);
            s.SetContentsOfCell("a4", "=B4+7");
            Assert.IsTrue(s.Changed);
            s.SetContentsOfCell("B4", "5");
            Assert.IsTrue(s.Changed);
            s.SetContentsOfCell("d3", "hello World");
            Assert.IsTrue(s.Changed);
            s.Save("test.xml");
            Assert.IsFalse(s.Changed);
            Assert.AreEqual("testerVersion",s.GetSavedVersion("test.xml"));

        }
        /// <summary>
        /// Tests for exception with faulty xml file.  
        /// ErrorTest1 has error of element tags that don't exist.
        /// Example: <Error>test</Error>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void BasicSavedVersionErrorTest()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("testerVersion", s.GetSavedVersion("errortest1.xml"));
        }

        /// <summary>
        /// Tests for exception with faulty xml file.  
        /// ErrorTest2 has missing end element tags
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void BasicSavedVersionError2Test()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("testerVersion", s.GetSavedVersion("errortest2.xml"));
        }
        /// <summary>
        /// Tests for exception with faulty xml file.  
        /// ErrorTest3 has the name of the cell outside the cell block
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void BasicSavedVersionError3Test()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("testerVersion", s.GetSavedVersion("errortest3.xml"));
        }

        /// <summary>
        /// Tests return is CircularException for cell with circular dependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void GetCellContentsFormulaCircularThreeElements()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "=B4+7");
            s.SetContentsOfCell("B4", "=C4 -1");
            s.SetContentsOfCell("C4", "=A4+2");
        }
        /// <summary>
        /// Given a valid dependency graph, if I change one in the middle,
        /// I want to assert that changes are made throughout
        /// </summary>
        [TestMethod]
        public void DependencyChange()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "=B4+7");
            s.SetContentsOfCell("B4", "=C4 -1");
            s.SetContentsOfCell("C4", "=8+2");
            Assert.AreEqual(3.0, s.GetNamesOfAllNonemptyCells().Count());
            s.SetContentsOfCell("B4", "=8/2");
            Assert.AreEqual(3.0, s.GetNamesOfAllNonemptyCells().Count());

        }
        /// <summary>
        /// Check that cell contents return empty
        /// </summary>
        [TestMethod]
        public void ContentChangetoEmpty()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "=B4+7");
            s.SetContentsOfCell("A4", "");
            Assert.AreEqual("", s.GetCellContents("A4"));

        }
        /// <summary>
        /// Check that cell contents return an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContentNull()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "=B4+7");
            s.SetContentsOfCell("A4", null);

        }
        /// <summary>
        /// Tests return is throw exception for invalid name 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void ExceptionNullName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=A4+7");
        }
        /// <summary>
        /// Test was given in lecture. Tests if invalid file path threw exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveThrowTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("/some/nonsense/path.xml");   // you would expect this line to throw
        }
        /// <summary>
        /// Tests if invalid file path threw exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SavedVersionThrowTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("/some/nonsense/path.xml");   // you would expect this line to throw
        }
        /// <summary>
        /// Tests return is exception for null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void ExceptionNullCell()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A5", "5-b2");
            s.GetCellContents(null);
        }

        /// <summary>
        /// Tests return is exception for invalid cell name
        /// Definition of cell name is taken from PS3
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void ExceptionInvalidCell()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("_A5", "5-b2");
        }
        /// <summary>
        /// See what happens when a cell is instantiated
        /// with a formula object containing invalid formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void FormulaContentInvalid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("_1", "=$2 + 1");
        }
        /// <summary>
        /// Tests if all the nonempty cells are returned
        /// </summary>
        [TestMethod]
        public void TestGetAllNamesOfNonEmptyCells()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A5", "5-b2");
            s.SetContentsOfCell("A1", "=3+1");
            s.SetContentsOfCell("c17", "12.0");
            LinkedList<string> cells = new LinkedList<string>();
            cells.AddLast("c17");
            cells.AddLast("A1");
            cells.AddLast("A5");
            Assert.IsTrue(cells.First().Equals(s.GetNamesOfAllNonemptyCells().First()));
            Assert.IsTrue(cells.Last().Equals(s.GetNamesOfAllNonemptyCells().Last()));
        }

        /// <summary>
        /// Tests if all the nonempty cells are returned with one cell being empty
        /// </summary>
        [TestMethod]
        public void TestGetAllNamesOfNonEmptyCellsWithEmptyCell()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A5", "");
            s.SetContentsOfCell("A1", "=3+1");
            s.SetContentsOfCell("c17", "12.0");
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
            LinkedList<string> cells = new LinkedList<string>();
            cells.AddLast("c17");
            cells.AddLast("A1");
            s.SetContentsOfCell("A5", " ");
            Assert.AreEqual(3, s.GetNamesOfAllNonemptyCells().Count());
            s.SetContentsOfCell("A5", "hello World");
            cells.AddLast("A5");
            Assert.AreEqual(3, s.GetNamesOfAllNonemptyCells().Count());
            s.SetContentsOfCell("A5", "");
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
        }
        /// <summary>
        /// Tests if dependency graph is properly created
        /// </summary>
        [TestMethod]
        public void SetCells()
        {
            IList<string> graph;
            Spreadsheet s = new Spreadsheet();
            graph = s.SetContentsOfCell("A5", "=B5-4");
            Assert.IsTrue(graph.Count() == 1);
            s.SetContentsOfCell("A1", "=B2+1");
            s.SetContentsOfCell("A3", "=B5/6");
            graph = s.SetContentsOfCell("B5", "8");
            Assert.AreEqual(3, graph.Count());

        }
        /// <summary>
        /// Stress tests the spreadsheet
        /// </summary>
        [TestMethod]
        public void StressTest()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A5", "=B5-4");
            s.SetContentsOfCell("A1", "=B2+1");
            s.SetContentsOfCell("A3", "=B5/6");
            Assert.AreEqual(1, s.SetContentsOfCell("A2", "31.3902").Count());
            s.SetContentsOfCell("A4", "364 - 31");
            s.SetContentsOfCell("A6", "22 / 4");
            s.SetContentsOfCell("A7", " ");
            Assert.AreEqual(3, s.SetContentsOfCell("B5", "").Count());
            Assert.AreEqual(7, s.GetNamesOfAllNonemptyCells().Count());
            s.SetContentsOfCell("A1", "=4");
            s.SetContentsOfCell("A3", "Hello World");
            s.SetContentsOfCell("A7", "12.4");
            Assert.AreEqual(7, s.GetNamesOfAllNonemptyCells().Count());
            Assert.AreEqual(2, s.SetContentsOfCell("B5", "39749-27.3").Count());
            s.SetContentsOfCell("A5", "");
            s.SetContentsOfCell("A1", "");
            s.SetContentsOfCell("A3", " ");
            Assert.AreEqual(1, s.SetContentsOfCell("A2", " ").Count());
            s.SetContentsOfCell("A4", "");
            s.SetContentsOfCell("A6", "");
            s.SetContentsOfCell("A7", " ");
            Assert.AreEqual("B5", s.SetContentsOfCell("B5", " ").First());
            Assert.AreEqual(4, s.GetNamesOfAllNonemptyCells().Count());
        }
        /// <summary>
        /// Stress tests the spreadsheet for updated values
        /// </summary>
        [TestMethod]
        public void StressTestValues()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A5", "=B5-4");
            s.SetContentsOfCell("A1", "=A4+1");
            s.SetContentsOfCell("A3", "=B5/6");
            Assert.IsTrue(s.GetCellValue("A3") is FormulaError);
            Assert.IsTrue(s.GetCellValue("A1") is FormulaError);
            Assert.AreEqual(1, s.SetContentsOfCell("A2", "31.3902").Count());
            s.SetContentsOfCell("A4", "=364 - 31");
            Assert.AreEqual(334.0, s.GetCellValue("A1"));
            s.SetContentsOfCell("A6", "22 / 4");
            s.SetContentsOfCell("A7", " ");
            Assert.AreEqual(3, s.SetContentsOfCell("B5", "84").Count());
            Assert.AreEqual(8, s.GetNamesOfAllNonemptyCells().Count());
            s.SetContentsOfCell("A1", "=4");
            Assert.AreEqual(14.0, s.GetCellValue("A3"));
            Assert.AreEqual(80.0, s.GetCellValue("A5"));
            s.SetContentsOfCell("A3", "Hello World");
            Assert.AreEqual("Hello World", s.GetCellValue("A3"));
            s.SetContentsOfCell("A7", "12.4");
            Assert.AreEqual(2, s.SetContentsOfCell("B5", "39749-27.3").Count());
            Assert.AreEqual(1, s.SetContentsOfCell("A2", " ").Count());
            Assert.AreEqual("B5", s.SetContentsOfCell("B5", " ").First());
            Assert.IsTrue(s.GetCellValue("A5") is FormulaError);
        }
    }
}
