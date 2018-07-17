namespace Bars.NuGet.Querying
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using global::Bars.NuGet.Querying.Feed;
    using global::Bars.NuGet.Querying.Types;

    public class NuGetFeed : IQueryable<NuGetPackage>
    {
        public NuGetFeed(params string[] feeds)
        {
            Provider = new NuGetFeedQueryProvider(feeds);
            Expression = Expression.Constant(this);
        }

        internal NuGetFeed(IQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public IEnumerator<NuGetPackage> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<NuGetPackage>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Type ElementType
        {
            get { return typeof(NuGetPackage); }
        }

        public Expression Expression { get; private set; }
        public IQueryProvider Provider { get; private set; }

        internal NuGetQueryFilter CurrentFilter = new NuGetQueryFilter();
    }
}