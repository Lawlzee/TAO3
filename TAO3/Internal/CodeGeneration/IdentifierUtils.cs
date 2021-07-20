using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.CodeGeneration
{
    internal static class IdentifierUtils
    {
        internal static string ToCSharpIdentifier(string str)
        {
            StringBuilder? sb = new StringBuilder(str.Length);
            bool shouldUpper = true;

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(shouldUpper ? char.ToUpper(c) : c);
                    shouldUpper = false;
                }
                else
                {
                    shouldUpper = true;
                }
            }

            return sb.ToString();
        }
    }
}
