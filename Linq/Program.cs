namespace NuGet.Querying
{
    using System;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            string[] possiblePackages = { "BarsUp.App", "BarsUp.Core" };

            var MyGet = new NuGetFeed("https://barsgroup.myget.org/F/barsup-net-core/auth/122a4baf-5686-4675-8420-3132823267c7/api/v3/index.json");

            var packages = MyGet.Where(x => x.Id.Contains("BarsUp"))
                .ToList();

            Console.WriteLine($"Finded packages:");
            packages.ForEach(p => Console.WriteLine(p.Id));
            Console.ReadLine();
        }
    }
}