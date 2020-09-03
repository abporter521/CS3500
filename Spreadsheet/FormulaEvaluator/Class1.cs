using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
/*This project is used to evaluate infix arithmetic sequences such as 1+1. Allowed operations are
 * multipliation, division, addition, subtraction and parentheses. 2 stacks will be used to handle
* order of operations. Variables with one or more letters followed by one or more numbers
* can also be used in place of concrete values. The lookup of these variables will be 
* supplied by a delgate given by the user. Exceptions will be thrown for null values, and invalid operator
* placement such as 1++3.
* 
* @author Andrew B Porter
* 31 August 2020
*/

namespace FormulaEvaluator
{

    public static class Evaluator
    {
        public delegate int Lookup(String v);

        //variable pattern will never change.  This is why it is static
        static string variablePattern = "^[a-zA-Z]+[0-9]+$";
        static Regex variableName = new Regex(variablePattern);

        public static int Evaluate(String exp, Lookup variableEval)
        {
            //Try catches an empty stack exception caused when operands are illegally placed such as -- or *+
            try
            {
                bool OpEmpty = true;
                string[] tokens = new string[exp.Length];
                Stack<string> operators = new Stack<string>();
                Stack<int> values = new Stack<int>();

                //Parse string into an array to evaluate separately
                tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

                //Move through the tokens and perform operations
                foreach (string tok in tokens)
                {
                    string trim = tok.Trim();
                    if (tok == "" || tok == " ")
                        continue;
                    int i = 0;

                    //If token is an integer, check for the next operator. If *, or / perform
                    // the operation, otherwise push the integer
                    if (int.TryParse(tok, out i) && !OpEmpty)
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
                                    throw new DivideByZeroException("Cannot divide by 0");
                                values.Push(values.Pop() / i);
                                operators.Pop();
                                break;
                            default:
                                values.Push(i);
                                break;
                        }
                    }

                    //if the first token of the whole expression is an integer, push onto value stack
                    else if (int.TryParse(tok, out i) && OpEmpty)
                        values.Push(i);

                    // if token is * or /, push onto operator stack
                    else if (tok == "*" || tok == "/")
                    {
                        operators.Push(tok);
                        OpEmpty = false;
                    }

                    //if token is a variable, lookup variable value and perform same algorithm
                    //as the integer section
                    else if (variableName.IsMatch(trim))
                    {
                        i = variableEval(trim);
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
                                    throw new DivideByZeroException("Cannot divide by 0");
                                values.Push(values.Pop() / i);
                                operators.Pop();
                                break;
                            default:
                                values.Push(i);
                                break;
                        }
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
                                    int subtractor = values.Pop();
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
                                int subtraction = values.Pop();
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
                                    int divisor = values.Pop();
                                    if (divisor == 0)
                                        throw new DivideByZeroException("Cannot divide by 0");
                                    i = values.Pop() / divisor;
                                    operators.Pop();
                                    if (operators.Count == 0)
                                        OpEmpty = true;
                                    values.Push(i);
                                    break;
                            }
                        }
                    }
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
                            int subtractor = values.Pop();
                            return values.Pop() - subtractor;
                        }
                    }
                }
                //Returns the value when there is only 1 value left in the value stack
                else if (operators.Count == 0)
                    return values.Pop();
                //Throws when illegal values are put in the formula.
                throw new ArgumentException("Incomplete formula. Needs more operands for this operation or variable has illegal name.");
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException("Incomplete formula.  Needs more operands for this operation.");

            }
        }
    }
}
