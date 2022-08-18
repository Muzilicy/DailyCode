using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleApp.DynamicQuery
{
    /// <summary>
    /// 动态表达式帮助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicExpression<T> where T:class
    {
        private ParameterExpression Parameter { get; set; }
        public DynamicExpression()
        {
            Parameter = Expression.Parameter(typeof(T));
        }

        /// <summary>
        /// 将条件列表转换为表达式
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public Expression<Func<T, bool>> Parser(IEnumerable<Condition> conditions)
        {
            //将条件列表转换为表达式body
            var query = ParsreBody(conditions);

            return Expression.Lambda<Func<T, bool>>(query, Parameter);
        }

        /// <summary>
        /// 添加排序
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public IQueryable<T> AddOrder(IQueryable<T> query, IEnumerable<Order> orders)
        {
            foreach (var order in orders)
            {
                var t = typeof(T);
                var propertyInfo = t.GetProperty(order.ZiDuanMing);
                Expression expression = Expression.Property(Parameter, propertyInfo);

                var orderby = Expression.Lambda<Func<T, object>>(expression, Parameter);
                if (order.OrderDirection == OrderDirection.DESC)
                    query = query.OrderByDescending(orderby);
                else
                    query = query.OrderBy(orderby);

            }

            return query;
        }

        /// <summary>
        /// 将条件列表转换为表达式body
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        private Expression ParsreBody(IEnumerable<Condition> conditions)
        {
            //条件为0，返回true
            if (conditions == null || conditions.Count() == 0)
            {
                return Expression.Constant(true, typeof(bool));
            }

            //1个条件
            if (conditions.Count() == 1)
            {
                return ParseCondition(conditions.First());
            }

            //多个条件递归
            Expression left = ParseCondition(conditions.First());
            Expression right = ParsreBody(conditions.Skip(1));

            return Expression.AndAlso(left, right);
        }

        /// <summary>
        /// 将条件转换为表达式body
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private Expression ParseCondition(Condition condition)
        {
            Expression key = Expression.Property(Parameter, condition.Key);
            Expression value = Expression.Constant(condition.Value);
            switch (condition.Operator)
            {
                case Operator.Contains:
                    return Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), value);
                case Operator.Equal:
                    return Expression.Equal(key, Expression.Convert(value, key.Type));
                case Operator.Greater:
                    return Expression.GreaterThan(key, Expression.Convert(value, key.Type));
                case Operator.GreaterEqual:
                    return Expression.GreaterThanOrEqual(key, Expression.Convert(value, key.Type));
                case Operator.Less:
                    return Expression.LessThan(key, Expression.Convert(value, key.Type));
                case Operator.LessEqual:
                    return Expression.LessThanOrEqual(key, Expression.Convert(value, key.Type));
                case Operator.NotEqual:
                    return Expression.NotEqual(key, Expression.Convert(value, key.Type));
                case Operator.In:
                    return ParaseIn(condition);
                default:
                    throw new NotImplementedException("不支持此操作");
            }
        }

        /// <summary>
        /// 解析In条件表达式
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private Expression ParaseIn(Condition condition)
        {
            Expression key = Expression.Property(Parameter, condition.Key);
            var valueArr = condition.Value.Split(',');
            Expression expression = Expression.Constant(true, typeof(bool));
            foreach (string val in valueArr)
            {
                Expression value = Expression.Constant(val);
                Expression right = Expression.Equal(key, Expression.Convert(value, key.Type));
                expression = Expression.Or(expression, right);
            }

            return expression;
        }
    }
}
