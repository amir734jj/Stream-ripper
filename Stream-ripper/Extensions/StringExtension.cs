using System.Linq;

namespace StreamRipper.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Contains any token
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string str, params string[] tokens) => tokens.Any(str.Contains);
    }
}