namespace Bars.NuGet.Querying.Visitors
{
    using System.Linq;
    using System.Linq.Expressions;
    using Bars.NuGet.Querying.Types;

    /// <summary>
    /// internal
    /// </summary>
    internal class NuGetSkipTakeVisitor : NuGetVisitor
    {
        private int count;

        public NuGetSkipTakeVisitor(NuGetQueryFilter nuGetQueryFilter) : base(nuGetQueryFilter)
        {
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            this.EnsureCount(node);

            if (node.Method.Name == "Skip")
            {
                this.nuGetQueryFilter.Skip = this.count;
            }
            else
            {
                this.nuGetQueryFilter.Take = this.count;
            }

            return node.Arguments.First();
        }

        protected void EnsureCount(MethodCallExpression node)
        {
            var conuntexpr = node.Arguments.Last();
            this.Visit(conuntexpr);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            this.count = (int)node.Value;
            return base.VisitConstant(node);
        }
    }
}