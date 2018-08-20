namespace Bars.NuGet.Querying
{
    using System;
    using System.Linq;

    public class Program
    {
        public static void Main(string[] args)
        {
            string[] possiblePackages = { "BarsUp.App", "BarsUp.Core" };

            var MyGet = new NuGetFeed(@"C:\nuget\", GetLogger(),
                "https://barsgroup.myget.org/F/barsup-net-core/auth/122a4baf-5686-4675-8420-3132823267c7/api/v3/index.json",
                "https://api.nuget.org/v3-index/index.json");


            Console.WriteLine($"Finded packages:");

            var packages = MyGet
                //.Where(x => x.Id == "BarsUp")
                .Where(x => x.Id.Contains("Bars"))
                .ForFramework(NetFramework.NetFramework, "4.5")
                .ForFramework(NetFramework.NetStandard, "2.0")
                .IncludePrerelease()
                .Latest()
                .OrderBy(x => x.Id)
                .OrderBy(x => x.Author)
                .OrderBy(x => x.Description)
                .OrderByDescending(x => x.Owner)
                .Skip(5)
                .Take(10);

            foreach (var package in packages)
            {
                Console.WriteLine($"{package.Id}\t{package.Author}\t{package.Owner}");
                Console.WriteLine("files:");
                foreach (var file in package.Items)
                {
                    Console.WriteLine(file);
                }
                Console.WriteLine("===============");
            }

            Console.WriteLine();
            Console.WriteLine("end");

            Console.ReadLine();
        }

        private static Microsoft.Extensions.Logging.ILogger GetLogger()
        {
            return new Microsoft.Extensions.Logging.Console.ConsoleLogger("any", (msg, lvl) =>
            {
                Console.WriteLine(msg);
                return true;
            }, true);
        }
    }
}