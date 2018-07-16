namespace Linq
{
    using System;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string[] possiblePackages = { "BarsUp.App", "BarsUp.Core" };

            var MyGet = new NuGetFeed();
            var pdbFiles = MyGet.Where(x => possiblePackages.Contains(x.Id))
                .Select(x => x.Pdb)
                .ToList();
        }
    }
}