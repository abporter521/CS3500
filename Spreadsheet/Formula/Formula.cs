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
        //These are the class variables that are created with each object
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
            //Tokens are placed in Token string array, placeHolder is the index of this array
            int placeHolder = 0;
            //A double to assign in case token is double
            double i;
            //Counts ( and )
            int closeParen = 0;
            int openParen = 0;
            //Initializes before token which stores token before the current token in token array
            string beforeToken = "";
            Regex varPattern = new Regex(basicVarPattern);

            //Check to make sure formula is not empty
            if (GetTokens(formula).Count() == 0 || formula == null)
                throw new FormulaFormatException("Please enter a non-empty formula.");
            tokens = new string[GetTokens(formula).Count()];
            //Checks that all tokens are of valid format
            foreach (string token in GetTokens(formula))
            {
                //Create bool to check if previous term is a double for checking Extra Following Rule
                bool beforeIsDouble = false;
                //Assigns beforeToken to previous element in tokens
                if (placeHolder > 0)
                    beforeToken = tokens[placeHolder - 1];
                double old;
                //Checks if previous term is a double
                if (double.TryParse(beforeToken, out old))
                    beforeIsDouble = true;

                //Check if the token is (, ), +, -, /, *
                if (token == "(" || token == "+" || token == "-" || token == ")" || token == "*" || token == "/")
                {
                    //Checks Starting Token Rule
                    if ((token == "+" || token == "-" || token == ")" || token == "*" || token == "/") && placeHolder == 0)
                        throw new FormulaFormatException("Formula must begin with a valid variable, number, or ( .");
                    //Checks Ending Token Rule
                    if ((token == "+" || token == "-" || token == "(" || token == "*" || token == "/") && placeHolder == tokens.Length - 1)
                        throw new FormulaFormatException("Formula must end with a valid variable, number, or ) .");
                    //Checks Parenthesis/Operator Following Rule
                    if ((token == "+" || token == "-" || token == ")" || token == "*" || token == "/") && beforeToken == "(")
                        throw new FormulaFormatException("Formula cannot have operator immediately following ( . Try adding a variable or number.");
                    //Checks Extra Following Rule
                    if (token == "(")
                    {
                        //If before token is ), a variable, or a double, Extra Following Rule is violated
                        if (beforeToken == ")" || varPattern.IsMatch(normalize(beforeToken)) || beforeIsDouble)
                            throw new FormulaFormatException("Implicit multiplication is not allowed.");
                        openParen++;
                    }
                    //Increments ) counter
                    if (token == ")")
                        closeParen++;
                    //Add the token to the tokens array
                    tokens[placeHolder] = token;
                    //Update placeHolder variable
                    placeHolder++;
                    continue;
                }
                //If the token is a double
                else if (double.TryParse(token, out i))
                {
                    //Checks Extra Following Rule
                    if (beforeToken == ")" || varPattern.IsMatch(normalize(beforeToken)) || beforeIsDouble)
                        throw new FormulaFormatException("Implicit multiplication is not allowed.");
                    //Saves double to the tokens array
                    tokens[placeHolder] = token;
                    //Update placeHolder variable
                    placeHolder++;
                    continue;
                }
                //If the token matches basic variable format when normalized
                if (varPattern.IsMatch(normalize(token)))
                {
                    //Checks if normalized token meets specifications of user's validator
                    if (isValid(normalize(token)))
                    {
                        //Checks Extra Following Rule
                        if (beforeToken == ")" || varPattern.IsMatch(normalize(beforeToken)) || beforeIsDouble)
                            throw new FormulaFormatException("Implicit multiplication is not allowed.");
                        //Save the normalized variable to tokens array
                        tokens[placeHolder] = normalize(token);
                        placeHolder++;
                    }
                }
                //If token goes through all these if statements and does not trigger them, it must be that the token is of an invalid format so throw error
                else
                    throw new FormulaFormatException("There is an invalid variable or character in the formula.\n Make sure variables are of valid format.");
            }

            //Checks that there are no unmatched parentheses
            if (openParen != closeParen)
                throw new FormulaFormatException("There are unmatched parentheses.\nPlease double check your formula.");

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
            //Check if lookup method is null, if so, return formula error
            if (lookup == null)
                return new FormulaError("Null lookup formula not allowed. Please input a lookup method");

            //Stacks to keep track of values and operators
            Stack<string> operators = new Stack<string>();
            Stack<double> values = new Stack<double>();
            foreach (string tok in tokens)
            {
                double i = 0;

                //If token is a double, check for the next operator. If *, or / perform
                // the operation, otherwise push the integer
                if (double.TryParse(tok, out i) && !(operators.Count == 0))
                {
                    switch (operators.Peek())
                    {
                        //If * found, perform operation and push value to stack
                        case "*":
                            values.Push(values.Pop() * i);
                            operators.Pop();
                            break;
                        //If / found, perform operation if divsior is not 0 and push value to stack
                        case "/":
                            if (i == 0)
                                return new FormulaError("Cannot divide by 0");
                            values.Push(values.Pop() / i);
                            operators.Pop();
                            break;
                        //If neither are found, push double to stack
                        default:
                            values.Push(i);
                            break;
                    }
                }

                //If the first token of the whole expression is an integer, push onto value stack
                else if (double.TryParse(tok, out i) && (operators.Count == 0))
                    values.Push(i);

                // If token is * or /, push onto operator stack
                else if (tok == "*" || tok == "/")
                    operators.Push(tok);

                //If token is + or -, check if previous operator on stack is also + or -.
                //If so, perform the operation and add operator to stack, 
                //otherwise just add operator to stack
                else if (tok == "+" || tok == "-")
                {
                    if (!(operators.Count == 0))
                    {
                        //If there are +, - in the stack means there is 
                        //a statement equivalent to a + b + c where the second +
                        //is the tok. We want to add a + b before pushing second +
                        //to the operator stack. Otherwise just push + to stack
                        switch (operators.Peek())
                        {
                            //If + is found, add the next two values on value stack and push to stack
                            //then push operator to operator stack
                            case "+":
                                i = values.Pop() + values.Pop();
                                operators.Pop();
                                values.Push(i);
                                operators.Push(tok);
                                break;
                            //If - is found, subtract the next two values on value stack and push to stack
                            //then push operator to operator stack
                            case "-":
                                double subtractor = values.Pop();
                                i = values.Pop() - subtractor;
                                operators.Pop();
                                values.Push(i);
                                operators.Push(tok);
                                break;
                            //Push + or - to stack
                            default:
                                operators.Push(tok);
                                break;
                        }
                    }
                    //Push + or - to operator stack if operator stack is empty
                    else
                        operators.Push(tok);
                }

                //if token is (, push to operator stack
                else if (tok == "(")
                    operators.Push(tok);

                //if token is ), check if addition or subtraction was performed and perform it if possible
                //then check for multiplication or division.  If ( is found then pop
                else if (tok == ")")
                {
                    //Checks for statments to see if addition or 
                    //subtraction occurs in parentheses of format (a + b)
                    //If so, perform operation and push new value to stack
                    //If opening parenthese is detected we assume no operators
                    //are within the parenthese.
                    switch (operators.Peek())
                    {
                        //This +, - section is different than the one above
                        //because we also have to check for closure of the 
                        //parenthetical expression. Creating a helper to do
                        //the 2 instances of line 295 to 297 and line 303-306
                        //would require passing in the 2 stacks.
                        //Having it written this way is just as readable
                        case "+":
                            i = values.Pop() + values.Pop();
                            operators.Pop();
                            values.Push(i);
                            if (operators.Peek() == "(")
                                operators.Pop();
                            break;
                        //Check if - exists in the parenthetical expression
                        //if yes, perform operation and push to stack
                        case "-":
                            double subtraction = values.Pop();
                            i = values.Pop() - subtraction;
                            operators.Pop();
                            values.Push(i);
                            if (operators.Peek() == "(")
                                operators.Pop();
                            break;
                        //If ( is found, we consider the parenthetical expression complete and pop ( from stack
                        case "(":
                            operators.Pop();
                            break;
                    }
                    //If the operator stack is not empty, we check for
                    //multiplication or division that could be done alongside
                    //the parentheses such as a *(b + c) or that is done inside
                    //the parentheses such as (a*b)
                    if (!(operators.Count == 0))
                    {
                        switch (operators.Peek())
                        {
                            //If * is found, perform operation and push product to stack
                            case "*":
                                i = values.Pop() * values.Pop();
                                operators.Pop();
                                values.Push(i);
                                break;
                            //As long as divisor is not 0, perform operation and push quotient ot stack
                            case "/":
                                double divisor = values.Pop();
                                if (divisor == 0)
                                    return new FormulaError("Division by 0 occurs");
                                i = values.Pop() / divisor;
                                operators.Pop();
                                values.Push(i);
                                break;
                        }
                    }
                }

                //if token is a variable, lookup variable value and perform same algorithm
                //as the double section
                else if (validator(tok))
                {
                    //Retrieves value of variable from lookup method. Return Formula Error if variable throws from lookup method
                    try
                    {
                        i = lookup(tok);
                    }
                    catch (ArgumentException e)
                    {
                        //Throws error if variable is not found.
                        return new FormulaError("The variable could not be found.\nLookup method threw exception. Please make sure the lookup method can find variable.");
                    }
                    //If there are no operators, push value to stack
                    if (operators.Count == 0)
                    {
                        values.Push(i);
                        continue;
                    }
                    //If there are operators, perform operation
                    switch (operators.Peek())
                    {
                        //If * is found, calculate and push product to stack
                        case "*":
                            values.Push(values.Pop() * i);
                            operators.Pop();
                            break;
                        //If / is found and variable does not return 0, calculate and push quotient to stack
                        case "/":
                            if (i == 0)
                                return new FormulaError("Cannot divide by 0");
                            values.Push(values.Pop() / i);
                            operators.Pop();
                            break;
                        //Else push variable value to stack
                        default:
                            values.Push(i);
                            break;
                    }
                }
            }
            //Pop stacks and return value. Value in operator stack should be + or -
            if (operators.Count == 1 && values.Count == 2)
            {
                //if + is left, calculate and return sum
                if (operators.Peek() == "+")
                    return values.Pop() + values.Pop();
                //If - is left, calculate and return difference
                else if (operators.Peek() == "-")
                {
                    double subtractor = values.Pop();
                    return values.Pop() - subtractor;
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
            HashSet<string> var = new HashSet<string>();
            Regex varPattern = new Regex(basicVarPattern);
            foreach (string varTok in tokens)
            {
                //Check if variable matches specification and does not print operator token
                if (varPattern.IsMatch(varTok) && validator(varTok))
                {
                    //Check if variable was already returned
                    if (var.Contains(varTok))
                        continue;
                    //Return the variable and place in var hashSet so that it is not returned again
                    yield return varTok;
                    var.Add(varTok);
                }
            }
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
            StringBuilder formula = new StringBuilder();
            //Add each token of the formula to the string 
            foreach (string formulaPieces in tokens)
            {
                formula.Append(formulaPieces);
            }
            return formula.ToString();
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
            if (!(obj is Formula) || obj == null)
                return false;

            Formula f = (Formula)obj;
            //If the size of their arrays are not the same length, they cannot be equal.
            if (f.TokensArray.Length != this.TokensArray.Length)
                return false;
            int i = 0;
            //doubles that represent possible doubles found while checking for equality
            double resultObj;
            double resultThis;
            foreach (string item in f.TokensArray)
            {
                //Checks for equality in doubles
                if (Double.TryParse(item, out resultObj))
                {
                    //if this does not have a double, but f does, return false
                    if (!Double.TryParse(tokens[i], out resultThis))
                        return false;
                    else
                    {
                        //If doubles are equal after converting back to string, do nothing. Else return false
                        if (resultObj.ToString().Equals(resultThis.ToString()))
                        {
                            i++;
                            continue;
                        }
                        else
                            return false;
                    }
                }
                //If any token in index i is not equivalent to f's token at index i, return false
                if (this.TokensArray[i] != f.TokensArray[i])
                    return false;
                i++;
            }
            //Return true if after cycling through every token, no inequality was flagged
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            //If both f1 and f2 are null, they are equal, return true
            if (ReferenceEquals(f1, null) && ReferenceEquals(f2, null))
                return true;
            //If only one is null, return false
            if (ReferenceEquals(f1, null))
                return false;
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            //If both f1 and f2 are null, they are equals, return false
            if (ReferenceEquals(f1, null) && ReferenceEquals(f2, null))
                return false;
            //If only one is null, return true
            if (ReferenceEquals(f1, null))
                return true;
            return !(f1.Equals(f2));
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            double helperD;
            int hashCode = 0;
            foreach (string item in this.TokensArray)
            {
                //Checks for sameness in doubles
                if (Double.TryParse(item, out helperD))
                {
                    //Convert doubles back to string to assure equality of hashcodes
                    string convertedD = helperD.ToString();
                    //Multiplication ensures uniqueness of hashcode
                    hashCode *= (convertedD.GetHashCode() / 2);
                }
                //Add to hashcode for non double tokens
                else
                    hashCode += item.GetHashCode();
            }
            return hashCode;
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
        /// <summary>
        /// Helper to get access tokens array.
        /// </summary>
        public string[] TokensArray
        {
            get { return tokens; }
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

