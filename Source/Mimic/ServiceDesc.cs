namespace Mimic
{
    public class ServiceDesc
    {
        #region Basic Parameters

        /// <summary>
        /// Specifies the ID of the service. If this is not set, an ID will 
        /// be generated automatically.
        /// </summary>
        public string Id = null;

        #endregion

        #region Request Parameters

        /// <summary>
        /// The HTTP method, i.e. GET, POST, PUT, DELETE, etc. that this 
        /// service will respond to. This is a required field.
        /// </summary>
        public string Method = null;

        /// <summary>
        /// The HTTP path, e.g. /api/someresource, that the service responds to.
        /// This is a required field.
        /// </summary>
        public string Path = null;

        /// <summary>
        /// A filter that can optionally be used to limit the service so that 
        /// it only responds to requests that contain the specified text in 
        /// their bodies.
        /// </summary>
        public string BodyContains = null;

        #endregion

        #region Response Parameters
        
        /// <summary>
        /// Specifies the content type of the response that the service will 
        /// issue when invoked.
        /// </summary>
        public string ContentType = "text/plain";

        /// <summary>
        /// Specifies the status code of the response that the service will 
        /// issue when invoked.
        /// </summary>
        public string StatusCode = "200";

        /// <summary>
        /// Specifies the body of the response that the service will issue 
        /// when invoked.
        /// </summary>
        public string Body = string.Empty;

        #endregion
    }
}
