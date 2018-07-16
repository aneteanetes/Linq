namespace Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    public class NuGetFeedQueryMaterializer
    {
        internal static object Execute(Expression expression, bool isEnumerable, string root)
        {
            var queryableElements = GetAllFilesAndFolders(root);

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

        private static IQueryable<NuGetPackage> GetAllFilesAndFolders(string root)
        {
            return Enumerable.Empty<NuGetPackage>().AsQueryable();

            //var list = new List<NuGetPackage>();
            //foreach (var directory in Directory.GetDirectories(root))
            //{
            //    list.Add(new FolderElement(directory));
            //}
            //foreach (var file in Directory.GetFiles(root))
            //{
            //    list.Add(new FileElement(file));
            //}
            //return list.AsQueryable();
        }
    }
}
