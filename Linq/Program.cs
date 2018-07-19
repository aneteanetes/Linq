namespace Bars.NuGet.Querying
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Bars.Linq.Async;
    using global::NuGet.Protocol;
    using global::NuGet.Protocol.Core.Types;

    class Program
    {
        static void Main(string[] args)
        {
            string[] possiblePackages = { "BarsUp.App", "BarsUp.Core" };

            var MyGet = new NuGetFeed("https://barsgroup.myget.org/F/barsup-net-core/auth/122a4baf-5686-4675-8420-3132823267c7/api/v3/index.json", "https://api.nuget.org/v3-index/index.json");

            var packages = MyGet
                .Where(x => x.Id.Contains("BarsUp"))
                .IncludePrerelease()
                .OrderBy(x => x.Id)
                .ToAsync();

            Console.WriteLine($"Finded packages:");

            var enumerator = packages.GetAsyncEnumerator();
            while (moveNext(enumerator))
            {
                var currentTask = enumerator.CurrentAsync;
                currentTask.Wait();
                Console.WriteLine(currentTask.Result?.Id ?? string.Empty);
            }

            Console.WriteLine("нихуящечки");

            //packages.Result.ForEach(p => Console.WriteLine(p.Id));
            Console.ReadLine();
        }

        private static bool moveNext(IAsyncEnumerator<NuGetPackage> asyncEnumerator)
        {
            var task = asyncEnumerator.MoveNext();
            task.Wait();
            return task.Result;
        }

        private static void test()
        {
            //var repo = Repository.Factory.GetCoreV2(new global::NuGet.Configuration.PackageSource("https://barsgroup.myget.org/F/barsup-net-core/auth/122a4baf-5686-4675-8420-3132823267c7/api/v2"));
            //var filter = new SearchFilter(true);
            //var task =  repo.GetResource<PackageSearchResource>().SearchAsync("Bars", filter, 0, 10, new global::NuGet.Common.NullLogger(), CancellationToken.None);
            //task.Wait();
            //task.Result.ToList().ForEach(x =>
            //{
            //    Console.WriteLine(x.Identity.Id);
            //});
        }
    }
}