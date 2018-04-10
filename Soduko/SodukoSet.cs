using System;
using System.Collections.Generic;
using System.Linq;

namespace Soduko
{
    public class SodukoSet
    {
        private static IReadOnlyList<string> allValues = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public static string EmptyValue => string.Empty;

        internal static bool IsValidValue(string value) => value == EmptyValue || allValues.Contains(value);

        internal static IReadOnlyList<string> AllValues() => allValues;
    }
}
