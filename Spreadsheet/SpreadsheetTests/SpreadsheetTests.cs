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
            s.SetCellContents("A4", new Formula("A4+7"));
            Assert.AreEqual(new Formula("A4+7"), s.GetCellContents("A4"));
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
            Assert.IsTrue(cells.First().Equals(s.GetNamesOfAllNonemptyCells().First()));
            Assert.IsTrue(cells.Last().Equals(s.GetNamesOfAllNonemptyCells().Last()));
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
            s.SetCellContents("A1", new Formula("3+1"));
        }
    }
}
