namespace Bars.NuGet.Querying.Visitors
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Bars.NuGet.Querying.Types;

    internal class NuGetOrderByVisitor : NuGetVisitor
    {
        private string method = "";
        internal static string[] ApplicableOrderByMethods = { "OrderBy", "OrderByDescending" };
        private IdOrderRule idOrderRule = IdOrderRule.None;

        public NuGetOrderByVisitor(NuGetQueryFilter nuGetQueryFilter) : base(nuGetQueryFilter)
        {
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            this.method = node.Method.Name;

            var args = node.Arguments;
            var orderBy = args.Last();
            this.Visit(orderBy);

            //if visited member and not find - return original
            if (idOrderRule != IdOrderRule.None)
            {
                this.nuGetQueryFilter.OrderById = this.idOrderRule;
                idOrderRule = IdOrderRule.None;
            }
            else
            {
                this.NotEvaluated(node);
            }

            return args.First();
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (ApplicableOrderByMethods.Contains(this.method))
            {
                var memeber = node.Member;

                if (memeber.Name == "Id" && typeof(NuGetPackageInfo).IsAssignableFrom(memeber.ReflectedType))
                {
                    idOrderRule = method == "OrderBy"
                        ? IdOrderRule.Asc
                        : IdOrderRule.Desc;
                }
            }

            return base.VisitMember(node);
        }
    }
}
