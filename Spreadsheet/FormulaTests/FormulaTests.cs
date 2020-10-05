using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SpreadsheetUtilities;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace FormulaTests
{
    /// <summary>
    /// This is my tester class for PS3. Besides the tests are 3 private helper methods: 
    /// 2 lookup methods and 1 normalizer.  These tests were in part the same ones used to 
    /// grade PS1 and  27 of them are original to this project. I believe the tests names are
    /// sufficient descriptions and a few XML comments are made where I think the test name is vague.
    /// @author Andrew Porter, 16 September 2020
    /// </summary>
    [TestClass]
    public class FormulaTests
    {
        // Normalizer tests
        [TestMethod(), Timeout(2000)]
        [TestCategory("1")]
        public void TestNormalizerGetVars()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            HashSet<string> vars = new HashSet<string>(f.GetVariables());

            Assert.IsTrue(vars.SetEquals(new HashSet<string> { "X1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("2")]
        public void TestNormalizerEquals()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("2+X1", s => s.ToUpper(), s => true);

            Assert.IsTrue(f.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("3")]
        public void TestNormalizerToString()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula(f.ToString());

            Assert.IsTrue(f.Equals(f2));
        }

        // Validator tests
        [TestMethod(), Timeout(2000)]
        [TestCategory("4")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorFalse()
        {
            Formula f = new Formula("2+x1", s => s, s => false);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("5")]
        public void TestValidatorX1()
        {
            Formula f = new Formula("2+x", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("6")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX2()
        {
            Formula f = new Formula("2+y1", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("7")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX3()
        {
            Formula f = new Formula("2+x1", s => s, s => (s == "x"));
        }


        // Simple tests that return FormulaErrors
        [TestMethod(), Timeout(2000)]
        [TestCategory("8")]
        public void TestUnknownVariable()
        {
            Formula f = new Formula("2+X1");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("9")]
        public void TestDivideByZero()
        {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("10")]
        public void TestDivideByZeroVars()
        {
            Formula f = new Formula("(5 + X1) / (X1 - 3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 3), typeof(FormulaError));
        }


        // Tests of syntax errors detected by the constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("11")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula f = new Formula("+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("12")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            Formula f = new Formula("2+5+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("13")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraCloseParen()
        {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("14")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOpenParen()
        {
            Formula f = new Formula("((3+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("15")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator()
        {
            Formula f = new Formula("5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("16")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator2()
        {
            Formula f = new Formula("5+5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("17")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator3()
        {
            Formula f = new Formula("5+7+(5)8");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("18")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator4()
        {
            Formula f = new Formula("5 5");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("19")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestDoubleOperator()
        {
            Formula f = new Formula("5 + + 3");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("20")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula f = new Formula("");
        }

        // Some more complicated formula evaluations
        [TestMethod(), Timeout(2000)]
        [TestCategory("21")]
        public void TestComplex1()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.14285714285714, (double)f.Evaluate(s => (s == "x7") ? 1 : 4), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("22")]
        public void TestRightParens()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, (double)f.Evaluate(s => 1), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("23")]
        public void TestLeftParens()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f.Evaluate(s => 2), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("53")]
        public void TestRepeatedVar()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, (double)f.Evaluate(s => 3), 1e-9);
        }

        // Test of the Equals method
        [TestMethod(), Timeout(2000)]
        [TestCategory("24")]
        public void TestEqualsBasic()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula("X1+X2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("25")]
        public void TestEqualsWhitespace()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula(" X1  +  X2   ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("26")]
        public void TestEqualsDouble()
        {
            Formula f1 = new Formula("2+X1*3.00");
            Formula f2 = new Formula("2.00+X1*3.0");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("27")]
        public void TestEqualsComplex()
        {
            Formula f1 = new Formula("1e-2 + X5 + 17.00 * 19 ");
            Formula f2 = new Formula("   0.0100  +     X5+ 17 * 19.00000 ");
            Assert.IsTrue(f1.Equals(f2));
        }


        [TestMethod(), Timeout(2000)]
        [TestCategory("28")]
        public void TestEqualsNullAndString()
        {
            Formula f = new Formula("2");
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals(""));
        }


        // Tests of == operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("29")]
        public void TestEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("30")]
        public void TestEqFalse()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsFalse(f1 == f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("31")]
        public void TestEqNull()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(null == f1);
            Assert.IsFalse(f1 == null);
            Assert.IsTrue(f1 == f2);
        }


        // Tests of != operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("32")]
        public void TestNotEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("33")]
        public void TestNotEqTrue()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsTrue(f1 != f2);
        }


        // Test of ToString method
        [TestMethod(), Timeout(2000)]
        [TestCategory("34")]
        public void TestString()
        {
            Formula f = new Formula("2*5");
            Assert.IsTrue(f.Equals(new Formula(f.ToString())));
        }


        // Tests of GetHashCode method
        [TestMethod(), Timeout(2000)]
        [TestCategory("35")]
        public void TestHashCode()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("2*5");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        // Technically the hashcodes could not be equal and still be valid,
        // extremely unlikely though. Check their implementation if this fails.
        [TestMethod(), Timeout(2000)]
        [TestCategory("36")]
        public void TestHashCodeFalse()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("3/8*2+(7)");
            Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("37")]
        public void TestHashCodeComplex()
        {
            Formula f1 = new Formula("2 * 5 + 4.00 - _x");
            Formula f2 = new Formula("2*5+4-_x");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }


        // Tests of GetVariables method
        [TestMethod(), Timeout(2000)]
        [TestCategory("38")]
        public void TestVarsNone()
        {
            Formula f = new Formula("2*5");
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("39")]
        public void TestVarsSimple()
        {
            Formula f = new Formula("2*X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("40")]
        public void TestVarsTwo()
        {
            Formula f = new Formula("2*X2+Y3");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "Y3", "X2" };
            Assert.AreEqual(actual.Count, 2);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("41")]
        public void TestVarsDuplicate()
        {
            Formula f = new Formula("2*X2+X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("42")]
        public void TestVarsComplex()
        {
            Formula f = new Formula("X1+Y2*X3*Y2+Z7+X1/Z8");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X1", "Y2", "X3", "Z7", "Z8" };
            Assert.AreEqual(actual.Count, 5);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        // Tests to make sure there can be more than one formula at a time
        [TestMethod(), Timeout(2000)]
        [TestCategory("43")]
        public void TestMultipleFormulae()
        {
            Formula f1 = new Formula("2 + a1");
            Formula f2 = new Formula("3");
            Assert.AreEqual(2.0, f1.Evaluate(x => 0));
            Assert.AreEqual(3.0, f2.Evaluate(x => 0));
            Assert.IsFalse(new Formula(f1.ToString()) == new Formula(f2.ToString()));
            IEnumerator<string> f1Vars = f1.GetVariables().GetEnumerator();
            IEnumerator<string> f2Vars = f2.GetVariables().GetEnumerator();
            Assert.IsFalse(f2Vars.MoveNext());
            Assert.IsTrue(f1Vars.MoveNext());
        }

        // Repeat this test to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("44")]
        public void TestMultipleFormulaeB()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("45")]
        public void TestMultipleFormulaeC()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("46")]
        public void TestMultipleFormulaeD()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("47")]
        public void TestMultipleFormulaeE()
        {
            TestMultipleFormulae();
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("48")]
        public void TestConstructor()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // This test is repeated to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("49")]
        public void TestConstructorB()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("50")]
        public void TestConstructorC()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("51")]
        public void TestConstructorD()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("52")]
        public void TestConstructorE()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }
        [TestMethod]
        public void BasicFormulaEval()
        {
            Formula f = new Formula("3+25");
            double x = (double)f.Evaluate(BasicLookup);
            Assert.AreEqual(28.0, x);
        }

        [TestMethod]
        public void ExceptionThrownUnknownVariable()
        {
            Formula f = new Formula("12 + _1");
            Assert.IsTrue(f.Evaluate(BasicLookup) is FormulaError);
        }

        [TestMethod]
        public void PassNullLookupMethod()
        {
            Formula f = new Formula(" 12+ 7 +A1");
            Assert.IsTrue(f.Evaluate(null) is FormulaError);
        }

        [TestMethod]
        public void BasicFormulaEvalWithVarCompiles()
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
        public void FormulaIsEmpty()
        {
            Formula f = new Formula(" ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaIsEmptyEmpty()
        {
            Formula f = new Formula("");
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
            Formula f = new Formula("(3.25+ ((28.25+32) - 4 * ___1) /(A12A*3)) + B24 - 2 * (_B32) + 5", SimpleNormalizer, s => true);
            Assert.AreEqual("(3.25+((28.25+32)-4*___1)/(a12a*3))+b24-2*(_b32)+5", f.ToString());
        }
        [TestMethod]
        public void EvaluateStressTest()
        {
            Formula f = new Formula("(3.25+ ((28.25+32) - 4 * ___1) /(A12A*3)) + B24 - 2 * (_B32) + 5");
            double x = (double)f.Evaluate(IntermedLookup);
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
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructFormulaWithNull()
        {
            Formula f = new Formula(null);
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
            IEnumerable<string> s = f.GetVariables();
            foreach (string svar in s)
            {
                if (svar == "A6")
                    Assert.IsTrue(true);
                else if (svar == "_y")
                    Assert.IsTrue(true);
                else if (svar == "Q_11")
                    Assert.IsTrue(true);
            }
        }
        /// <summary>
        /// This is testing to see if getVariables returns the
        /// same variable more than once because the variable appears
        /// more than once in the formula.  Flags are created to detect this.
        /// Code Coverage of this test is observed to make sure each Assert.IsTrue(true)
        /// Code Coverage reveals that all variables were identified in the formula once
        /// is touched on.
        /// </summary>
        [TestMethod]
        public void GetVariablesTestSameVariables()
        {
            Formula f = new Formula("A6+A6/13 -(3-5)*_y *a6 +_y +(Q_11-5)");
            IEnumerable<string> s = f.GetVariables();
            bool A6Seen = false;
            bool _ySeen = false;

            foreach (string svar in s)
            {
                if (svar == "A6")
                {
                    if (!A6Seen)
                    {
                        Assert.IsTrue(true);
                        A6Seen = true;
                    }
                    else
                        Assert.IsTrue(false);
                }
                else if (svar == "_y")
                {
                    if (!_ySeen)
                    {
                        Assert.IsTrue(true);
                        _ySeen = true;
                    }
                    else
                        Assert.IsTrue(false);
                }
                else if (svar == "Q_11")
                    Assert.IsTrue(true);
                else if (svar == "a6")
                    Assert.IsTrue(true);
            }
        }

        /// <summary>
        /// I am using tests from PS1 Grading Tests
        /// </summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("1")]
        public void TestSingleNumber()
        {
            Assert.AreEqual(5, (double)new Formula("5").Evaluate(IntermedLookup), 1e-9);
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
            Assert.AreEqual(8.0, (double)f.Evaluate(IntermedLookup), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("7")]
        public void TestArithmeticWithVariable()
        {
            Assert.AreEqual(6, (double)new Formula("2+Abba").Evaluate(IntermedLookup), 1e-9);
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
            Assert.AreEqual(20, (double)new Formula("2 + 6*3").Evaluate(IntermedLookup), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("11")]
        public void TestParenthesesTimes()
        {
            Assert.AreEqual(24, (double)new Formula("(2+6)*3").Evaluate(IntermedLookup), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("14")]
        public void TestPlusComplex()
        {
            Assert.AreEqual(14, (double)new Formula("2+(3+5*9)/Abba").Evaluate(IntermedLookup), 1e-9);
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
            Assert.AreEqual(194, (double)new Formula("2+3*5+(3+4*8)*5+2").Evaluate(IntermedLookup), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("18")]
        public void TestDivideBy0()
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
        public void TestSingleOperators()
        {
            Formula f = new Formula("+");
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("20")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperators()
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
            Assert.AreEqual(30, (double)f.Evaluate(BasicLookup), 1e-9);
        }

        /// <summary>
        /// This is a basic lookup method that only returns one value
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public double BasicLookup(string s)
        {
            if (s == "_1")
                throw new ArgumentException();
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

        /// <summary>
        /// Simple normalizer 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
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
