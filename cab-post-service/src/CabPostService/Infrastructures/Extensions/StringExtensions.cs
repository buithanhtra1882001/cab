namespace CabPostService.Infrastructures.Extensions
{
    public static class StringExtensions
    {
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
    }
}