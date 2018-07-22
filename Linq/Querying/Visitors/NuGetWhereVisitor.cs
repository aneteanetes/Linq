namespace Bars.NuGet.Querying.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Bars.NuGet.Querying.Types;
    using FastMember;

    internal class NuGetWhereVisitor : NuGetVisitor
    {
        private static IEnumerable<string> availableFields;
        private static IEnumerable<string> AvailableFields
        {
            get
            {
                if (availableFields == null)
                {
                    var packageFields = TypeAccessor.Create(typeof(NuGetPackage)).GetMembers().Select(x => x.Name);
                    var filterFields = TypeAccessor.Create(typeof(NuGetQueryFilter)).GetMembers().Select(x => x.Name);

                    availableFields = packageFields
                        .Concat(filterFields)
                        .Except(new string[] { "Filter" });
                }

                return availableFields;
            }
        }

        private MethodCallExpression Where;
        private bool CanEvaluated = true;

        public NuGetWhereVisitor(NuGetQueryFilter nuGetQueryFilter) : base(nuGetQueryFilter)
        {
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Where")
            {
                Where = node;
                var whereBody = node.Arguments.Last();
                Visit(whereBody);

                if (!CanEvaluated)
                {
                    this.NotEvaluated(node);
                }

                return Where.Arguments.First();
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.NodeType == ExpressionType.Call)
            {
                CanEvaluated = true;
            }

            if (node.NodeType == ExpressionType.Equal)
            {
                CanEvaluated = true;
            }

            CanEvaluated = false;
            return base.VisitLambda(node);
        }
        
        //protected override Expression VisitMember(MemberExpression node)
        //{
        //    var member = node.Member;

        //    if (AvailableFields.Contains(member.Name))
        //    {
        //        if (CurrentMethod != null)
        //        {
        //            this.CollapseMethod = true;
        //            this.Visit<NuGetCallVisitor>(CurrentMethod);
        //        }

        //        if (CurrentUnary != null)
        //        {
        //            this.CollapseUnary = true;
        //            this.Visit<NuGetEqualsVisitor>(CurrentUnary);
        //        }
        //    }

        //    return base.VisitMember(node);
        //}
    }
}