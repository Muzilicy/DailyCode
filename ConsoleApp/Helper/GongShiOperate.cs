using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp.Helper
{
    /// <summary>
    /// 公式分隔
    /// </summary>
    public class GongShiOperate
    {
        /// <summary>
        /// 用于匹配运算符和参数的正则表达式
        /// </summary>
        private readonly Regex _regex = new Regex(@"(^-|(?<=\D)-)?[\d.]+|[+\-*\/%!()√^]|log");
        /// <summary>
        /// 存放运算符对应的优先级，查字典判断运算符优先级
        /// </summary>
        private readonly Dictionary<string, int> _prio = new Dictionary<string, int>();

        public GongShiOperate()
        {
            _prio.Add("(", 0); //左括号的优先级最低，遇到左括号直接入栈
            _prio.Add(")", 0); //不会用到右括号的优先级，但是右括号不可能入栈，再次只把右括号当做一个运算符


            //双目运算符
            _prio.Add("+", 1); //加法优先级为1
            _prio.Add("-", 1); //减法优先级为1
            _prio.Add("*", 2); //乘法优先级为2
            _prio.Add("/", 2); //除法优先级为2

        }

        // if [职工信息].[合同工龄] > 26 &&  [职工信息].[合同工龄] < 35 then 10 * 100 else [职工信息].[合同工龄] * 100

        public decimal Calculate(string expression)
        {
            //expression = @"if {职工信息$合同工龄} > 26 
//then 10 * 100 else {职工信息$合同工龄} * 100"; //&&  [职工信息].[合同工龄] < 35
            expression = Regex.Replace(expression, @"\r\n?|\n", "")
                              .Replace("如果", "if").Replace("那么",",").Replace("否则",",")
                              .Replace("then", ",").Replace("else", ",");
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException("表达式不能为空");
            }
            Console.WriteLine("公式:" + expression);
            //int indexIf = expression.IndexOf("if");
            //int indexThen = expression.IndexOf("then");
            //int indexElse = expression.IndexOf("else");
            //string expression1 = string.Empty, expression2 = string.Empty, expression3 = string.Empty;
            ////string expression1 = expression.Substring()
            //if(indexIf > -1 && indexThen > -1)
            //{
            //    expression1 = expression.Substring(indexIf + 2, indexThen - indexIf -2);
            //    Console.WriteLine(expression1);
            //}
            //if (indexThen > -1 && indexElse > -1)
            //{
            //    expression2 = expression.Substring(indexThen + 4, indexElse - indexThen - 4);
            //    Console.WriteLine(expression2);
            //}
            //if (indexElse > -1)
            //{
            //    expression3 = expression.Substring(indexElse + 4);
            //    Console.WriteLine(expression3);
            //}
            //if(!string.IsNullOrEmpty(expression1) && expression1.IndexOf("&&") > -1)
            //{
            //    int indexAnd = expression1.IndexOf("&&");

            //}

            string result = DealSingle(expression);
            decimal d = new MixedOperation().GetMixedOperationRes(result);
            Console.WriteLine("结果："+d);
            return d;
        }

        private  string GetVal(string Deform)
        {
            GetString(Deform, 0);

            return string.Empty;
        }

        /// <summary>
        /// 处理单个if
        /// </summary>
        /// <param name="Deform"></param>
        public string DealSingle(string Deform)
        {
            //if ("KX002" > 150 ,99 ,88 )
            // 新思路 提取每个编码,转化为值,最后做逻辑运算
            string ret = "";
            var value1 = GetString(Deform.Split(',')[0], 1);//第一个值，这里表示 KX002
            var value2 = GetSecondValue(Deform);//第一个值，这里表示 150,这里也有可能不是数字，是款项编码
            var value3 = Deform.Split(',')[1];//这表示99，这里也有可能不是数字，是款项编码
            var value4 = Deform.Split(',')[2];//这表示88，这里也有可能不是数字，是款项编码
            value2 = value2.Replace("{", "").Replace("}", "");//去掉{}
            value3 = value3.Replace("{", "").Replace("}", "");
            value4 = value4.Replace("{", "").Replace("}", "");

            value1 = GetItemValue(value2);
            value2 = GetItemValue(value2);
            value3 = GetItemValue(value3);
            value4 = GetItemValue(value4);

            ret = GetVal(Deform, value1, value2, value3, value4);
            return ret;
            
        }

        /// <summary>
        /// 获取[]里面的内容
        /// </summary>
        /// <param name="Deform"></param>
        /// <param name="index">获取第几对双引号</param>
        /// <returns></returns>
        public string GetString(string Deform, int index)
        {
            int start = 0;
            int end = 0;
            if (index == 1)
            {
                start = Deform.IndexOf("{", 0);
                end = Deform.IndexOf("}", start + 1);
            }
            else if (index == 2)
            {
                start = Deform.IndexOf("{", 0);
                end = Deform.IndexOf("}", start + 1);

                start = Deform.IndexOf("{", end + 1);
                end = Deform.IndexOf("}", start + 1);
            }
            if (end == -1 && start == -1)  //类似格式 if 1>1
            {
                //没有[]
                if (Deform.IndexOf(">") != -1)
                {
                    end = Deform.IndexOf(">");
                }
                else if (Deform.IndexOf("<") != -1)
                {
                    end = Deform.IndexOf("<");
                }
                else if (Deform.IndexOf("=") != -1)
                {
                    end = Deform.IndexOf("=");
                }
                start = Deform.IndexOf("if") + 2;

            }

            Deform = Deform.Substring(start + 1, end - start - 1);

            return Deform;
        }

        /// <summary>
        /// 获取单个if里面的第二个编码或值
        /// </summary>
        /// <returns></returns>
        public string GetSecondValue(string Deform)
        {
            var rightVal = "";
            if (Deform.IndexOf(">=") > -1)
            {
                int j = 2;// 表示>=的长度
                int num1 = Deform.IndexOf(">=");
                int start = 0;
                int end = 0;
                start = num1;
                end = Deform.IndexOf(",", start + j);//then位置
                rightVal = Deform.Substring(start + j, end - start - j);
            }
            else if (Deform.IndexOf("<=") > -1)
            {
                int j = 2;// 表示<=的长度
                int num1 = Deform.IndexOf("<=");
                int start = 0;
                int end = 0;
                start = num1;
                end = Deform.IndexOf(",", start + j);//then位置
                rightVal = Deform.Substring(start + j, end - start - j);
            }
            else if (Deform.IndexOf(">") > -1)
            {
                int j = 1;// 表示>的长度
                int num1 = Deform.IndexOf(">");
                int start = 0;
                int end = 0;
                start = num1;
                end = Deform.IndexOf(",", start + j);//then位置
                rightVal = Deform.Substring(start + j, end - start - j);
            }
            else if (Deform.IndexOf("<") > -1)
            {
                int j = 1;// 表示<的长度
                int num1 = Deform.IndexOf("<");
                int start = 0;
                int end = 0;
                start = num1;
                end = Deform.IndexOf(",", start + j);//then位置
                rightVal = Deform.Substring(start + j, end - start - j);
            }
            else // 最后一种 等于 =
            {
                int j = 1;// 表示=的长度
                int num1 = Deform.IndexOf("=");
                int start = 0;
                int end = 0;
                start = num1;
                end = Deform.IndexOf(",", start + j);//then位置
                rightVal = Deform.Substring(start + j, end - start - j);
            }

            return rightVal;
        }

        /// <summary>
        /// 获取占位符对应的值
        /// </summary>
        /// <returns></returns>
        public string GetItemValue(string val)
        {
            val = val.Replace("职工信息", "")
                     .Replace("合同工龄","6")
                     .Replace("$", "");
            return val;
        }
        /// <summary>
        /// if逻辑计算
        /// </summary>
        /// <returns></returns>
        public string GetVal(string Deform, string value1, string value2, string value3, string value4)
        {
            string ret = "";
            if (Deform.IndexOf(">=") > -1)
            {
                if (Decimal.Parse(value1) >= Decimal.Parse(value2))
                {
                    ret = value3;
                }
                else
                {
                    ret = value4;
                }
            }
            else if (Deform.IndexOf("<=") > -1)
            {
                if (Decimal.Parse(value1) <= Decimal.Parse(value2))
                {
                    ret = value3;
                }
                else
                {
                    ret = value4;
                }
            }
            else if (Deform.IndexOf(">") > -1)
            {
                if (Decimal.Parse(value1) > Decimal.Parse(value2))
                {
                    ret = value3;
                }
                else
                {
                    ret = value4;
                }
            }
            else if (Deform.IndexOf("<") > -1)
            {
                if (Decimal.Parse(value1) < Decimal.Parse(value2))
                {
                    ret = value3;
                }
                else
                {
                    ret = value4;
                }
            }
            else // 最后一种 等于 =
            {
                if (value1 == value2)
                {
                    ret = value3;
                }
                else
                {
                    ret = value4;
                }
            }
            return ret;
        }
    }
}
