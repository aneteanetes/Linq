namespace Bars.NuGet.Querying.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Bars.NuGet.Querying.Types;

    internal class NuGetExpressionVisitor : ExpressionVisitor
    {
        NuGetQueryFilter nuGetQueryFilter = new NuGetQueryFilter();

        private Dictionary<ExpressionType, Action<Expression>> BodyProcessor = new Dictionary<ExpressionType, Action<Expression>>();

        public NuGetExpressionVisitor()
        {
            this.BodyProcessor.Add(ExpressionType.Call, Call);
            this.BodyProcessor.Add(ExpressionType.Equal, Compare);
            this.BodyProcessor.Add(ExpressionType.NotEqual, Compare);
        }


        internal new(Expression expression, NuGetQueryFilter filter) Visit(Expression node) => (base.Visit(node), this.nuGetQueryFilter);

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.ReturnType == typeof(bool))
            {
                return RewriteWhere(node);
            }

            return base.VisitLambda(node);
        }

        private Expression RewriteWhere(LambdaExpression expression)
        {
            var body = expression.Body;
            if (BodyProcessor.TryGetValue(body.NodeType, out var bodyProcessor))
                bodyProcessor(body);

            return Expression.Lambda<Func<NuGetPackage, bool>>(Expression.Constant(true), expression.Parameters);
        }

        private void Call(Expression expression)
        {

        }

        private void Compare(Expression expression)
        {
            var binary = expression as BinaryExpression;
            var property = GetLeft(binary.Left);
            var value = GetRight(binary.Right);

            BindProperty(property, binary.Left, value);            
        }

        private string GetLeft(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return (expression as MemberExpression).Member.Name;
            }

            return string.Empty;
        }

        private object GetRight(Expression expression)
        {
            if(expression.NodeType== ExpressionType.Constant)
            {
                return (expression as ConstantExpression).Value;
            }

            return default;
        }

        private void BindProperty(string leftProperty, Expression left, object value)
        {
            string[] internalBools = { "IncludePrerelease", "IncludeDelisted", "Latest" };
            if (internalBools.Contains(leftProperty))
            {
                (left as MemberExpression).Member.SetValue(this.nuGetQueryFilter, value);
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }

        //protected override Expression VisitMember(MemberExpression node)
        //{
        //    return base.VisitMember(node);
        //}
    }
}