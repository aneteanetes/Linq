namespace Bars.NuGet.Querying.Feed
{
    using global::Bars.NuGet.Querying.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// internal
    /// </summary>
    internal class NuGetFeedQueryProvider : IQueryProvider
    {
        private readonly NuGetRepository NuGetRepository;

        public NuGetFeedQueryProvider(NuGetRepository nuGetRepository)
        {
            this.NuGetRepository = nuGetRepository;
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
            return Execute<IEnumerator<NuGetPackage>>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            if (!IsSupported<TResult>())
                throw new ArgumentException($"The type argument - {typeof(TResult)}, is not supported by {nameof(NuGetFeedQueryProvider)}.");

            return (TResult)NuGetFeedQueryMaterializer.Execute(expression, NuGetRepository);
        }

        private bool IsSupported<T>() => typeof(IEnumerator<NuGetPackage>).IsAssignableFrom(typeof(T));
    }
}