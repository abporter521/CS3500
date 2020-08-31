using System;
using System.Collections.Generic;
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
        static string variablePattern = "/[a-zA-Z]+[0-9]+/";
        static Regex variableName = new Regex(variablePattern);
        public static int Evaluate (String exp, Lookup variableEval)
        {
            bool OpEmpty = true;
            bool ValEmpty = true;
            string[] tokens = new string[exp.Length]; //POSSIBLE ERROR HERE WITH LENGTH
            Stack <string> operators = new Stack<string>();
            Stack<int> values = new Stack<int>();
            int finalValue = 0;

            //Parse string into an array to evaluate separately
            tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            
            //Move through the tokens and perform operations
            foreach(string tok in tokens)
            {
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
                            values.Push(values.Pop() / i);
                            operators.Pop();
                            break;
                        default:
                            values.Push(i);
                            ValEmpty = false;
                            break;
                    }
                }

                //if the first token of the whole expression is an integer
                else if (int.TryParse(tok, out i) && OpEmpty)
                    values.Push(i);

                // if token is * or /
                if (tok == "*" || tok == "/")
                {
                    operators.Push(tok);
                    OpEmpty = false;
                }

                //if token is a variable
                if (variableName.IsMatch(tok) && !OpEmpty)
                {
                    i = variableEval(tok);
                    switch (operators.Peek())
                    {
                        case "*":
                            values.Push(values.Pop() * i);
                            operators.Pop();
                            break;
                        case "/":
                            values.Push(values.Pop() / i);
                            operators.Pop();
                            break;
                        default:
                            values.Push(i);
                            ValEmpty = false;
                            break;
                    }

                }

                //if token is + or -
                if (tok == "+" || tok == "-")
                {
                    switch (operators.Peek())
                    {
                        case "+":
                            i = values.Pop() + values.Pop();
                            operators.Pop();
                            values.Push(i);
                            ValEmpty = false;
                            break;
                        case "-":
                            i = values.Pop() - values.Pop();
                            operators.Pop();
                            values.Push(i);
                            ValEmpty = false;
                            break;
                        default:
                            operators.Push(tok);
                            OpEmpty = false;
                            break;

                    }

                }

                //if token is (
                if (tok == "(")
                {
                    operators.Push(tok);
                    OpEmpty = false;
                }

                //if token is )
                if (tok == ")")
                {
                    switch (operators.Peek())
                    {
                        case "+":
                            i = values.Pop() + values.Pop();
                            operators.Pop();
                            values.Push(i);
                            ValEmpty = false;
                            break;

                        case "-":
                            i = values.Pop() - values.Pop();
                            operators.Pop();
                            values.Push(i);
                            ValEmpty = false;
                            break;
                        case "(":
                            operators.Pop();
                            break;
                        case "*":
                            i = values.Pop() * values.Pop();
                            operators.Pop();
                            values.Push(i);
                            ValEmpty = false;
                            break;
                        case "/":
                            i = values.Pop() / values.Pop();
                            operators.Pop();
                            values.Push(i);
                            ValEmpty = false;
                            break;
                    }
                }
            }

            //Pop stacks and return value
            if (operators.Count == 1)
            {
                if (values.Count == 2) //can remove this when process is better.
                {
                    if (operators.Peek() == "+")
                        return values.Pop() + values.Pop();
                    else if (operators.Peek() == "-")
                        return values.Pop() - values.Pop();
                }
            }
            else if (operators.Count == 0)
                return values.Pop();
            return -1; //shows error
        }
               
    }
}
