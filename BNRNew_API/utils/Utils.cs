using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace BNRNew_API.utils
{
    public class Utils
    {
        public static bool validatePoliceNo(string platNo)
        {
            Match m = Regex.Match(platNo, "^[a-zA-Z]{1,2} [0-9]{1,4} [a-zA-Z]{1,3}$", RegexOptions.IgnoreCase);
            return m.Success;
        }

        public static string formatStringForPrinter(string left, string right, int count)
        {
            var c = left.Length + right.Length;
            var s = string.Concat(System.Linq.Enumerable.Repeat(" ", count - c));
            return String.Concat(left,s ,right);
        }
    }
}
