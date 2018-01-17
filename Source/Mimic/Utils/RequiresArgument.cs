using System;

namespace Mimic.Utils
{
    /// <summary>
    /// Provides a set of methods for validating arguments to functions and 
    /// methods.
    /// </summary>
    public static class RequiresArgument
    {
        /// <summary>
        /// Throws an ArgumentException when the specified object is null.
        /// </summary>
        public static void NotNull(object obj, string argumentName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Throws an ArgumentException when the specified string is null 
        /// or white space.
        /// </summary>
        public static void NotNullOrWhiteSpace(string str, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Throws an ArgumentException when the specified array is not the 
        /// required length.
        /// </summary>
        public static void LengthEquals<T>(T[] arr, int length, string errorMessage)
        {
            if (arr.Length != length)
            {
                throw new ArgumentException(errorMessage);
            }
        }
    }
}
