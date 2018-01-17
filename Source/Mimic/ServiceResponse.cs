namespace Mimic
{
    /// <summary>
    /// The data necessary for generating a response from a virtual service. 
    /// This is used in the Service class.
    /// </summary>
    struct ServiceResponse
    {
        public string ContentType;
        public int StatusCode;
        public string Body;
    }
}
