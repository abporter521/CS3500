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
    /// 
    /// I have not been allowed by the program structure to assign null
    /// as a cell content for any test. For this reason, there are not
    /// spreadsheets with null cells but valid cell names. For this coverage is
    /// not 100%. These lines were kept though to comply with method contracts
    /// 
    /// Andrew Porter 22 Sept 2
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
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A4"));
        }
        /// <summary>
        /// Tests return is double for valid cell
        /// </summary>
        [TestMethod]
        public void GetCellContentsValidDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "32.0");
            Assert.AreEqual(32.0, s.GetCellContents("A4"));
        }
        /// <summary>
        /// Tests return is string for valid cell 
        /// </summary>
        [TestMethod]
        public void GetCellContentsValidString()
        {
            Spreadsheet s = new Spreadsheet();
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
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A5 ", "1+1");
        }
        /// <summary>
        /// Tests cell dependency is correctly created
        /// </summary>
        [TestMethod]
        public void ChangeDependencyDouble()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1,s.SetContentsOfCell("A4", "=B4+7").Count());
            Assert.AreEqual(2,s.SetContentsOfCell("B4", "=C4 + 2").Count());
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
            Spreadsheet s = new Spreadsheet();
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
            Spreadsheet s = new Spreadsheet();
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
            Assert.AreEqual("5-b2", s.GetCellContents("_A5"));
            s.GetCellContents("5_V");
        }
        /// <summary>
        /// See what happens when a cell is instantiated
        /// with a formula object containing invalid formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
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
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A5", "=B5-4");            
            s.SetContentsOfCell("A1", "=B2+1");
            s.SetContentsOfCell("A3", "=B5/6");
            Assert.AreEqual(1,s.SetContentsOfCell("A2", "31.3902").Count());
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
            s.SetContentsOfCell("A5","");
            s.SetContentsOfCell("A1", "");
            s.SetContentsOfCell("A3"," ");
            Assert.AreEqual(1, s.SetContentsOfCell("A2", " ").Count());
            s.SetContentsOfCell("A4", "");
            s.SetContentsOfCell("A6", "");
            s.SetContentsOfCell("A7", " ");
            Assert.AreEqual("B5", s.SetContentsOfCell("B5", " ").First());
            Assert.AreEqual(4, s.GetNamesOfAllNonemptyCells().Count());
        }
    }
}
