namespace Bars.NuGet.Querying.Functionality
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Bars.NuGet.Querying.Patches;

    public static class Binder
    {
        internal static IEnumerable<TResult> Bind<TSource, TResult>(this IEnumerable<TSource> sources, Func<TSource, TResult> command)
        {
            var result = new List<TResult>();

            foreach (var source in sources)
            {
                var commandResult = command(source);
                result.Add(commandResult);
            }

            return result;
        }

        internal static TResult BindMany<TSource, TResult>(this IEnumerable<TSource> sources, Func<TSource, TResult> command, Func<IEnumerable<TResult>, TResult> merge)
        {
            return merge(Bind(sources, command));
        }

        internal static IEnumerable<TResult> BindParams<TSource, TParams, TResult>(this IEnumerable<TSource> sources, Func<TSource, TParams> paramExtract, Func<TSource, TParams, TResult> command)
        {
            var result = new List<TResult>();

            foreach (var source in sources)
            {
                var @params = paramExtract(source);
                var commandResult = command(source, @params);
                result.Add(commandResult);
            }

            return result;
        }

        internal static TResult BindManyParams<TSource, TParams, TResult>(this IEnumerable<TSource> sources, Func<TSource, TParams> paramExtract, Func<TSource, TParams, TResult> command, Func<IEnumerable<TResult>, TResult> merge)
        {
            return merge(BindParams(sources, paramExtract, command));
        }
        
        internal static Task<IEnumerable<T>> TaskCombine<T>(IEnumerable<Task<T>> results) => TaskPatch.WhenAll(results);

        internal static Task<IEnumerable<T>> TaskMerge<T>(Task<IEnumerable<IEnumerable<T>>> task) => task.ContinueWith(t => Apply.Collection(t.Result));
    }
}