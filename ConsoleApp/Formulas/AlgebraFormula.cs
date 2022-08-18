using ConsoleApp.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Formulas
{
    public class AlgebraFormula : SalaryFormula
    {
        public AlgebraFormula(string expression) : base(expression)
        {
        }

        protected override Stack<string> Split()
        {
            Stack<string> stack = new Stack<string>();
            string cal = MathUtilHelper.ToPostfixExpression(_SourceExpressionString);
            stack.Push(cal);
            return stack;
        }
    }
}
