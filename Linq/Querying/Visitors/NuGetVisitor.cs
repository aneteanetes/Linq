namespace Bars.NuGet.Querying.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Versioning;
    using Bars.NuGet.Querying.Types;

    internal class NuGetVisitor : ExpressionVisitor
    {
        protected List<Func<Expression, Expression>> appliers = new List<Func<Expression, Expression>>();
        protected NuGetQueryFilter nuGetQueryFilter;
        internal NuGetQueryFilter GetNuGetQueryFilter()
        {
            if (this.nuGetQueryFilter.SupportedFrameworks == null)
            {
                this.ForFramework();
            }

            return nuGetQueryFilter;
        }
        
        public NuGetVisitor(NuGetQueryFilter nuGetQueryFilter)
        {
            this.nuGetQueryFilter = nuGetQueryFilter;
        }

        private void ForFramework()
        {
            var framework = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName;

            if (framework == null)
                throw new InvalidFilterCriteriaException("No one of framework version was chosen, and can't indicate current version of framework, use .ForFramework() method of query to add versions of frameworks applicabel to packages");

            var splitted = framework.Split(",");

            var version = new Version(splitted[1].Replace("Version=v", ""));
            var frameworkName = new FrameworkName(splitted[0].Replace(".", "").ToLowerInvariant(), version);

            nuGetQueryFilter.SupportedFrameworks = new List<FrameworkName>() { frameworkName };
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
        /// this method can't be evaluated for NuGetFeed, so, we can use this later, if mode will be SyncAvailable
        /// </summary>
        /// <param name="methodCallExpression"></param>
        protected void NotEvaluated(MethodCallExpression methodCallExpression)
        {
            this.AddApplier(x => Expression.Call(methodCallExpression.Method, x, methodCallExpression.Arguments.Last()));
        }

        protected void AddApplier(Func<Expression, Expression> func) => this.appliers.Add(func);
    }
}