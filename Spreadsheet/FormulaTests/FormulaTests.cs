using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SpreadsheetUtilities;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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
        public void BasicEqualsTestDifferentFormulae()
        {
            Formula f = new Formula("14*(3 + 7)");
            Formula e = new Formula("10*(3+7)");
            Assert.IsTrue(f != e);
            Formula x = new Formula("A6*(3+7)");
            Formula y = new Formula("14/(3 + 7)");
            Assert.IsFalse(x.Equals(f));
            Assert.IsTrue(y != f);
            Formula a = new Formula("(A5 + 17) -4*2");
            Formula b = new Formula("(A5+17)-4*   2");
            Assert.IsTrue(a == b);
            Assert.IsTrue(a.Equals(b));
            Assert.IsFalse(f.Equals(a));
            Assert.IsFalse(f == null);
            Assert.IsTrue(e != null);
            Assert.IsTrue(null != b);
        }
        [TestMethod]
        public void ToStringTest()
        {
            Formula f = new Formula("(3.25+ ((28.25+32) - 4 * ___1) /(A12A*3)) + B24 - 2 * (_B32) + 5", SimpleNormalizer, s=> true);        
            Assert.AreEqual("(3.25+((28.25+32)-4*___1)/(a12a*3))+b24-2*(_b32)+5", f.ToString());
        }
        [TestMethod]   
        public void EvaluateStressTest()
        {
            Formula f = new Formula("(3.25+ ((28.25+32) - 4 * ___1) /(A12A*3)) + B24 - 2 * (_B32) + 5");
            double x = (double) f.Evaluate(IntermedLookup);
            Assert.AreEqual(16.25, x);

        }

        [TestMethod]
        public void EvaluateFormulaErrorTestDivideBy0()
        {
            Formula f = new Formula("(3.25+ ((28.25+32) - 4 * ___1) /(_B32*3)) + B24 - 2 * (A12A) + 5");
            Assert.IsTrue(f.Evaluate(IntermedLookup) is FormulaError);

        }

        [TestMethod]
        public void AreEqualWithDoublesDifferentLengths()
        {
            Formula f = new Formula("17.000000000000001 + 4/2 + A23");
            Formula e = new Formula("17 + 4.0/2.00 + A23");
            Assert.IsTrue(f.Equals(e));

        }

        [TestMethod]
        public void HashCodeEqualWithDoublesDifferentLengths()
        {
            Formula f = new Formula("17.000000000000001 + 4/2 + A23");
            Formula e = new Formula("17 + 4.0/2.00 + A23");
            Assert.IsTrue(f.GetHashCode() == e.GetHashCode());

        }

        [TestMethod]
        public void HashCodeNotEqualWithVariables()
        {
            Formula f = new Formula("17.000000000000001 + 4/2 + A23");
            Formula e = new Formula("17 + A23 + 4.0/2.00 ");
            Assert.IsFalse(f.GetHashCode() == e.GetHashCode());
        }

        [TestMethod]
        public void NotEqualWithDoublesDifferentPlaces()
        {
            Formula f = new Formula("17.00 + 4/2 + A23");
            Formula e = new Formula("4.0/2.00  + 17 + A23");
            Assert.IsFalse(f.Equals(e));

        }

        [TestMethod]
        public void NotEqualWithNull()
        {
            Formula f = new Formula("17.00 + 4/2 + A23");
            Assert.IsFalse(f.Equals(null));

        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MethodCallWillEmptyArguement()
        {
            Formula f = new Formula(" ");
            f.GetHashCode();

        }

        [TestMethod]
        public void GetVariablesTest()
        {
            Formula f = new Formula("A6+39/13 -(3-5)*_y +(Q_11-5)");
            Assert.AreEqual(3, f.GetVariables().Count());
        }

        [TestMethod]
        public void GetVariablesTestSameVariable()
        {
            Formula f = new Formula("A6+A6/13 -(3-5)*_y +(Q_11-5)");
            Assert.AreEqual(3, f.GetVariables().Count());
        }

        /// <summary>
        /// I am using tests from PS1 Grading Tests
        /// </summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("1")]
        public void TestSingleNumber()
        {
            Assert.AreEqual(5, (double) new Formula("5").Evaluate(IntermedLookup),1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("2")]
        public void TestSingleVariable()
        {
            Assert.AreEqual(0.5, new Formula("X5").Evaluate(IntermedLookup));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("4")]
        public void TestSubtraction()
        {
            Assert.AreEqual(8.384, new Formula("10.384-2").Evaluate(IntermedLookup));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("5")]
        public void TestMultiplication()
        {
            Assert.AreEqual(8.2, new Formula("2*4.1").Evaluate(IntermedLookup));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("6")]
        public void TestDivision()
        {
            Formula f = new Formula("16/2");
            Assert.AreEqual(8.0, (double) f.Evaluate(IntermedLookup), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("7")]
        public void TestArithmeticWithVariable()
        {
            Assert.AreEqual(6, (double) new Formula("2+Abba").Evaluate(IntermedLookup), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("9")]
        public void TestLeftToRight()
        {
            Assert.AreEqual(15.4, new Formula("2 * 6+3.4").Evaluate(IntermedLookup));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("10")]
        public void TestOrderOperations()
        {
            Assert.AreEqual(20, (double) new Formula("2 + 6*3").Evaluate(IntermedLookup), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("11")]
        public void TestParenthesesTimes()
        {
            Assert.AreEqual(24, (double) new Formula("(2+6)*3").Evaluate(IntermedLookup), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("14")]
        public void TestPlusComplex()
        {
            Assert.AreEqual(14, (double) new Formula("2+(3+5*9)/Abba").Evaluate(IntermedLookup), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("15")]
        public void TestOperatorAfterParens()
        {
            Formula f = new Formula("(1*1)-2/2");

            Assert.AreEqual(0, (double)f.Evaluate(BasicLookup), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("17")]
        public void TestComplexAndParentheses()
        {
            Assert.AreEqual(194, (double) new Formula("2+3*5+(3+4*8)*5+2").Evaluate(IntermedLookup), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("18")]
        public void TestDivideByZero()
        {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(BasicLookup), typeof(FormulaError));
        }
        [TestMethod(), Timeout(5000)]
        [TestCategory("18")]
        public void TestDivideByVariableEqualTo0()
        {
            Formula f = new Formula("5/_B32");
            Assert.IsInstanceOfType(f.Evaluate(IntermedLookup), typeof(FormulaError));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("19")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula f = new Formula("+");
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("20")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            Formula f = new Formula("2+5+");
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("21")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraParentheses()
        {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("23")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestPlusInvalidVariable()
        {
            Formula f = new Formula("5+%xx");
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("28")]
        public void TestComplexNestedParensLeft()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(30, (double) f.Evaluate(BasicLookup), 1e-9);
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
                case "Abba":
                    return 4;
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
        public string SimpleNormalizer(string s)
        {
            StringBuilder variable = new StringBuilder();
            foreach (char c in s)
            {
                if (Char.IsLetter(c))
                    variable.Append(Char.ToLower(c));
                else
                    variable.Append(c);
            }
            return variable.ToString();
        }

    }
}
