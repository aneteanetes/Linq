namespace Bars.NuGet.Querying.Visitors
{
    using Bars.NuGet.Querying.Types;
    using FastMember;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Versioning;

    /// <summary>
    /// Remove all <see cref="Queryable.Where{TSource}(System.Linq.IQueryable{TSource}, System.Linq.Expressions.Expression{System.Func{TSource, bool}})" />
    /// if it is applicable.
    /// If not : apply what can, but adds to <see cref="NuGetQueryExtensions.SyncIncompatibility(System.Linq.IQueryable{NuGetPackage})"/>
    /// internal
    /// </summary>
    internal class NuGetWhereVisitor : NuGetVisitor
    {
        private static TypeAccessor QueryFilterAccessor = TypeAccessor.Create(typeof(NuGetQueryFilter));

        private static IEnumerable<string> nuGetPackageAvailableFields;
        private static IEnumerable<string> NuGetPackageAvailableFields
        {
            get
            {
                if (nuGetPackageAvailableFields == null)
                {
                    nuGetPackageAvailableFields = TypeAccessor.Create(typeof(NuGetPackage)).GetMembers().Select(x => x.Name)
                        .Except(new string[] { "Filter" });
                }
                return nuGetPackageAvailableFields;
            }
        }

        private static IEnumerable<string> queryFilterAvailableFields;
        private static IEnumerable<string> QueryFilterAvailableFields
        {
            get
            {
                if (queryFilterAvailableFields == null)
                {
                    queryFilterAvailableFields = TypeAccessor.Create(typeof(NuGetQueryFilter)).GetMembers().Select(x => x.Name);
                }
                return queryFilterAvailableFields;
            }
        }
        
        private static IEnumerable<string> AvailableFields
        {
            get
            {
                return QueryFilterAvailableFields
                        .Concat(NuGetPackageAvailableFields);
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

            if (node.Method.Name == "Contains")
            {
                this.VisitContains(node);
                return node;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var body = node.Body;

            if (body.NodeType == ExpressionType.Call)
            {
                if (body is MethodCallExpression callBody)
                {
                    if (callBody.Method.Name == "Contains")
                    {
                        this.VisitContains(callBody);
                        return node;
                    }
                }
                else
                {
                    throw CantParseExpression(body);
                }
            }

            if (body.NodeType == ExpressionType.Equal)
            {
                if (body is BinaryExpression binaryBody)
                {
                    this.VisitEqual(binaryBody);
                    return node;
                }
                else
                {
                    throw CantParseExpression(body);
                }
            }

            CanEvaluated = false;
            return base.VisitLambda(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Equal)
            {
                this.VisitEqual(node);
            }

            return base.VisitBinary(node);
        }

        private void VisitEqual(BinaryExpression node)
        {
            #region left

            string propertyName = string.Empty;

            if (node.Left is MemberExpression memberNode)
            {
                propertyName = memberNode.Member.Name;
            }
            else
            {
                CanEvaluated = false;
                return;
            }            

            if (!AvailableFields.Contains(propertyName))
            {
                CanEvaluated = false;
                return;
            }

            #endregion

            #region right + result

            try
            {
                object value = Expression.Lambda(node.Right).Compile().DynamicInvoke();

                if (QueryFilterAvailableFields.Contains(propertyName))
                {
                    QueryFilterAccessor[this.nuGetQueryFilter, propertyName] = value;
                }
                else
                {
                    if (this.nuGetQueryFilter.Filter.TryGetValue(propertyName, out string current))
                    {
                        this.nuGetQueryFilter.Filter[propertyName] = (value as string);
                    }
                    else
                    {
                        this.nuGetQueryFilter.Filter.Add(propertyName, (value as string));
                    }
                }
            }
            catch
            {
                CanEvaluated = false;
            }

            #endregion
        }

        private void VisitContains(MethodCallExpression node)
        {
            MemberInfo property = null;

            #region left

            if (node.Object is MemberExpression memberNode)
            {
                property = memberNode.Member;
            }
            else
            {
                CanEvaluated = false;
                return;
            }

            if (!AvailableFields.Contains(property.Name))
            {
                CanEvaluated = false;
                return;
            }

            #endregion

            #region right

            object value = null;

            try
            {
                value = Expression.Lambda(node.Arguments[0]).Compile().DynamicInvoke();
            }
            catch
            {
                CanEvaluated = false;
                return;
            }

            #endregion

            #region process

            //рефактор потом, сейчас ночь бл*

            string tag = "Tags";

            if (property.Name == "SupportedFrameworks")
            {
                this.nuGetQueryFilter.SupportedFrameworks.Add(value as FrameworkName);
            }
            else if (property.Name == tag)
            {
                if (this.nuGetQueryFilter.Filter.TryGetValue(tag, out string currentTags))
                {
                    this.nuGetQueryFilter.Filter[tag] = currentTags + " " + (value as string);
                }
                else
                {
                    this.nuGetQueryFilter.Filter.Add(tag, (value as string));
                }
            }
            else
            {
                if (this.nuGetQueryFilter.Filter.TryGetValue(property.Name, out string current))
                {
                    this.nuGetQueryFilter.Filter[property.Name] = current + " " + (value as string);
                }
                else
                {
                    this.nuGetQueryFilter.Filter.Add(property.Name, (value as string));
                }
            }

            #endregion
        }

        private Exception CantParseExpression(Expression node)
        {
            return new Exception($"This expression can't be parsed for nuget query: {node.ToString()}");
        }
    }
}