using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

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
            s.SetCellContents("A4", new Formula ("A4+7"));
            Assert.AreEqual(new Formula("A4+7"), s.GetCellContents("A4"));
        }
    }
}
