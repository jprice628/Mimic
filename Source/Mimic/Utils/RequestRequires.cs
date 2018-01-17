namespace Mimic.Utils
{
    /// <summary>
    /// Provides a set of methods for validating incoming requests to the 
    /// service.
    /// </summary>
    public static class RequestRequires
    {
        /// <summary>
        /// Throws a BadRequestException when the specified string is null 
        /// or white space.
        /// </summary>
        public static void NotNullOrWhiteSpace(string str, string message)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new BadRequestException(message);
            }
        }
    }
}
