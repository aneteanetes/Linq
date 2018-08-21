namespace Bars.NuGet.Querying.Types
{
    using System;
    using System.Collections.Generic;

    internal class NuGetQueryValidator
    {
        public bool IsValid { get; set; } = true;

        public IEnumerable<Exception> Errors { get; set; }

        public static NuGetQueryValidator ValidateFilter(NuGetQueryFilter nuGetQueryFilter)
        {
            var validator = new NuGetQueryValidator();
            var errors = new List<Exception>();

            if (nuGetQueryFilter.SupportedFrameworks.Count == 0)
            {
                validator.IsValid = false;
                errors.Add(new Exception($"NuGet Queryable must contains at least one {nameof(NuGetQueryExtensions.ForFramework)} method!"));
            }

            validator.Errors = errors;
            return validator;
        }
    }
}