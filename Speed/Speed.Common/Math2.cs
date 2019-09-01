using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Speed
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class Math2
    {

        public static double Truncate(double value, int decimalPlaces)
        {
            double pow = Math.Pow(10, decimalPlaces);
            return Math.Truncate(value * pow) / pow;
        }

        /// <summary>
        /// Checa se um número é ímpar
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        public static int Multiple(int value, int multiple)
        {
            var ret = (value / multiple) * multiple;
            if (ret == 0 || ret < value)
                ret += multiple;
            return ret;
        }

        public static int MultipleGt(int value, int multiple)
        {
            var ret = multiple;
            while (ret < value)
                ret *= multiple;
            return ret;
        }

    }

}
