using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp.Helper
{
    /// <summary>
    /// 比较运算符计算器
    /// </summary>
    public class CompareOperation
    {
        /// <summary>
        /// 用于匹配运算符和参数的正则表达式
        /// </summary>
        private readonly Regex _regex = new Regex(@"(^-|(?<=\D)-)?[\d.]+|[()]|([>][=])|([<][=])|([<][>])|([>])|([<])|([=][=])");
        private readonly Regex _regexNum = new Regex(@"(^-|(?<=\D)-)?[\d.]+");
        private readonly Regex _regexOperation = new Regex(@"[()]|([>][=])|([<][=])|([<][>])|([>])|([<])|([=][=])");
        /// <summary>
        /// 存放运算符对应的优先级，查字典判断运算符优先级
        /// </summary>
        private readonly Dictionary<string, int> _prio = new Dictionary<string, int>();
        /// <summary>
        /// 保存算式的集合
        /// </summary>
        private List<string> _suanShiList = new List<string>();
        /// <summary>
        /// 保存算式的中间集合
        /// </summary>
        private List<string> _suanShiZJList = new List<string>();
        /// <summary>
        /// 使用该堆栈将中缀表达式转为后缀表达式
        /// </summary>
        private Stack<string> _stk = new Stack<string>();

        public CompareOperation()
        {
            _prio.Add("(", 0); //左括号的优先级最低，遇到左括号直接入栈
            _prio.Add(")", 0); //不会用到右括号的优先级，但是右括号不可能入栈，再次只把右括号当做一个运算符


            //双目运算符
            _prio.Add(">=", 1);
            _prio.Add("<=", 1);
            _prio.Add("<>", 1);
            _prio.Add(">", 1);
            _prio.Add("<", 1);
            _prio.Add("=", 1);
        }
        /// <summary>
        /// 中文符号替换成英文符号
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private string RepSymbol(string exp)
        {
            Dictionary<string, string> repSymbol = new Dictionary<string, string>();
            repSymbol.Add("，", ",");
            repSymbol.Add("：", ":");
            repSymbol.Add("（", "(");
            repSymbol.Add("）", ")");
            repSymbol.Add("【", "[");
            repSymbol.Add("】", "]");
            repSymbol.Add("、", "/");
            repSymbol.Add("》",">");
            repSymbol.Add("《", "<");
            foreach (var item in repSymbol)
            {
                exp = exp.Replace(item.Key, item.Value);
            }
            return exp;
        }

        private string Compare(decimal num, string symbol)
        {
            if (symbol == ">=")
            {
                return (num >= 0).ToString();
            }
            else if (symbol == "<=")
            {
                return (num <= 0).ToString();
            }
            else if (symbol == "<>")
            {
                return (num != 0).ToString();
            }
            else if (symbol == ">")
            {
                return (num > 0).ToString();
            }
            else if (symbol == "<")
            {
                return (num < 0).ToString();
            }
            else
            {
                //symbol为"="时
                return (num == 0).ToString();
            }
        }
        /// <summary>
        /// 简单四则运算
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string Compute(string exp)
        {
            DataTable dt = new DataTable();
            string result = "";
            try
            {
                result = dt.Compute(exp, "").ToString();
            }
            catch (DivideByZeroException ex)
            {
                throw new Exception("发生除0错误,计算公式为{" + exp + " }");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ",计算公式为{ " + exp + " }");
            }

            if (result == "非数字" || result == "NaN")//可能和语言有关系，有些环境计算结果是非数字，有些是NaN
            {
                if (exp.IndexOf("/0") > -1)
                {
                    throw new Exception("发生除0错误,计算公式为{" + exp + " }");
                }
                else
                {
                    throw new NotFiniteNumberException("计算结果不为数字,计算公式为{ " + exp + " }");
                }
            }
            if (result.IndexOf("-") > -1)
            {
                result = "(" + result + ")";//当结果为负数时
            }
            return result;
        }

        private static string CompareDate(string paramstr, ref string errorMsg)
        {
            return DateDif("CompareDate", paramstr, ref errorMsg);
        }
        private void ErrorMsg(string date, string str1, ref string errorMsg)
        {
            if (!IsDateTime(date))
            {
                errorMsg = string.Format("{0}函数接收到非日期参数", str1);
            }
            else
            {
                errorMsg = string.Format("{0}函数的参数需要输入整数", str1);
            }
        }

        #region 判断字符串格式
        public string IsValidDate(string paramstr, ref string errorMsg)
        {
            if (IsDateTime(paramstr))
                return "1";
            else
                return "0";
        }
        private bool IsDateTime(string str)
        {
            if (Regex.IsMatch(str, @"^[12]\d{3}-\d{1,2}-\d{1,2} \d{2}:\d{2}:\d{2}"))//日期格式为yyyy-MM-dd hh:mm:ss
                return true;
            else
                return false;
        }
        private bool IsDigit(string str)
        {
            if (Regex.IsMatch(str, @"^\d+[.]?\d*$"))
            {
                return true;
            }
            else
                return false;
        }
        private static bool IsInt(string str)
        {
            if (Regex.IsMatch(str, @"^\d+$"))
            {
                return true;
            }
            else
                return false;
        }
        #endregion

        #region 数值转换
        private string Round(string paramstr, ref string errorMsg)
        {
            paramstr = Compute(paramstr);
            return Parse("Round", paramstr, ref errorMsg);
        }
        private string Int(string paramstr, ref string errorMsg)
        {
            paramstr = Compute(paramstr);
            return Parse("Int", paramstr, ref errorMsg);
        }
        private string Ceil(string paramstr, ref string errorMsg)
        {
            paramstr = Compute(paramstr);
            return Parse("Ceil", paramstr, ref errorMsg);
        }
        private string Floor(string paramstr, ref string errorMsg)
        {
            paramstr = Compute(paramstr);
            return Parse("Floor", paramstr, ref errorMsg);
        }
        private string Parse(string funcName, string paramstr, ref string errorMsg)
        {
            string result = "";

            if (IsDigit(paramstr))
            {
                switch (funcName)
                {
                    case "Round":
                        result = Math.Round(Convert.ToDouble(paramstr), MidpointRounding.AwayFromZero).ToString();
                        break;
                    case "Int":
                        result = ((int)double.Parse(paramstr)).ToString();
                        break;
                    case "Ceil":
                        result = Math.Ceiling(double.Parse(paramstr)).ToString();
                        break;
                    case "Floor":
                        result = Math.Floor(double.Parse(paramstr)).ToString();
                        break;
                    default:
                        errorMsg = string.Format("未知的函数名{0}", funcName);
                        break;
                }
                return result;
            }
            else
            {
                errorMsg = string.Format("{0}函数接收到非数字参数", funcName);
                return "";
            }
        }
        #endregion

        #region 日期增加
        public string DateAddYears(string paramstr, ref string errorMsg)
        {
            return DateAdd("DateAddYears", paramstr, ref errorMsg);
        }
        public string DateAddMonths(string paramstr, ref string errorMsg)
        {
            return DateAdd("DateAddMonths", paramstr, ref errorMsg);
        }
        public string DateAddDays(string paramstr, ref string errorMsg)
        {
            return DateAdd("DateAddDays", paramstr, ref errorMsg);
        }
        public string DateAddHours(string paramstr, ref string errorMsg)
        {
            return DateAdd("DateAddHours", paramstr, ref errorMsg);
        }
        public string DateAddMinutes(string paramstr, ref string errorMsg)
        {
            return DateAdd("DateAddMinutes", paramstr, ref errorMsg);
        }
        public string DateAdd(string funcName, string paramstr, ref string errorMsg)
        {
            string[] paramArr = paramstr.Split(',');
            string result = "";
            if (IsDateTime(paramArr[0]) && IsDigit(paramArr[1]))
            {
                switch (funcName)
                {
                    case "DateAddYears":
                        result = (DateTime.Parse(paramArr[0]).AddYears(int.Parse(paramArr[1]))).ToString("yyyy-MM-dd hh:mm:ss");
                        break;
                    case "DateAddMonths":
                        result = (DateTime.Parse(paramArr[0]).AddMonths(int.Parse(paramArr[1]))).ToString("yyyy-MM-dd hh:mm:ss");
                        break;
                    case "DateAddDays":
                        result = (DateTime.Parse(paramArr[0]).AddDays(int.Parse(paramArr[1]))).ToString("yyyy-MM-dd hh:mm:ss");
                        break;
                    case "DateAddHours":
                        result = (DateTime.Parse(paramArr[0]).AddHours(int.Parse(paramArr[1]))).ToString("yyyy-MM-dd hh:mm:ss");
                        break;
                    case "DateAddMinutes":
                        result = (DateTime.Parse(paramArr[0]).AddMinutes(int.Parse(paramArr[1]))).ToString("yyyy-MM-dd hh:mm:ss");
                        break;
                    default:
                        errorMsg = string.Format("未知的函数名{0}", funcName);
                        break;
                }
                return result;
            }
            else
            {
                ErrorMsg(paramArr[0], funcName, ref errorMsg);
                return "";
            }
        }
        #endregion


        #region 日期获取
        public string GetYear(string paramstr, ref string errorMsg)
        {
            return Get("GetYear", paramstr, ref errorMsg);
        }
        public string GetMonth(string paramstr, ref string errorMsg)
        {
            return Get("GetMonth", paramstr, ref errorMsg);
        }
        public string GetDay(string paramstr, ref string errorMsg)
        {
            return Get("GetDay", paramstr, ref errorMsg);
        }
        public string GetHour(string paramstr, ref string errorMsg)
        {
            return Get("GetHour", paramstr, ref errorMsg);
        }
        public string GetMinute(string paramstr, ref string errorMsg)
        {
            return Get("GetMinute", paramstr, ref errorMsg);
        }
        public string Get(string funcName, string paramstr, ref string errorMsg)
        {
            string result = "";
            if (IsDateTime(paramstr))
            {
                switch (funcName)
                {
                    case "GetYear":
                        result = (DateTime.Parse(paramstr).Year).ToString();
                        break;
                    case "GetMonth":
                        result = (DateTime.Parse(paramstr).Month).ToString();
                        break;
                    case "GetDay":
                        result = (DateTime.Parse(paramstr).Day).ToString();
                        break;
                    case "GetHour":
                        result = (DateTime.Parse(paramstr).Hour).ToString();
                        break;
                    case "GetMinute":
                        result = (DateTime.Parse(paramstr).Minute).ToString();
                        break;
                    default:
                        errorMsg = string.Format("未知的函数名{0}", funcName);
                        break;
                }
                return result;
            }
            else
            {
                errorMsg = string.Format("{0}的参数并不是有效的日期", funcName);
                return "";
            }
        }
        #endregion

        #region 日期之差
        public static string DateDifYears(string paramstr, ref string errorMsg)
        {
            return DateDif("DateDifYears", paramstr, ref errorMsg);
        }
        public static string GetMonthSpan(string paramstr, ref string errorMsg)
        {
            return DateDifMonths(paramstr, ref errorMsg);
        }
        public static string DateDifMonths(string paramstr, ref string errorMsg)
        {
            return DateDif("DateDifMonths", paramstr, ref errorMsg);
        }
        public static string DateDifDays(string paramstr, ref string errorMsg)
        {
            return DateDif("DateDifDays", paramstr, ref errorMsg);
        }
        public static string DateDifHours(string paramstr, ref string errorMsg)
        {
            return DateDif("DateDifHours", paramstr, ref errorMsg);
        }
        public static string DateDifMinutes(string paramstr, ref string errorMsg)
        {
            return DateDif("DateDifMinutes", paramstr, ref errorMsg);
        }
        public static string DateDif(string funcName, string paramstr, ref string errorMsg)
        {
            string[] paramArr = paramstr.Split(',');
            if (paramArr.Length != 2)
            {
                errorMsg = string.Format("{0}函数需要2个参数,当前参数为{1}个", funcName, paramArr.Length);
                return "";
            }
            try
            {
                DateTime start = DateTime.Parse(paramArr[0]);
                DateTime end = DateTime.Parse(paramArr[1]);
                TimeSpan span = end.Subtract(start);
                if (funcName == "CompareDate")
                    return DateTime.Compare(start, end).ToString();
                else if (funcName == "DateDifYears")
                    return ((Math.Abs(span.Days) / 365).ToString());
                else if (funcName == "DateDifMonths")
                    return ((Math.Abs(span.Days) / 30).ToString());
                else if (funcName == "DateDifDays")
                    return Math.Abs(span.Days).ToString();
                else if (funcName == "DateDifHours")
                    return ((int)Math.Abs(span.TotalHours)).ToString();
                else if (funcName == "DateDifMinutes")
                    return ((int)Math.Abs(span.TotalHours)).ToString();
                else
                {
                    errorMsg = "未知的函数名";
                    return "";
                }
            }
            catch
            {
                errorMsg = string.Format("请检查{0}的参数是否为有效的日期", funcName);
                return "";
            }
        }
        #endregion

        /// <summary>
        /// 预处理，将预算参数提取出来保存为单独一个字符串
        /// </summary>
        /// <param name="str"></param>
        private void Pretreatment(string str)
        {
            string dealStr = RepSymbol(str);
            MatchCollection matches = _regex.Matches(dealStr);
            foreach (Match item in matches)
            {
                _suanShiList.Add(item.Groups[0]?.Value?.Trim());
            }
        }


        /// <summary>
        /// 计算相邻逗号或逗号和左括号之间的值,即每一个括号中的参数都先计算成最简单的数据形式
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public string ComputeParm(string exp, ref string errorMsg)
        {
            string[] cmpSym = new string[] { ">=", "<=", "<>", ">", "<", "=" };//所有比较符,不能调换顺序，因为>在>=前面的话会先匹配>
            if (exp.IndexOf(":") > -1)
            {
                for (int i = 0; i < cmpSym.Length; i++)
                {
                    if (exp.IndexOf(cmpSym[i]) > -1)//是否存在比较符
                    {
                        string leftExp = exp.Substring(0, exp.IndexOf(cmpSym[i])).Trim();//比较符左参数
                        string rightExp = exp.Substring(exp.IndexOf(cmpSym[i]) + cmpSym[i].Length).Trim();//比较符右参数
                        if (!IsDateTime(leftExp) || !IsDateTime(rightExp))
                        {
                            errorMsg = "日期只能和日期进行比较";
                            return "";
                        }
                        else
                        {
                            int cmpResult = int.Parse(CompareDate(leftExp + "," + rightExp, ref errorMsg));//leftTime比rightTime大则cmpResult>0,相等则cmpResult=0,小于则cmpResult<0
                            return Compare(cmpResult, cmpSym[i]);
                        }
                    }
                }
                //循环结束没有return证明式子里面没有cmpSym里面的比较符，那么表达式只可能是单个datetime格式的字符串，直接返回即可
                return exp;
            }
            else if (exp.IndexOf("\"") > -1)//字符串比较
            {
                for (int i = 0; i < cmpSym.Length; i++)
                {
                    if (exp.IndexOf(cmpSym[i]) > -1)//是否存在比较符
                    {
                        var tmpValue = false;
                        string leftExp = exp.Substring(0, exp.IndexOf(cmpSym[i]));//比较符左参数
                        string rightExp = exp.Substring(exp.IndexOf(cmpSym[i]) + cmpSym[i].Length);//比较符右参数
                        var result = string.Compare(leftExp, rightExp);
                        if (result == 1)
                        {
                            if (i == 0 || i == 2 || i == 3)
                            {
                                tmpValue = true;
                            }
                        }
                        else if (result == 0)
                        {
                            if (i == 0 || i == 1 || i == 5)
                            {
                                tmpValue = true;
                            }
                        }
                        else if (result == -1)
                        {
                            if (i == 1 || i == 2 || i == 4)
                            {
                                tmpValue = true;
                            }
                        }
                        return tmpValue.ToString();
                    }
                }
                //循环结束没有return证明式子里面没有cmpSym里面的比较符，那么表达式只可能是单个字符串，直接返回即可
                return exp;
            }
            else//表达式里面没有时间参数
            {
                for (int i = 0; i < cmpSym.Length; i++)
                {
                    if (exp.IndexOf(cmpSym[i]) > -1)
                    {
                        string leftExp = exp.Substring(0, exp.IndexOf(cmpSym[i]));
                        string rightExp = exp.Substring(exp.IndexOf(cmpSym[i]) + cmpSym[i].Length);
                        try
                        {
                            decimal dleftExp = new MixedOperation().GetMixedOperationRes(leftExp);
                            decimal rleftExp = new MixedOperation().GetMixedOperationRes(rightExp);
                            return Compare(dleftExp - rleftExp, cmpSym[i]);
                            //leftExp = Compute(leftExp);//可能比较符前后有四则运算，那么先进行四则运算
                            //rightExp = Compute(rightExp);
                            //leftExp = leftExp.Replace("(", "").Replace(")", "");
                            //rightExp = rightExp.Replace("(", "").Replace(")", "");
                            //return Compare(decimal.Parse(leftExp) - decimal.Parse(rightExp), cmpSym[i]);
                        }
                        catch (Exception e)
                        {
                            errorMsg = string.Format("参数{0}格式错误", exp);
                            return "";
                        }
                    }
                }
                //运行到此处没有返回证明表达式没有比较符，只可能是四则运算式子或者数字
                return Compute(exp);
            }
        }
        public void PretreatmentTest(string str)
        {
            string dealStr = RepSymbol(str);
            //MatchCollection matches = _regex.Matches(dealStr);
            //foreach (Match item in matches)
            //{
            //    _suanShiList.Add(item.Groups[0]?.Value?.Trim());
            //}

            //Console.WriteLine(_suanShiList.Count(d => _regexNum.Match(d).Success));
            //Console.WriteLine(_suanShiList.Count(d => _regexOperation.Match(d).Success));
            string errorMsg = string.Empty;
            string result = ComputeParm(dealStr, ref errorMsg);
            if (string.IsNullOrEmpty(errorMsg))
            {
                Console.WriteLine(result);
                bool luoJiYSF = new LuoJiYSFOperation().GetMixedOperationRes($"!{result}");
                Console.WriteLine("逻辑非：" + luoJiYSF);
                bool luoJiYSY = new LuoJiYSFOperation().GetMixedOperationRes($"{result}&&!{result}");
                Console.WriteLine("逻辑与：" + luoJiYSY);
                bool luoJiYSH = new LuoJiYSFOperation().GetMixedOperationRes($"{result}||!{result}");
                Console.WriteLine("逻辑或：" + luoJiYSH);
                
            }
        }


        public decimal GetMixedOperationRes(string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(nameof(str));
            }
            //运算前进行一些初始化操作，清空所有的集合和堆栈
            _suanShiList.Clear();
            _suanShiZJList.Clear();
            while(_stk.Count > 0)
            {
                _stk.Pop();
            }

            //预处理表达式字符串
            Pretreatment(str);

            return 0;
        }
    }

}
