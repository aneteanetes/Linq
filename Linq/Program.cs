namespace Bars.NuGet.Querying
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            string[] possiblePackages = { "BarsUp.App", "BarsUp.Core" };

            var MyGet = new NuGetFeed("https://barsgroup.myget.org/F/barsup-net-core/auth/122a4baf-5686-4675-8420-3132823267c7/api/v3/index.json");

            var packages = MyGet
                .Where(x => x.Id.Contains("BarsUp"))
                .IncludePrerelease()
                .OrderBy(x => x.Id)
                .Latest()
                .Async();

            packages.Wait();
            packages.Result.

            Console.WriteLine($"Finded packages:");
            //packages.ForEach(p => Console.WriteLine(p.Id));
            Console.ReadLine();
        }
    }
}