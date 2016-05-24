using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicMicroOrm
{
    /// <summary>   List extensions. </summary>
    ///
    /// <remarks>   Nsl, 08.01.2013. </remarks>

    public static class ListExtensions
    {
        /// <summary>
        ///     A List extension method that converts this object to a formatted string.
        /// </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <param name="s">            The s to act on. </param>
        /// <param name="delimiter">    The delimiter. </param>
        ///
        /// <returns>   The given data converted to a string. </returns>

        public static string ToFormattedString(this List<string> s, string delimiter)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < s.Count(); i++)
            {
                sb.Append(s[i]);

                if (i + 1 < s.Count())
                {
                    sb.Append(delimiter);
                }
            }

            return sb.ToString();
        }
    }
}
