using System.Web;

namespace CabPaymentService.Infrastructures.Extensions
{
    public static class StringExtensions
    {
        public static void ParseFromQueryStringToSortedList(this string queryString, IDictionary<string, string> data)
        {
            data.Clear();
            var x = HttpUtility.ParseQueryString(queryString);

            foreach (var key in x.AllKeys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    data.Add(key, x[key] ?? string.Empty);
                }
            }
        }

        public static Guid ToGuid(this string id)
        {
            return Guid.Parse(id);
        }
        /// <summary>
        /// Indicates whether this string is insensitive equal with another string.
        /// </summary>
        public static bool EqualsIgnoreCase(this string s, string o)
        {
            return string.Equals(s, o, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Indicates whether this string is null or an System.String.Empty string.
        /// </summary>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Indicates whether this string is null, empty, or consists only of white-space characters.
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Indicates whether this string is not null, empty, or consists only of white-space characters.
        /// </summary>
        public static bool HasValue(this string str)
        {
            return !IsNullOrEmpty(str) && !IsNullOrWhiteSpace(str);
        }
    }
}
