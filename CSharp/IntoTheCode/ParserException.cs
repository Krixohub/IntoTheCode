using IntoTheCode.Grammar;
using System;
using System.Collections.Generic;

namespace IntoTheCode
{
    /// <summary>Error when the input doesn't match the Grammar.</summary>
    public class ParserException : Exception
    {
        /// <summary>Creator.</summary>
        /// <param name="msg">The message.</param>
        internal ParserException(string msg) : base (msg)
        {
            List<ParserError> errors = null;
            AllErrors = new List<ParserError>();
            if (errors != null)
                AllErrors.AddRange(errors);
        }

        public List<ParserError> AllErrors { get; internal set; }
    }
}
