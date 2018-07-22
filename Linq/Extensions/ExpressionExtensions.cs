namespace Bars.NuGet.Querying
{
    using System.Linq.Expressions;

    internal static class ExpressionExtensions
    {
        public static object CompiledValue(this ConstantExpression constantExpression)
        {
            var val = constantExpression;
            
            return default;
        }

        public static T CompiledValue<T>(this ConstantExpression constantExpression)
        {


            //return (T)Expression.Lambda<Func<FrameworkName>>(Expression.PropertyOrField(node, "framework")).Compile().DynamicInvoke();

            return default;
        }
    }
}
