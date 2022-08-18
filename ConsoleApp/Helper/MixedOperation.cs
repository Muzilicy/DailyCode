using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp.Helper
{
    /// <summary>
    /// 四则运算计算器
    /// </summary>
    public class MixedOperation
    {
        /// <summary>
        /// 用于匹配运算符和参数的正则表达式
        /// </summary>
        private readonly Regex _regex = new Regex(@"(^-|(?<=\D)-)?[\d.]+|[+\-*\/%!()√^]|log");
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

        public MixedOperation()
        {
            _prio.Add("(", 0); //左括号的优先级最低，遇到左括号直接入栈
            _prio.Add(")", 0); //不会用到右括号的优先级，但是右括号不可能入栈，再次只把右括号当做一个运算符


            //双目运算符
            _prio.Add("+", 1); //加法优先级为1
            _prio.Add("-", 1); //减法优先级为1
            _prio.Add("*", 2); //乘法优先级为2
            _prio.Add("/", 2); //除法优先级为2

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
            foreach (var item in repSymbol)
            {
                exp = exp.Replace(item.Key, item.Value);
            }
            return exp;
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
        /// 比如输入为1+5*7,转为后缀表达式157*+
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
                    //如果栈为空或者是左括号，直接入栈,为什么左括号也要直接入栈? 考虑到括号嵌套问题，如6+((1+2)*3+4)*5
                    if (_stk.Count == 0 || item == "(")
                    {
                        _stk.Push(item);
                    }
                    else if (item == ")") //遇到右括号，将栈顶元素一直出栈知道遇到左括号为止，并将该左括号出栈
                    {
                        while (_stk.Count != 0 && _stk.Peek() != "(")
                        {
                            _suanShiZJList.Add(_stk.Pop());
                        }
                        if(_stk.Count != 0)
                        {
                            _stk.Pop();//将左括号出栈
                        }
                    }
                    else if (_prio.ContainsKey(item) &&
                             _prio.ContainsKey(peek) &&
                             peek != null &&
                             _prio[item] > _prio[peek])
                    {
                        // 如果该运算符优先级高于栈顶运算符的优先级，
                        // 直接入栈
                        _stk.Push(item);
                    }
                    else
                    {
                        // 该运算符优先级低于或等于栈顶运算符优先级，
                        // 分四种情况
                        while(_stk.Count > 0 &&
                              _stk.Peek() != "(" &&
                             _prio.ContainsKey(item) &&
                             _prio.ContainsKey(peek) &&
                             _prio[item] <= _prio[peek])
                        {
                            _suanShiZJList.Add(_stk.Pop());

                        }
                        _stk.Push(item);
                    }
                }
                else
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
            decimal d1, d2;
            decimal d3 = decimal.Zero;

            for (int i = 0; i < _suanShiZJList.Count; i++)
            {
                string item = _suanShiZJList[i];
                if(!_prio.ContainsKey(item)) //对参数进行处理
                {
                    _stk.Push(item); //参数直接入栈
                }
                else //对运算符进行处理
                {
                    string p1 = _stk.Pop();
                    string p2 = _stk.Pop();
                    d1 = Convert.ToDecimal(p1);
                    d2 = Convert.ToDecimal(p2);

                    switch (item)
                    {
                        case "+":
                            d3 = d2 + d1;
                            break;
                        case "-":
                            d3 = d2 - d1;
                            break;
                        case "*":
                            d3 = d2 * d1;
                            break;
                        case "/":
                            if(d1 == 0)
                            {
                                throw new ArgumentException("公式中存在0");
                            }
                            d3 = d2 / d1;
                            break;
                        default:
                            d3 = decimal.Zero;
                            break;
                    }

                    _stk.Push(d3.ToString());
                }
            }

            return _stk.Pop();
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

            //中缀表达式转成后缀表达式
            Infix2Postfix();

            //计算后缀表达式
            string result = PostfixCalculate();

            decimal.TryParse(result,out decimal res);

            return res;
        }
    }

}
