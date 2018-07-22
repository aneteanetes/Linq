namespace Bars.NuGet.Querying.Visitors
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Bars.NuGet.Querying.Types;
    using FastMember;

    internal class NuGetMemberVisitor : NuGetVisitor
    {
        public string PropertyName;

        static Lazy<string[]> PossibleProperties => new Lazy<string[]>(() => TypeAccessor.Create(typeof(NuGetPackageInfo)).GetMembers().Select(x => x.Name).ToArray());

        public NuGetMemberVisitor(NuGetQueryFilter nuGetQueryFilter) : base(nuGetQueryFilter)
        {
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var member = node.Member;
            if (typeof(NuGetPackageInfo).IsAssignableFrom(member.ReflectedType))
            {
                PropertyName = member.Name;
            }

            return base.VisitMember(node);
        }
    }
}
