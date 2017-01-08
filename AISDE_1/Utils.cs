using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISDE_1
{
    public static class Utils
    {
        /// <summary>
        /// Obcina stringa do maksymalnej liczby znaków.
        /// </summary>
        /// <param name="length">Liczba znaków, które mają pozostać.</param>
        /// <returns></returns>
        public static string Truncate(string s, int length)
        {
            if (s.Length < length)
                return s;
            else return s.Substring(0, length);
        }
    }
}
