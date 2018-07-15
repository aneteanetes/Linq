using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Linq
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string[] possiblePackages = { "BarsUp.App", "BarsUp.Core" };

            var MyGet = new NuGetFeed<NuGetPackage>();
            var pdbFiles = MyGet.Where(x => possiblePackages.Contains(x.Id))
                .Select(x => x.Pdb)
                .ToList();

            

            //var db = new Data
            //{
            //    Title="Root",
            //    Grade=0,
            //    Obj=new List<>
            //}

            //var query = new 
        }
    }

    public class NuGetPackage : INuGetPackage
    {
        public string Id { get; set; }

        public Stream Pdb => Stream.Null;
    }

    public class ProviderScope
    {
        private static IQueryProvider current;
        public static IQueryProvider CurrentProvider
        {
            get
            {
                if (current == null)
                    current = new NuGetQueryProvider();

                return current;
            }
        }
    }

    public class NuGetFeed<TNuGetPackage> : IQueryable<TNuGetPackage> where TNuGetPackage : INuGetPackage
    {
        public List<INuGetPackage> InternalFeed { get; set; }

        public Type ElementType => typeof(Exception);

        public Expression Expression => Expression.Lambda(Expression.PropertyOrField(Expression.Constant(this), "InternalFeed");

        public IQueryProvider Provider => ProviderScope.CurrentProvider;

        public IEnumerator<TNuGetPackage> GetEnumerator()
        {
            Debugger.Break();throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Debugger.Break();throw new NotImplementedException();
        }
    }

    public class NuGetQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            Debugger.Break(); throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            Debugger.Break();throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            Debugger.Break();throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            Debugger.Break();throw new NotImplementedException();
        }
    }

    public interface INuGetPackage
    {
        string Id { get; }

        Stream Pdb { get; }
    }
}