using System;
using System.Collections.Generic;

namespace ConsoleApp.Formulas
{
    public abstract class SalaryFormula
    {
        protected string _SourceExpressionString { get; set; }
        public SalaryFormula(string expression)
        {
            _SourceExpressionString = expression;
        }

        public static SalaryFormula CreateFormula(string expression)
        {
            if (expression.StartsWith("if", StringComparison.OrdinalIgnoreCase))
            {
                return new IfElseFormula(expression);
            }

            return new AlgebraFormula(expression);
        }

        //分隔公式字符串，解析成队列
        //protected abstract Queue<string> Split();
        protected abstract Stack<string> Split();
        protected bool ValidateExpression()
        {
            return true;
        }

        public Stack<string> GetQueue()
        {
            return Split();
        }

        public decimal Compulete()
        {
            return 0;
        }
    }
}
