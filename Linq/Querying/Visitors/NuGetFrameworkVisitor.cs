namespace Bars.NuGet.Querying.Visitors
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.Versioning;
    using Bars.NuGet.Querying.Types;

    internal class NuGetFrameworkVisitor : NuGetVisitor
    {
        public NuGetFrameworkVisitor(NuGetQueryFilter nuGetQueryFilter) : base(nuGetQueryFilter)
        {
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var property = Expression.PropertyOrField(node, "framework");
            var lambda = Expression.Lambda<Func<FrameworkName>>(property);
            var frameworkName = (FrameworkName)lambda.Compile().DynamicInvoke();
            this.nuGetQueryFilter.SupportedFrameworks.Add(frameworkName);

            return base.VisitConstant(node);
        }
    }
}
