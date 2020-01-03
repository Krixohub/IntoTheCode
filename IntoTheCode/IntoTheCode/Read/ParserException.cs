using System;
using System.Collections.Generic;

namespace IntoTheCode.Read
{
    /// <summary>Error when the input doesn't match the syntax.</summary>
    public class ParserException : Exception
    {
        /// <summary>Creator.</summary>
        /// <param name="msg">The message.</param>
        internal ParserException(string msg) : base (msg)
        {
            List<ParserError> errors = null;
            Errors = new List<ParserError>();
            if (errors != null)
                Errors.AddRange(errors);
        }

        public List<ParserError> Errors { get; internal set; }
    }
}
