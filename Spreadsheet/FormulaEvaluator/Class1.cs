using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
/*This project is used to evaluate simple infix arithmetic sequences such as 1+1.  2 stacks will be used to handle
* order of operations. Variables with one or more letters followed by one or more numbers
* can also be used in place of concrete values
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
            //Try catches an empty stack exception if operands are illegally placed such as --
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

                    //If token is an integer
                    if (int.TryParse(tok, out i) && !OpEmpty)
                    {
                        switch (operators.Peek())
                        {
                            case "*":
                                values.Push(values.Pop() * i);
                                operators.Pop();
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

                    //if the first token of the whole expression is an integer
                    else if (int.TryParse(tok, out i) && OpEmpty)
                        values.Push(i);

                    // if token is * or /
                    else if (tok == "*" || tok == "/")
                    {
                        operators.Push(tok);
                        OpEmpty = false;
                    }

                    //if token is a variable
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

                    //if token is + or -
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

                    //if token is (
                    else if (tok == "(")
                    {
                        operators.Push(tok);
                        OpEmpty = false;
                    }

                    //if token is )
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

                //Pop stacks and return value
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
                else if (operators.Count == 0)
                    return values.Pop();

                throw new ArgumentException("Incomplete formula. Needs more operands for this operation.");
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException("Incomplete formula.  Needs more operands for this operation.");

            }
        }
    }
}
