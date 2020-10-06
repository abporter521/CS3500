using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Threading;

namespace ss
{
    /// <summary>
    /// This is my test class for PS5
    /// Code coverage is 98.44% for my spreadsheet class
    /// Code Coverage also accounts for AbstractSpreadsheet 
    /// of which I have no way of accessing the methods there like
    /// getCellstoRecalculate.  So it looks like overall coverage is
    /// less that 98%
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
            AbstractSpreadsheet s = new Spreadsheet(null, s => s, "default");
        }
        /// <summary>
        /// Tests return throws for null arguments
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void NullConstructorArguments2()
        {
            AbstractSpreadsheet s = new Spreadsheet(null, s => true, s => s, "default");
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
            Spreadsheet s = new Spreadsheet("errortest6.xml", s => true, s => s, " ");

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
            AbstractSpreadsheet s = new Spreadsheet("errortest4.xml", s=> true, s=>s, "testerVersion");
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
        /// Tests if a cells values are changed when a new cell gets instantiated.
        /// 
        /// For Example cells A4 = B4 + 7 and when  the cell is set,
        /// with store a FormulaError object because B4 does not have value.
        /// When B4 is instantiated with 10, A4's value should update to 17
        /// </summary>
        [TestMethod]
        public void CellValueUpdatesTest2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A4", "B4+7");
            Assert.AreEqual("B4+7", s.GetCellValue("A4"));
            s.SetContentsOfCell("B4", "=A4-5");
            Assert.IsTrue(s.GetCellValue("B4") is FormulaError);
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
        /// Tests the SavedVersion method of spreadsheet
        /// </summary>
        [TestMethod]
        public void BasicSavedVersionTest()
        {
            AbstractSpreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "testerVersion");
            Assert.IsFalse(s.Changed);
            s.SetContentsOfCell("a4", "=B4+7");
            Assert.IsTrue(s.Changed);
            s.SetContentsOfCell("B4", "5");
            Assert.IsTrue(s.Changed);
            s.SetContentsOfCell("d3", "hello World");
            Assert.IsTrue(s.Changed);
            s.Save("test.xml");
            Assert.IsFalse(s.Changed);
            Assert.AreEqual("testerVersion", s.GetSavedVersion("test.xml"));

        }
        /// <summary>
        /// Tests for exception with faulty xml file.  
        /// ErrorTest1 has error of element tags that don't exist.
        /// Example: <Error>test</Error>
        /// </summary>
        [TestMethod]
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

        /// <summary>
        ///This is a test class for SpreadsheetTest and is intended
        ///to contain all SpreadsheetTest Unit Tests
        ///</summary>
        [TestClass()]
        public class GradingTests
        {

            // Verifies cells and their values, which must alternate.
            public void VV(AbstractSpreadsheet sheet, params object[] constraints)
            {
                for (int i = 0; i < constraints.Length; i += 2)
                {
                    if (constraints[i + 1] is double)
                    {
                        Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                    }
                    else
                    {
                        Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                    }
                }
            }


            // For setting a spreadsheet cell.
            public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
            {
                List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
                return result;
            }

            // Tests IsValid
            [TestMethod, Timeout(2000)]
            [TestCategory("1")]
            public void IsValidTest1()
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "x");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("2")]
            [ExpectedException(typeof(InvalidNameException))]
            public void IsValidTest2()
            {
                AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
                ss.SetContentsOfCell("A1", "x");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("3")]
            public void IsValidTest3()
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("B1", "= A1 + C1");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("4")]
            [ExpectedException(typeof(FormulaFormatException))]
            public void IsValidTest4()
            {
                AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
                ss.SetContentsOfCell("B1", "= A1 + C1");
            }

            // Tests Normalize
            [TestMethod, Timeout(2000)]
            [TestCategory("5")]
            public void NormalizeTest1()
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("B1", "hello");
                Assert.AreEqual("", s.GetCellContents("b1"));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("6")]
            public void NormalizeTest2()
            {
                AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
                ss.SetContentsOfCell("B1", "hello");
                Assert.AreEqual("hello", ss.GetCellContents("b1"));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("7")]
            public void NormalizeTest3()
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("a1", "5");
                s.SetContentsOfCell("A1", "6");
                s.SetContentsOfCell("B1", "= a1");
                Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("8")]
            public void NormalizeTest4()
            {
                AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
                ss.SetContentsOfCell("a1", "5");
                ss.SetContentsOfCell("A1", "6");
                ss.SetContentsOfCell("B1", "= a1");
                Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
            }

            // Simple tests
            [TestMethod, Timeout(2000)]
            [TestCategory("9")]
            public void EmptySheet()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                VV(ss, "A1", "");
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("10")]
            public void OneString()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                OneString(ss);
            }

            public void OneString(AbstractSpreadsheet ss)
            {
                Set(ss, "B1", "hello");
                VV(ss, "B1", "hello");
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("11")]
            public void OneNumber()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                OneNumber(ss);
            }

            public void OneNumber(AbstractSpreadsheet ss)
            {
                Set(ss, "C1", "17.5");
                VV(ss, "C1", 17.5);
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("12")]
            public void OneFormula()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                OneFormula(ss);
            }

            public void OneFormula(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "5.2");
                Set(ss, "C1", "= A1+B1");
                VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("13")]
            public void Changed()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                Assert.IsFalse(ss.Changed);
                Set(ss, "C1", "17.5");
                Assert.IsTrue(ss.Changed);
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("14")]
            public void DivisionByZero1()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                DivisionByZero1(ss);
            }

            public void DivisionByZero1(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "0.0");
                Set(ss, "C1", "= A1 / B1");
                Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("15")]
            public void DivisionByZero2()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                DivisionByZero2(ss);
            }

            public void DivisionByZero2(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "5.0");
                Set(ss, "A3", "= A1 / 0.0");
                Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
            }



            [TestMethod, Timeout(2000)]
            [TestCategory("16")]
            public void EmptyArgument()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                EmptyArgument(ss);
            }

            public void EmptyArgument(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "C1", "= A1 + B1");
                Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("17")]
            public void StringArgument()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                StringArgument(ss);
            }

            public void StringArgument(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "hello");
                Set(ss, "C1", "= A1 + B1");
                Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("18")]
            public void ErrorArgument()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                ErrorArgument(ss);
            }

            public void ErrorArgument(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "");
                Set(ss, "C1", "= A1 + B1");
                Set(ss, "D1", "= C1");
                Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("19")]
            public void NumberFormula1()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                NumberFormula1(ss);
            }

            public void NumberFormula1(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "C1", "= A1 + 4.2");
                VV(ss, "C1", 8.3);
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("20")]
            public void NumberFormula2()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                NumberFormula2(ss);
            }

            public void NumberFormula2(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "= 4.6");
                VV(ss, "A1", 4.6);
            }


            // Repeats the simple tests all together
            [TestMethod, Timeout(2000)]
            [TestCategory("21")]
            public void RepeatSimpleTests()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                Set(ss, "A1", "17.32");
                Set(ss, "B1", "This is a test");
                Set(ss, "C1", "= A1+B1");
                OneString(ss);
                OneNumber(ss);
                OneFormula(ss);
                DivisionByZero1(ss);
                DivisionByZero2(ss);
                StringArgument(ss);
                ErrorArgument(ss);
                NumberFormula1(ss);
                NumberFormula2(ss);
            }

            // Four kinds of formulas
            [TestMethod, Timeout(2000)]
            [TestCategory("22")]
            public void Formulas()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                Formulas(ss);
            }

            public void Formulas(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.4");
                Set(ss, "B1", "2.2");
                Set(ss, "C1", "= A1 + B1");
                Set(ss, "D1", "= A1 - B1");
                Set(ss, "E1", "= A1 * B1");
                Set(ss, "F1", "= A1 / B1");
                VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("23")]
            public void Formulasa()
            {
                Formulas();
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("24")]
            public void Formulasb()
            {
                Formulas();
            }


            // Are multiple spreadsheets supported?
            [TestMethod, Timeout(2000)]
            [TestCategory("25")]
            public void Multiple()
            {
                AbstractSpreadsheet s1 = new Spreadsheet();
                AbstractSpreadsheet s2 = new Spreadsheet();
                Set(s1, "X1", "hello");
                Set(s2, "X1", "goodbye");
                VV(s1, "X1", "hello");
                VV(s2, "X1", "goodbye");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("26")]
            public void Multiplea()
            {
                Multiple();
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("27")]
            public void Multipleb()
            {
                Multiple();
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("28")]
            public void Multiplec()
            {
                Multiple();
            }

            // Reading/writing spreadsheets
            [TestMethod, Timeout(2000)]
            [TestCategory("29")]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest1()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                ss.Save(Path.GetFullPath("/missing/save.txt"));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("30")]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest2()
            {
                AbstractSpreadsheet ss = new Spreadsheet(Path.GetFullPath("/missing/save.txt"), s => true, s => s, "");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("31")]
            public void SaveTest3()
            {
                AbstractSpreadsheet s1 = new Spreadsheet();
                Set(s1, "A1", "hello");
                s1.Save("save1.txt");
                s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
                Assert.AreEqual("hello", s1.GetCellContents("A1"));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("32")]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest4()
            {
                using (StreamWriter writer = new StreamWriter("save2.txt"))
                {
                    writer.WriteLine("This");
                    writer.WriteLine("is");
                    writer.WriteLine("a");
                    writer.WriteLine("test!");
                }
                AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("33")]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest5()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                ss.Save("save3.txt");
                ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("34")]
            public void SaveTest6()
            {
                AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
                ss.Save("save4.txt");
                Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("35")]
            public void SaveTest7()
            {
                using (XmlWriter writer = XmlWriter.Create("save5.txt"))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "hello");
                    writer.WriteEndElement();

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A2");
                    writer.WriteElementString("contents", "5.0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A3");
                    writer.WriteElementString("contents", "4.0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A4");
                    writer.WriteElementString("contents", "= A2 + A3");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
                VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("36")]
            public void SaveTest8()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                Set(ss, "A1", "hello");
                Set(ss, "A2", "5.0");
                Set(ss, "A3", "4.0");
                Set(ss, "A4", "= A2 + A3");
                ss.Save("save6.txt");
                using (XmlReader reader = XmlReader.Create("save6.txt"))
                {
                    int spreadsheetCount = 0;
                    int cellCount = 0;
                    bool A1 = false;
                    bool A2 = false;
                    bool A3 = false;
                    bool A4 = false;
                    string name = null;
                    string contents = null;

                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    Assert.AreEqual("default", reader["version"]);
                                    spreadsheetCount++;
                                    break;

                                case "cell":
                                    cellCount++;
                                    break;

                                case "name":
                                    reader.Read();
                                    name = reader.Value;
                                    break;

                                case "contents":
                                    reader.Read();
                                    contents = reader.Value;
                                    break;
                            }
                        }
                        else
                        {
                            switch (reader.Name)
                            {
                                case "cell":
                                    if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                                    else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                                    else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                                    else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                                    else Assert.Fail();
                                    break;
                            }
                        }
                    }
                    Assert.AreEqual(1, spreadsheetCount);
                    Assert.AreEqual(4, cellCount);
                    Assert.IsTrue(A1);
                    Assert.IsTrue(A2);
                    Assert.IsTrue(A3);
                    Assert.IsTrue(A4);
                }
            }


            // Fun with formulas
            [TestMethod, Timeout(2000)]
            [TestCategory("37")]
            public void Formula1()
            {
                Formula1(new Spreadsheet());
            }
            public void Formula1(AbstractSpreadsheet ss)
            {
                Set(ss, "a1", "= a2 + a3");
                Set(ss, "a2", "= b1 + b2");
                Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
                Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
                Set(ss, "a3", "5.0");
                Set(ss, "b1", "2.0");
                Set(ss, "b2", "3.0");
                VV(ss, "a1", 10.0, "a2", 5.0);
                Set(ss, "b2", "4.0");
                VV(ss, "a1", 11.0, "a2", 6.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("38")]
            public void Formula2()
            {
                Formula2(new Spreadsheet());
            }
            public void Formula2(AbstractSpreadsheet ss)
            {
                Set(ss, "a1", "= a2 + a3");
                Set(ss, "a2", "= a3");
                Set(ss, "a3", "6.0");
                VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
                Set(ss, "a3", "5.0");
                VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("39")]
            public void Formula3()
            {
                Formula3(new Spreadsheet());
            }
            public void Formula3(AbstractSpreadsheet ss)
            {
                Set(ss, "a1", "= a3 + a5");
                Set(ss, "a2", "= a5 + a4");
                Set(ss, "a3", "= a5");
                Set(ss, "a4", "= a5");
                Set(ss, "a5", "9.0");
                VV(ss, "a1", 18.0);
                VV(ss, "a2", 18.0);
                Set(ss, "a5", "8.0");
                VV(ss, "a1", 16.0);
                VV(ss, "a2", 16.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("40")]
            public void Formula4()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                Formula1(ss);
                Formula2(ss);
                Formula3(ss);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("41")]
            public void Formula4a()
            {
                Formula4();
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("42")]
            public void MediumSheet()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                MediumSheet(ss);
            }

            public void MediumSheet(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "1.0");
                Set(ss, "A2", "2.0");
                Set(ss, "A3", "3.0");
                Set(ss, "A4", "4.0");
                Set(ss, "B1", "= A1 + A2");
                Set(ss, "B2", "= A3 * A4");
                Set(ss, "C1", "= B1 + B2");
                VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
                Set(ss, "A1", "2.0");
                VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
                Set(ss, "B1", "= A1 / A2");
                VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("43")]
            public void MediumSheeta()
            {
                MediumSheet();
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("44")]
            public void MediumSave()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                MediumSheet(ss);
                ss.Save("save7.txt");
                ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
                VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("45")]
            public void MediumSavea()
            {
                MediumSave();
            }


            // A long chained formula. Solutions that re-evaluate 
            // cells on every request, rather than after a cell changes,
            // will timeout on this test.
            // This test is repeated to increase its scoring weight
            [TestMethod, Timeout(6000)]
            [TestCategory("46")]
            public void LongFormulaTest()
            {
                object result = "";
                LongFormulaHelper(out result);
                Assert.AreEqual("ok", result);
            }

            [TestMethod, Timeout(6000)]
            [TestCategory("47")]
            public void LongFormulaTest2()
            {
                object result = "";
                LongFormulaHelper(out result);
                Assert.AreEqual("ok", result);
            }

            [TestMethod, Timeout(6000)]
            [TestCategory("48")]
            public void LongFormulaTest3()
            {
                object result = "";
                LongFormulaHelper(out result);
                Assert.AreEqual("ok", result);
            }

            [TestMethod, Timeout(6000)]
            [TestCategory("49")]
            public void LongFormulaTest4()
            {
                object result = "";
                LongFormulaHelper(out result);
                Assert.AreEqual("ok", result);
            }

            [TestMethod, Timeout(6000)]
            [TestCategory("50")]
            public void LongFormulaTest5()
            {
                object result = "";
                LongFormulaHelper(out result);
                Assert.AreEqual("ok", result);
            }

            public void LongFormulaHelper(out object result)
            {
                try
                {
                    AbstractSpreadsheet s = new Spreadsheet();
                    s.SetContentsOfCell("sum1", "= a1 + a2");
                    int i;
                    int depth = 100;
                    for (i = 1; i <= depth * 2; i += 2)
                    {
                        s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                        s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                    }
                    s.SetContentsOfCell("a" + i, "1");
                    s.SetContentsOfCell("a" + (i + 1), "1");
                    Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
                    s.SetContentsOfCell("a" + i, "0");
                    Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
                    s.SetContentsOfCell("a" + (i + 1), "0");
                    Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
                    result = "ok";
                }
                catch (Exception e)
                {
                    result = e;
                }
            }

        }
    }
}
