// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens

// Andrew Porter, u1071655 14 Sept 2020



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private string formulaString;
        private Func<string, string> normalizer;
        private Func<string, bool> validator;
        private string[] tokens;
        private string basicVarPattern = "^[a-zA-Z_][0-9a-zA-Z_]+$";

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            int placeHolder = 0;
            double i;
            int closeParen = 0;
            int openParen = 0;
            string beforeToken = "";           
            Regex varPattern = new Regex(basicVarPattern);

            //Check to make sure formula is not empty
            if (GetTokens(formula).Count() == 0)
                throw new FormulaFormatException("Please enter a non-empty formula.");
            tokens = new string[GetTokens(formula).Count()];
            //Checks that all tokens are of valid format
            foreach (string token in GetTokens(formula))
            {
                bool isDouble = false;

                if (placeHolder > 0)
                    beforeToken = tokens[placeHolder - 1];
                double old;
                if (double.TryParse(beforeToken, out old))
                    isDouble = true;

                if (token == "(" || token == "+" || token == "-" || token == ")" || token == "*" || token == "/")
                {
                    //Checks Starting Token Rule
                    if ((token == "+" || token == "-" || token == ")" || token == "*" || token == "/") && placeHolder == 0)
                        throw new FormulaFormatException("Formula must begin with a valid variable, number, or ( .");
                    //Checks Ending Token Rule
                    if ((token == "+" || token == "-" || token == "(" || token == "*" || token == "/") && placeHolder == tokens.Length)
                        throw new FormulaFormatException("Formula must end with a valid variable, number, or ) .");
                    //Checks Parenthesis/Operator Following Rule
                    if ((token == "+" || token == "-" || token == ")" || token == "*" || token == "/") && tokens[placeHolder - 1] == "(")
                        throw new FormulaFormatException("Formula cannot have operator immediately following ( .");
                    //Checks Extra Following Rule
                    if (token == "(")
                    {
                        if (beforeToken == ")" || varPattern.IsMatch(beforeToken) || isDouble)
                            throw new FormulaFormatException("Implicit multiplication is not allowed.");
                        openParen++;
                    }
                    if (token == ")")
                        closeParen++;
                    tokens[placeHolder] = token;
                    placeHolder++;
                    continue;
                }
                else if (double.TryParse(token, out i))
                {                   
                    //Checks Extra Following Rule
                    if (beforeToken == ")" || varPattern.IsMatch(beforeToken) || isDouble)
                        throw new FormulaFormatException("Implicit multiplication is not allowed.");
                    tokens[placeHolder] = token;
                    placeHolder++;
                    continue;
                }
                if (varPattern.IsMatch(token))
                {
                    if (isValid(token))
                    {                       
                        //Checks Extra Following Rule
                        if (beforeToken == ")" || isValid(beforeToken) || isDouble)
                            throw new FormulaFormatException("Implicit multiplication is not allowed.");
                        tokens[placeHolder] = token;
                        placeHolder++;
                    }
                }
                else
                    throw new FormulaFormatException("There is an invalid variable or character in the formula.\n Make sure variables are of valid format.");
            }

            //Checks that there are no unmatched parentheses
            if (openParen != closeParen)
                throw new FormulaFormatException("There are unmatched parentheses.\n Please double check your formula.");

            //Set our class variables
            formulaString = formula;
            normalizer = normalize;
            validator = isValid;

        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            bool OpEmpty = true;
            Stack<string> operators = new Stack<string>();
            Stack<double> values = new Stack<double>();
            foreach (string tok in tokens)
            {
                double i = 0;

                //If token is an integer, check for the next operator. If *, or / perform
                // the operation, otherwise push the integer
                if (double.TryParse(tok, out i) && !OpEmpty)
                {
                    switch (operators.Peek())
                    {
                        case "*":
                            values.Push(values.Pop() * i);
                            operators.Pop();
                            if (operators.Count == 0)
                                OpEmpty = true;
                            break;
                        case "/":
                            if (i == 0)
                                return new FormulaError("Cannot divide by 0");
                            values.Push(values.Pop() / i);
                            operators.Pop();
                            break;
                        default:
                            values.Push(i);
                            break;
                    }
                }

                //if the first token of the whole expression is an integer, push onto value stack
                else if (double.TryParse(tok, out i) && OpEmpty)
                    values.Push(i);

                // if token is * or /, push onto operator stack
                else if (tok == "*" || tok == "/")
                {
                    operators.Push(tok);
                    OpEmpty = false;
                }

                //if token is + or -, check if previous operator on stack is also + or -.
                //If so, perform the operation otherwise and add operator to stack, 
                //otherwise just add operator to stack
                else if (tok == "+" || tok == "-")
                {
                    if (!OpEmpty)
                    {
                        switch (operators.Peek())
                        {
                            case "+":
                                i = values.Pop() + values.Pop();
                                operators.Pop();
                                values.Push(i);
                                operators.Push(tok);
                                break;
                            case "-":
                                double subtractor = values.Pop();
                                i = values.Pop() - subtractor;
                                operators.Pop();
                                values.Push(i);
                                operators.Push(tok);
                                break;
                            default:
                                operators.Push(tok);
                                OpEmpty = false;
                                break;
                        }
                    }
                    else
                    {
                        operators.Push(tok);
                        OpEmpty = false;
                    }
                }

                //if token is (, push to operator stack
                else if (tok == "(")
                {
                    operators.Push(tok);
                    OpEmpty = false;
                }

                //if token is ), check if addition or subtraction was performed and perform it if possible
                //then check for multiplication or division.  If ( is found then pop
                else if (tok == ")")
                {
                    switch (operators.Peek())
                    {
                        case "+":
                            i = values.Pop() + values.Pop();
                            operators.Pop();
                            values.Push(i);
                            if (operators.Peek() == "(")
                            {
                                operators.Pop();
                                if (operators.Count == 0)
                                    OpEmpty = true;
                            }
                            break;

                        case "-":
                            double subtraction = values.Pop();
                            i = values.Pop() - subtraction;
                            operators.Pop();
                            values.Push(i);
                            if (operators.Peek() == "(")
                            {
                                operators.Pop();
                                if (operators.Count == 0)
                                    OpEmpty = true;
                            }
                            break;

                        case "(":
                            operators.Pop();
                            if (operators.Count == 0)
                                OpEmpty = true;
                            break;
                    }
                    if (!OpEmpty)
                    {
                        switch (operators.Peek())
                        {
                            case "*":
                                i = values.Pop() * values.Pop();
                                operators.Pop();
                                if (operators.Count == 0)
                                    OpEmpty = true;
                                values.Push(i);
                                break;
                            case "/":
                                double divisor = values.Pop();
                                if (divisor == 0)
                                    return new FormulaError("Division by 0 occurs");
                                i = values.Pop() / divisor;
                                operators.Pop();
                                if (operators.Count == 0)
                                    OpEmpty = true;
                                values.Push(i);
                                break;
                        }
                    }
                }
               
                //if token is a variable, lookup variable value and perform same algorithm
                //as the integer section
                else if (validator(normalizer(tok)))
                {
                    i = lookup(normalizer(tok));
                    if (OpEmpty)
                    {
                        values.Push(i);
                        continue;
                    }
                    switch (operators.Peek())
                    {
                        case "*":
                            values.Push(values.Pop() * i);
                            operators.Pop();
                            if (operators.Count == 0)
                                OpEmpty = true;
                            break;
                        case "/":
                            if (i == 0)
                                return new FormulaError("Cannot divide by 0");
                            values.Push(values.Pop() / i);
                            operators.Pop();
                            break;
                        default:
                            values.Push(i);
                            break;
                    }
                }
                else if (!validator(normalizer(tok)))
                    return new FormulaError("The normalized variable is unknown");
            }
            //Pop stacks and return value. Value in operator stack should be + or -
            if (operators.Count == 1)
            {
                if (values.Count == 2)
                {
                    if (operators.Peek() == "+")
                        return values.Pop() + values.Pop();
                    else if (operators.Peek() == "-")
                    {
                        double subtractor = values.Pop();
                        return values.Pop() - subtractor;
                    }
                }
            }
            //Returns the value when there is only 1 value left in the value stack
            return values.Pop();
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return null;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return null;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            return false;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return false;
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return false;
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}

