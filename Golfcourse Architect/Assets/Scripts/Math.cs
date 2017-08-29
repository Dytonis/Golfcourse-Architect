using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA
{
    public class Math
    {
        /// <summary>
        /// Normalizes float 'f' to 0-1 by bounds
        /// </summary>
        /// <param name="f">float to normalize</param>
        /// <param name="lower">lower bounds (inclusive)</param>
        /// <param name="upper">upper bounds (inclusive)</param>
        /// <returns></returns>
        public static float Normalize(float f, float lower, float upper)
        {
            return (f - lower) / (upper - lower);
        }

        /// <summary>
        /// Normalizes float 'f' to range normalizedLower-normalizedUpper by bounds
        /// </summary>
        /// <param name="f">float to normalize</param>
        /// <param name="lower">lower bounds (inclusive)</param>
        /// <param name="upper">upper bounds (inclusive)</param>
        /// <param name="normalizedLower">the new lower bounds (inclusive)</param>
        /// <param name="normalizedUpper">the new upper bounds (inclusive)</param>
        /// <returns></returns>
        public static float NormalizeRange(float f, float lower, float upper, float normalizedLower, float normalizedUpper)
        {
            float a = Normalize(f, lower, upper);

            return (a * (normalizedUpper - normalizedLower)) + normalizedLower;
        }

        /// <summary>
        /// Normalizes float 'f' to range normalizedLower-normalizedUpper by bounds and then inverses the result
        /// </summary>
        /// <param name="f">float to normalize</param>
        /// <param name="lower">lower bounds (inclusive)</param>
        /// <param name="upper">upper bounds (inclusive)</param>
        /// <param name="normalizedLower">the new lower bounds (inclusive)</param>
        /// <param name="normalizedUpper">the new upper bounds (inclusive)</param>
        /// <returns></returns>
        public static float InverseNormalizeRange(float f, float lower, float upper, float normalizedLower, float normalizedUpper)
        {
            float a = Normalize(f, lower, upper);

            return (normalizedUpper - ((a * (normalizedUpper - normalizedLower)) + normalizedLower)) + normalizedLower;
        }

        /// <summary>
        /// Returns the difference between two floats regardless of order.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float AbsDifference(float a, float b)
        {
            if (a > b)
                return a - b;
            else return b - a;
        }
    }
}
