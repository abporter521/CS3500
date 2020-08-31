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

        public static int Evaluate (String exp, Lookup variableEval)
        {
            string[] tokens = new string[exp.Length]; //POSSIBLE ERROR HERE WITH LENGTH
            Stack <string> operators;
            Stack<string> values;
            int finalValue = 0;
            
            //Parse string into an array to evaluate separately
            tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            
            //Move through the tokens and perform operations
            foreach(string tok in tokens)
            {
                int i = 0;
                if (int.TryParse(tok, out i) && operators.Peek() == "*")
                    finalValue += 1;

            }
            //Pop stacks and return value
            return finalValue;
        }
       
    }
}
