using System.IO;

namespace Mimic.Utils
{
    /// <summary>
    /// A structure for holding state while parsing. This is used primarily 
    /// for parsing service descriptions. This is used when a request to add 
    /// a new service is received.
    /// </summary>
    public class ParseContext
    {
        /// <summary>
        /// A StreamReader that can read from the content to be parsed. This 
        /// usually points at the body of the request message and is only used
        /// for reading to the end of the stream once the "# Body" comment has 
        /// been encoutered.
        /// </summary>
        public StreamReader Reader { get; private set; }
        
        /// <summary>
        /// A state object that can be built up/manipulated during parsing. 
        /// This is useful for storing intermediate data as well as 
        /// constructing the output of the parser. When adding a service, this 
        /// will be a ServiceDesc object.
        /// </summary>
        public object State { get; private set; }

        /// <summary>
        /// Stores the current line being parsed.
        /// </summary>
        public string Input { get; set; }

        public ParseContext(StreamReader reader, object state)
        {
            RequiresArgument.NotNull(reader, "reader");
            RequiresArgument.NotNull(state, "state");

            Reader = reader;
            State = state;
        }
    }
}
