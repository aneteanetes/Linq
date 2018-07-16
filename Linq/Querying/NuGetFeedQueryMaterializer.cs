﻿namespace NuGet.Querying.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using NuGet.Querying.Client;

    internal class NuGetFeedQueryMaterializer
    {
        internal static object Execute(Expression expression, bool isEnumerable, IEnumerable<string> feeds)
        {
            var queryableElements = Root(feeds);

            // Copy the expression tree that was passed in, changing only the first
            // argument of the innermost MethodCallExpression.
            var treeCopier = new NuGetFeedQueryVisitor(queryableElements);
            var newExpressionTree = treeCopier.Visit(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods.
            if (isEnumerable)
            {
                return queryableElements.Provider.CreateQuery(newExpressionTree);
            }
            else
            {
                return queryableElements.Provider.Execute(newExpressionTree);
            }
        }

        private static IQueryable<NuGetPackage> Root(IEnumerable<string> feeds)
        {
            var repo = new NuGetRepository(feeds);

            var list = new List<NuGetPackage>();

            return null;

            //return repo.SearchMeta().Select(x => new NuGetPackage
            //{
            //    Id = x.Identity.Id
            //}).AsQueryable();            
        }
    }
}