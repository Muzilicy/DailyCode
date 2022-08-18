using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp.Helper
{
    public class MathUtilHelper
    {
        public static string Add(string a, string b)
        {
            return (double.Parse(a) + double.Parse(b)).ToString();
        }
        public static string Sub(string a, string b)
        {
            return (double.Parse(a) - double.Parse(b)).ToString();
        }
        public static string Mul(string a, string b)
        {
            return (double.Parse(a) * double.Parse(b)).ToString();
        }
        public static string Div(string a, string b)
        {
            double b1 = double.Parse(b);
            if (b1 == 0.0)
                return "除零错误";
            string s = (double.Parse(a) / b1).ToString();
            return s;
        }
        //是否为数 
        public static bool IsNum(char a)
        {
            if ((a >= '0' && a <= '9') || a == '.')
                return true;
            return false;
        }
        public static bool IsNum(string a)
        {
            if (a.Length <= 0)
                return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] > '9' || a[i] < '0')
                {
                    if (!(a[i] == '.' && i != 0))
                        return false;
                }
            }
            return true;
        }

        //是否为运算符 
        public static bool IsOperator(char a)
        {
            if (a == '+' || a == '-' || a == '*' || a == '/' || a == '(')
                return true;
            return false;
        }

        //计算优先级 
        public static bool Priority(char a, char b)
        {
            if (a == '*' || a == '/')
            {
                if (b == '-' || b == '+')
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        //将前缀表达式转换为后缀表达式空格隔开每个元素
        public static string ToPostfixExpression(string str)
        {
            string rel = "";
            int i = 0;
            Stack<char> sta = new Stack<char>();

            while (true)
            {
                if (i >= str.Length)
                    break;

                if (IsNum(str[i]))
                {
                    while (i < str.Length && IsNum(str[i]))
                    {
                        rel += str[i];
                        i++;
                    }
                    rel += " ";
                }
                if (i < str.Length && !IsNum(str[i]))
                {
                    if (str[i] == ')')
                    {//右括号 
                        while (true)
                        {
                            char item = sta.Count > 0 ? sta.Peek() : ' ';
                            if (item == '(')
                            {
                                sta.Pop();
                                break;
                            }
                            else
                            {
                                rel += item;
                                rel += " ";
                            }
                            if(sta.Count > 0)
                            {
                                sta.Pop();
                            }
                            
                        }
                    }
                    else if (i < str.Length && sta.Count == 0 || str[i] == '(' || sta.Peek() == '(')
                    {
                        //栈顶为空或者遇见左括号或者当前栈顶是左括号 
                        sta.Push(str[i]);
                    }
                    else
                    {
                        //栈顶优先级小于待入栈符号 
                        while (i < str.Length && !(sta.Count() == 0) && !Priority(str[i], sta.Peek()))
                        {
                            if (sta.Peek() != '(')
                            {
                                rel += sta.Peek();
                                rel += " ";
                            }
                            else
                            {
                                sta.Pop();
                                break;
                            }
                            sta.Pop();
                        }
                        sta.Push(str[i]);
                    }
                }
                i++;
            }

            while (!(sta.Count() == 0))
            {
                if (sta.Peek() != '(')
                {
                    rel += sta.Peek();
                    rel += " ";
                }
                sta.Pop();
            }
            return rel;
        }
        //传入后缀表达式计算结果 
        public static string calculatingSuffix(string ex)
        {
            Stack<string> sta = new Stack<string>();
            string[] strs = ex.Split(' ');
            for (int i = 0; i < strs.Length; i++)
            {
                if (IsNum(strs[i]))//是数字
                {
                    sta.Push(strs[i]);
                    //Console.WriteLine("数字" + strs[i]);
                }
                else if (!strs[i].Equals(" ") && !strs[i].Equals("") && sta.Count > 1)
                {
                    string x = sta.Pop();
                    string y = sta.Pop();
                    if (strs[i].Equals("+"))
                    {
                        sta.Push(Add(x, y));
                    }
                    else if (strs[i].Equals("-"))
                    {
                        sta.Push(Sub(y, x));
                    }
                    else if (strs[i].Equals("*"))
                    {
                        sta.Push(Mul(y, x));
                    }
                    else if (strs[i].Equals("/"))
                    {
                        sta.Push(Div(y, x));
                    }
                }
            }
            if(sta.Count > 0)
            {
                //Console.WriteLine(sta.Count + "--" + sta.Peek());
                return sta.Pop();
            }
            return null;
        }
    }
}
