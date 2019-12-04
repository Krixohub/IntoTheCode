using System;

namespace IntoTheCode.Basic
{
    /// <summary>Error when the input doesn't match the syntax.</summary>
    public class SyntaxErrorException : Exception
    {
        /// <summary>Creator.</summary>
        /// <param name="msg">The message.</param>
        internal SyntaxErrorException(string msg) : base (msg)
        { }
    }
}
