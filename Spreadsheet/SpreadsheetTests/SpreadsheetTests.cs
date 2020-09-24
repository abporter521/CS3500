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
            s.SetCellContents("A4", 32.0);
            Assert.AreEqual(32.0, s.GetCellContents("A4"));
        }
        /// <summary>
        /// Tests return is string for valid cell 
        /// </summary>
        [TestMethod]
        public void GetCellContentsValidString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A4", "c7+ 2k");
            Assert.AreEqual("c7+ 2k", s.GetCellContents("A4"));
        }
        /// <summary>
        /// Tests return is formula for valid cell 
        /// </summary>
        [TestMethod]
        public void GetCellContentsValidFormula()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A4", new Formula("B4+7"));
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
            s.SetCellContents("A4", new Formula("A4+7"));
         
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
            s.SetCellContents("A4", new Formula("B4+7"));
            s.SetCellContents("B4", new Formula("C4 -1"));
            s.SetCellContents("C4", new Formula("8+2"));
            Assert.AreEqual(3.0, s.GetNamesOfAllNonemptyCells().Count());
            s.SetCellContents("B4", new Formula("A4/2"));
        }
        /// <summary>
        /// Tests return is CircularException for cell with circular dependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void GetCellContentsFormulaCircularThreeElements()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A4", new Formula("B4+7"));
            s.SetCellContents("B4", new Formula("C4 -1"));
            s.SetCellContents("C4", new Formula("A4+2"));           
        }
        /// <summary>
        /// Given a valid dependency graph, if I change one in the middle,
        /// I want to assert that changes are made throughout
        /// </summary>
        [TestMethod]
        public void DependencyChange()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A4", new Formula("B4+7"));
            s.SetCellContents("B4", new Formula("C4 -1"));
            s.SetCellContents("C4", new Formula("8+2"));
            Assert.AreEqual(3.0, s.GetNamesOfAllNonemptyCells().Count());
            s.SetCellContents("B4", new Formula("8/2"));
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
            s.SetCellContents(null, new Formula("A4+7"));
        }
        /// <summary>
        /// Tests return is exception for null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void ExceptionNullCell()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A5", "5-b2");
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
            s.SetCellContents("_A5", "5-b2");
            Assert.AreEqual("5-b2", s.GetCellContents("_A5"));
            s.GetCellContents("5_V");
        }
        /// <summary>
        /// Tests if all the nonempty cells are returned
        /// </summary>
        [TestMethod]
        public void TestGetAllNamesOfNonEmptyCells()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A5", "5-b2");
            s.SetCellContents("A1", new Formula("3+1"));
            s.SetCellContents("c17", 12d);
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
            s.SetCellContents("A5", "");
            s.SetCellContents("A1", new Formula("3+1"));
            s.SetCellContents("c17", 12d);
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
            LinkedList<string> cells = new LinkedList<string>();
            cells.AddLast("c17");
            cells.AddLast("A1");
            s.SetCellContents("A5", " ");
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
            s.SetCellContents("A5", "hello World");
            cells.AddLast("A5");
            Assert.AreEqual(3, s.GetNamesOfAllNonemptyCells().Count());
            s.SetCellContents("A5", "");
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
            graph = s.SetCellContents("A5", new Formula("B5-4"));
            Assert.IsTrue(graph.Count() == 1);
            s.SetCellContents("A1", new Formula("B2+1"));
            s.SetCellContents("A3", new Formula("B5/6"));
            graph = s.SetCellContents("B5", 8);
            Assert.AreEqual(3, graph.Count());

        }
    }
}
