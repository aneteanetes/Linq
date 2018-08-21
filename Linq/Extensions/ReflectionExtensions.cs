namespace Bars.NuGet.Querying
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// internal
    /// </summary>
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// Instantiate new object through expression tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="ctor"></param>
        /// <param name="argsObj"></param>
        /// <returns></returns>
        internal static T New<T>(this Type type, ConstructorInfo ctor, params object[] argsObj)
        {
            ParameterInfo[] par = ctor.GetParameters();
            Expression[] args = new Expression[par.Length];
            ParameterExpression param = Expression.Parameter(typeof(object[]));
            for (int i = 0; i != par.Length; ++i)
            {
                args[i] = Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(i)), par[i].ParameterType);
            }
            var expression = Expression.Lambda<Func<object[], T>>(
                Expression.New(ctor, args), param
            );

            var func = expression.Compile();

            return func(argsObj);
        }
    }
}