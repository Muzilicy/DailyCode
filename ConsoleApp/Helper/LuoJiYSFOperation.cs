using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp.Helper
{
    /// <summary>
    /// 逻辑运算符计算器
    /// </summary>
    public class LuoJiYSFOperation
    {
        /// <summary>
        /// 用于匹配逻辑运算符和true false的正则表达式
        /// </summary>
        private readonly Regex _regex = new Regex(@"(^-|(?<=\D)-)?[\d.]+|[!()]|([Tt][Rr][Uu][Ee]|[Ff][Aa][Ll][Ss][Ee])|([&][&])|([|][|])");
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

        public LuoJiYSFOperation()
        {
            _prio.Add("!", 3); //单目逻辑运算符
            _prio.Add("&&", 2); //双目逻辑运算符
            _prio.Add("||", 1); //双目逻辑运算符

            _prio.Add("(", 0); //左括号的优先级最低，遇到左括号直接入栈
            _prio.Add(")", 0); //不会用到右括号的优先级，但是右括号不可能入栈，再次只把右括号当做一个运算符


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
            repSymbol.Add("！", "!");
            repSymbol.Add("&&", "&&");
            foreach (var item in repSymbol)
            {
                exp = exp.Replace(item.Key, item.Value);
            }
            return exp;
        }

        public void PretreatmentTest(string str)
        {
            string dealStr = RepSymbol(str);
            MatchCollection matches = _regex.Matches(dealStr);
            foreach (Match item in matches)
            {
                Console.WriteLine(item.Groups[0]?.Value?.Trim());
            }
        }
        /// <summary>
        /// 预处理，将预算参数提取出来保存为单独一个字符串
        /// 比如输入"1.23+4.56",整体为一个字符串，经过处理后被分隔成三个字符串"1.23","+","4.56"
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
        /// 中缀表达式转后缀表达式
        /// </summary>
        private void Infix2Postfix()
        {
            for (int i = 0; i < _suanShiList.Count; i++)
            {
                string item = _suanShiList[i];
                //对运算符进行处理
                if (_prio.ContainsKey(item))
                {
                    string peek = _stk.Count > 0 ? _stk.Peek() : null;
                    //如果栈为空或者是左括号、非运算符，直接入栈,为什么左括号也要直接入栈? 考虑到括号嵌套问题
                    if (_stk.Count == 0 || item == "(" || item == "!")
                    {
                        _stk.Push(item);
                    }
                    else if (item == "&&") //当前元素为&与运算符
                    {
                        //若字符串为&，则判断栈顶元素是否为!,若是，则开始弹出
                        if (_stk.Count > 0)
                        {
                            if(peek == "!") 
                            {
                                if (!_stk.Contains("(")) //栈中没有左括号(,则将栈中所有的!弹出
                                {
                                    string aPeek = _stk.Pop();
                                    _suanShiZJList.Add(aPeek);
                                    while (_stk.Count > 0 && _stk.Peek() == "!") //此时栈可能为空,判断是否为空
                                    {
                                        aPeek = _stk.Pop();
                                        _suanShiZJList.Add(aPeek);
                                    }
                                }
                                else //栈中有左括号
                                {
                                    string bPeek = _stk.Pop();
                                    while(bPeek == "(")
                                    {
                                        _suanShiZJList.Add(bPeek);
                                        if(_stk.Count == 0)
                                        {
                                            break;
                                        }
                                        bPeek = _stk.Pop();
                                    }
                                    _stk.Push("(");
                                }
                            }
                            _stk.Push(item);

                        }
                    }
                    else if (item == "||")
                    {
                        if( peek == "!" || peek == "&&")
                        {
                            if(!_stk.Contains("(")) // 栈中没有(,将栈中所有操作符弹出，添加到_suanShiZJList中
                            {
                                while (_stk.Count > 0)
                                {
                                    _suanShiZJList.Add(_stk.Pop());
                                }
                            }
                            else
                            {
                                string cPeek = _stk.Pop();
                                while(cPeek != "(")
                                {
                                    _suanShiZJList.Add(cPeek);
                                    if(_stk.Count == 0)
                                    {
                                        break;
                                    }
                                    cPeek = _stk.Pop();
                                }
                                _stk.Push("(");
                            }
                        }

                        _stk.Push(item);
                    }
                    else if(item == ")") //若为),将栈中(前的所有操作符弹出,添加到_suanShiZJList中,(也弹出，但是不添加到栈中
                    {
                        string dPeek = _stk.Pop();
                        while (dPeek != "(")
                        {
                            _suanShiZJList.Add(dPeek);
                            if (_stk.Count == 0)
                            {
                                break;
                            }
                            dPeek = _stk.Pop();
                        }
                    }
                }
                else //数字或者 true、false
                {
                    _suanShiZJList.Add(item);
                }
            }

            while(_stk.Count > 0)
            {
                _suanShiZJList.Add(_stk.Pop());
            }
        }

        /// <summary>
        /// 对后缀表达式计算
        /// </summary>
        /// <returns></returns>
        private string PostfixCalculate()
        {
            for (int i = 0; i < _suanShiZJList.Count; i++)
            {
                string item = _suanShiZJList[i];
                if(!_prio.ContainsKey(item)) //对参数进行处理
                {
                    _stk.Push(item); //参数直接入栈
                }
                else //对运算符进行处理
                {
                    string result = string.Empty;
                    string p1, p2;
                    switch (item)
                    {
                        case "&&"://若为“&”操作符，从栈中弹出两个数字进行与运算，并将结果保存在在栈中
                            if(_stk.Count < 2)
                            {
                                throw new Exception("表达式异常-运算符&&");
                            }
                            p1 = _stk.Pop();
                            p2 = _stk.Pop();
                            bool b4 = (decimal.TryParse(p1,out decimal p3) && p3 != 0) || (bool.TryParse(p1, out bool b3) && b3);
                            bool b6 = (decimal.TryParse(p2, out decimal p5) && p5 != 0) || (bool.TryParse(p2, out bool b5) && b5);
                            result = b4 && b6 ? "1" : "0";
                            break;
                        case "||"://若为“|”操作符，从栈中弹出两个数字进行或运算，并将结果保存在在栈中
                            if (_stk.Count < 2)
                            {
                                throw new Exception("表达式异常-运算符||");
                            }
                            p1 = _stk.Pop();
                            p2 = _stk.Pop();
                            bool b8 = (decimal.TryParse(p1, out decimal p7) && p7 != 0) || (bool.TryParse(p1, out bool b7) && b7);
                            bool b10 = (decimal.TryParse(p2, out decimal p9) && p9 != 0) || (bool.TryParse(p2, out bool b9) && b9);
                            result = b8 || b10 ? "1" : "0";
                            break;
                        case "!"://# 若为“！”操作符，从栈中弹出一个数字进行非运算，并将结果保存在在栈中
                            if (_stk.Count < 1)
                            {
                                throw new Exception("表达式异常!");
                            }
                            p1 = _stk.Pop();
                            bool b12 = (decimal.TryParse(p1, out decimal p11) && p11 != 0) || (bool.TryParse(p1, out bool b11) && b11);
                            result = !b12 ? "1" : "0";
                            break;
                        default:
                            break;
                    }

                    _stk.Push(result);
                }
            }

            return _stk.Pop();
        }

        public bool GetMixedOperationRes(string str)
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

            //中缀表达式转成后缀表达式
            Infix2Postfix();

            //计算后缀表达式
            string result = PostfixCalculate();

            return result == "1" || result == "true";
        }
    }

}
