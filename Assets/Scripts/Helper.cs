using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SajberSim.Helper
{
    class Helper
    {
        /// <summary>
        /// Checks if input is an int or not
        /// </summary>
        public static bool IsNum(string input)
        {
            if (int.TryParse(input, out int n)) return true;
            else return false;
        }
    }
}
