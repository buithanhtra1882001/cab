using System;
using System.Collections.Generic;

namespace CabIdentityService.Infrastructures.Exceptions
{
    public class ValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> ErrorsDictionary { get; set; }

        public ValidationException(
            IReadOnlyDictionary<string, string[]> errorsDictionary)
        {
            ErrorsDictionary = errorsDictionary;
        }
    }
}
