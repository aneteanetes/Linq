namespace Linq
{
    using System.Linq;
    using System.Linq.Expressions;

    public class NuGetFeedQueryProvider : IQueryProvider
    {
        private readonly string root;

        public NuGetFeedQueryProvider(string root)
        {
            this.root = root;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new NuGetFeed(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)new NuGetFeed(this, expression);
        }

        public object Execute(Expression expression)
        {
            return Execute<NuGetPackage>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var isEnumerable = (typeof(TResult).Name == "IEnumerable`1");
            return (TResult)NuGetFeedQueryMaterializer.Execute(expression, isEnumerable, root);
        }
    }
}