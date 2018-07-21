namespace Bars.NuGet.Querying.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Versioning;
    using Bars.NuGet.Querying.Types;

    internal class NuGetVisitor : ExpressionVisitor
    {
        protected NuGetQueryFilter nuGetQueryFilter;
        internal NuGetQueryFilter GetNuGetQueryFilter()
        {
            if (this.nuGetQueryFilter.SupportedFrameworks == null)
            {
                this.ForFramework();
            }

            return nuGetQueryFilter;
        }

        public NuGetVisitor(NuGetQueryFilter nuGetQueryFilter)
        {
            this.nuGetQueryFilter = nuGetQueryFilter;
        }

        private void ForFramework()
        {
            var framework = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName;

            if (framework == null)
                throw new InvalidFilterCriteriaException("No one of framework version was chosen, and can't indicate current version of framework, use .ForFramework() method of query to add versions of frameworks applicabel to packages");

            var splitted = framework.Split(",");

            var version = new Version(splitted[1].Replace("Version=v", ""));
            var frameworkName = new FrameworkName(splitted[0].Replace(".", "").ToLowerInvariant(), version);

            nuGetQueryFilter.SupportedFrameworks = new List<FrameworkName>() { frameworkName };
        }
    }
}