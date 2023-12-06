using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;

namespace BNRNew_API.utils
{
    public class Utils
    {
        public static bool validatePoliceNo(string platNo)
        {
            Match m = Regex.Match(platNo, "^[a-zA-Z]{1,2} [0-9]{1,4} [a-zA-Z]{1,3}$", RegexOptions.IgnoreCase);
            return m.Success;
        }
    }
}
