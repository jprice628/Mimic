namespace Mimic
{
    /// <summary>
    /// Represents interesting information about a service. This is used in 
    /// the Service class, and it's what is returned to callers when the 
    /// ServiceFunctions.QueryService function is invoked.
    /// </summary>
    public struct ServiceStats
    {
        public int CallCount;
        public string LastRequestBody;
    }
}
