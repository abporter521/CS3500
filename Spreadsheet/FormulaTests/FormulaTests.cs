using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SpreadsheetUtilities;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void BasicFormula()
        {
            Formula f = new Formula("3+25");
            double x =(double) f.Evaluate(MyLookup);
            Assert.AreEqual(28.0, x);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BasicVariableNameError()
        {
            Formula f = new Formula("6A + 7");
        }

        public double MyLookup(string s)
        {
            return 5d;
        }
    }
}
