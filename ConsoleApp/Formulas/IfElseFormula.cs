using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp.Formulas
{
    public class IfElseFormula : SalaryFormula
    {
        private readonly string[] _operators = new string[] { "if", "then", "else", "else if" };

        private DBContext _dBContext { get; set; }
        public IfElseFormula(string expression) : base(expression)
        {
            _dBContext = new DBContext();
        }

        private bool IsOperator(string s)
        {
            for (int i = 0; i < _operators.Length; i++)
            {
                if (_operators[i] == s)
                {
                    return true;
                }
            }
            return false;
        }

        protected Queue<string> Split2()
        {
            Queue<string> queue = new Queue<string>();
            char[] chars = _SourceExpressionString.ToCharArray();
            int i = 0, length = chars.Length;
            string str, str1, item = string.Empty;
            for (; i < length; i++)
            {
                str = chars[i].ToString();
                if ((i + 1) == length)
                {
                    str1 = chars[i].ToString() + " ";
                }
                else
                {
                    str1 = chars[i].ToString() + chars[i + 1].ToString();
                }
                string str2 = string.Empty;
                if ((i + 5) < length)
                {
                    str2 = _SourceExpressionString.Substring(i, 4);
                }
                if (IsOperator(str2))
                {
                    if (item != string.Empty)
                    {
                        queue.Enqueue(item);
                        item = string.Empty;
                    }
                    if (str2 == "else" && i + 7 < length && _SourceExpressionString.Substring(i, 7) == "else if")
                    {
                        str2 = _SourceExpressionString.Substring(i, 7);
                    }

                    queue.Enqueue(str2);
                    i += str2.Length - 1;
                }
                else if (IsOperator(str1))
                {
                    if (item != string.Empty)
                    {
                        queue.Enqueue(item);
                        item = string.Empty;
                    }
                    queue.Enqueue(str1);
                    i+=1;
                }
                else
                {
                    item = item + str;
                }
            }
            if (item != string.Empty)
            {
                queue.Enqueue(item);
            }

            return queue;
        }

        protected override Stack<string> Split()
        {
            Stack<string> stack = new Stack<string>();
            char[] chars = _SourceExpressionString.ToCharArray();
            int i = 0, length = chars.Length;
            string str, str1, item = string.Empty;
            //var zhiGongXX = _dBContext.zhiGongXXes.Where(d => d.ID == "784403639014789120").ToList()?.FirstOrDefault();
            //Console.WriteLine(zhiGongXX?.ToString());

            return stack;
        }
    }
}
