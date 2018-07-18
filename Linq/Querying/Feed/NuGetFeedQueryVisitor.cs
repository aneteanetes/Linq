namespace Bars.NuGet.Querying.Feed
{
    using global::Bars.Linq.Async;
    using System.Linq;
    using System.Linq.Expressions;

    internal class NuGetFeedQueryVisitor<T> : ExpressionVisitor
    {
        private IAsyncQueryable<T> queryablePlaces;

        internal NuGetFeedQueryVisitor(IAsyncQueryable<T> places)
        {
            this.queryablePlaces = places;
        }

        internal Expression CopyAndModify(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            // Replace the constant QueryableTerraServerData arg with the queryable Place collection. 
            if (c.Type == typeof(NuGetFeed))
                return Expression.Constant(this.queryablePlaces);
            else
                return c;
        }
    }
}
