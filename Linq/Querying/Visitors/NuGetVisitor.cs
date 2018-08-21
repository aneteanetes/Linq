namespace Bars.NuGet.Querying.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Versioning;
    using Bars.NuGet.Querying.Types;

    /// <summary>
    /// internal
    /// </summary>
    internal class NuGetVisitor : ExpressionVisitor
    {
        protected List<Func<Expression, Expression>> appliers = new List<Func<Expression, Expression>>();
        protected NuGetQueryFilter nuGetQueryFilter;

        internal NuGetQueryFilter GetNuGetQueryFilter() => nuGetQueryFilter;
        
        public NuGetVisitor(NuGetQueryFilter nuGetQueryFilter)
        {
            this.nuGetQueryFilter = nuGetQueryFilter;
        }
        
        public Expression Visit<TVisitor, TCast>(Expression node, Func<TCast, Expression> func)
            where TVisitor : NuGetVisitor
            where TCast : Expression
        {
            var cast = Visit<TVisitor>(node) as TCast;
            return func(cast);
        }

        public Expression Visit<TVisitor>(Expression node)
            where TVisitor : NuGetVisitor
        {
            var internalCtor = typeof(TVisitor).GetConstructors().FirstOrDefault();

            var newVisitor = typeof(TVisitor).New<TVisitor>(internalCtor, this.nuGetQueryFilter);
            var visit = newVisitor.Visit(node);
            this.appliers.AddRange(newVisitor.appliers);
            return visit;
        }

        /// <summary>
        /// If method can't be evaluated for NuGetFeed, we can use this later with SyncIncompatibility mode
        /// </summary>
        /// <param name="methodCallExpression"></param>
        protected void NotEvaluated(MethodCallExpression methodCallExpression)
        {
            this.AddApplier(x => Expression.Call(methodCallExpression.Method, x, methodCallExpression.Arguments.Last()));
        }

        protected void AddApplier(Func<Expression, Expression> func) => this.appliers.Add(func);
    }
}