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
            double x = (double)f.Evaluate(BasicLookup);
            Assert.AreEqual(28.0, x);
        }
        [TestMethod]
        public void BasicFormulaWithVar()
        {
            Formula f = new Formula("3+_3");
            double x = (double)f.Evaluate(BasicLookup);
            Assert.AreEqual(8.0, x);
        }

        [TestMethod]
        public void FormulaWithComplicatedVar()
        {
            Formula f = new Formula("14 + 12 - 3 *(_AFDA87_fva7 + b___242jk)/2");
            double x = (double)f.Evaluate(BasicLookup);
            Assert.AreEqual(11.0, x);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IsEmpty()
        {
            Formula f = new Formula(" ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BasicVariableNameError()
        {
            Formula f = new Formula("6A + 7");
        }

        /// <summary>
        /// Since 6A is parsed into 6 and A. The exception thrown on previous test was thrown
        /// on A.  I want to see if it will allow 6A8 where A8 is a valid variable name if
        /// the exception is still thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BasicVariableNameErrorVerify()
        {
            Formula f = new Formula("6A8 + 7");
        }

        [TestMethod]
        public void BasicParentheseAreValid()
        {
            Formula f = new Formula("12*(A_13 + 7)");
            double d = (double)f.Evaluate(BasicLookup);
            Assert.AreEqual(144.0, d);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BasicParentheseError()
        {
            Formula f = new Formula("14*(3 + 7");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BasicParentheseOperatorFollowingError()
        {
            Formula f = new Formula("14*(+3 + 7)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BasicExtraFollowingError()
        {
            Formula f = new Formula("14*(3 + 7 190)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EmptyParentheseError()
        {
            Formula f = new Formula("14*() + 12");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ImplicitMultiplicationError()
        {
            Formula f = new Formula("14(5) + 12");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BasicBeginningTokenError()
        {
            Formula f = new Formula(") 14*(3 + 7 190)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BasicBeginningTokenErrorInvalidToken()
        {
            Formula f = new Formula("$ 14*(3 + 7 190)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BasicEndingTokenError()
        {
            Formula f = new Formula("14*(3 + 7) -");
        }

        [TestMethod]   
        public void EvaluateStressTest()
        {
            Formula f = new Formula("(3.25+ ((28.25+32) - 4 * ___1) /(A12A*3)) + B24 - 2 * (_B32) + 5");
            double x = (double) f.Evaluate(IntermedLookup);
            Assert.AreEqual(16.25, x);

        }
        /// <summary>
        /// This is a basic lookup method that only returns one value
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public double BasicLookup(string s)
        {
            return 5d;
        }

        public double IntermedLookup(string s)
        {
            switch (s)
            {
                case "A12A":
                    return 2.5;
                case "_B32":
                    return 0;
                case "___1":
                    return 1;
                default:
                    return 0.5;
            }

        }
    }
}
