namespace Bars.NuGet.Querying.Visitors
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Versioning;
    using Bars.NuGet.Querying.Types;

    internal class NuGetWhereVisitor : NuGetVisitor
    {
        public NuGetWhereVisitor(NuGetQueryFilter nuGetQueryFilter) : base(nuGetQueryFilter)
        {
        }

        //protected override Expression VisitLambda<T>(Expression<T> node)
        //{
        //    var body = node.Body;
        //    if (body.NodeType == ExpressionType.Equal)
        //    {
        //        return this.Visit<NuGetEqualsVisitor>(node);
        //    }

        //    if (body.NodeType == ExpressionType.Call)
        //    {
        //        return this.Visit<NuGetCallVisitor>(node);
        //    }

        //    return Expression.Lambda<Func<NuGetPackage, bool>>(Expression.Constant(true), node.Parameters);
        //}

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var args = node.Arguments;

            var targetMethod = args.Last();

            this.Visit(targetMethod);

            return args.First();
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var member = node.Member;
            if (typeof(NuGetPackageInfo).IsAssignableFrom(member.ReflectedType))
            {
                //
            }

            if (member.MemberType() == typeof(FrameworkName))
            {
                Debugger.Break();
            }

            return base.VisitMember(node);
        }

        private bool ReadNeedToRemove(Expression node)
        {
            return (bool)((node as UnaryExpression).Operand as LambdaExpression).Compile().DynamicInvoke(new NuGetPackage());
        }
    }
}