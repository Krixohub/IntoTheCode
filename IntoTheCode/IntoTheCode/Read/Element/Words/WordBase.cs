using System;

using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Read;

namespace IntoTheCode.Read.Element.Words
{
    /// <summary>A Syntax element that can read charecters (a word).</summary>
    public abstract class WordBase : ParserElementBase
    {
        /// <summary>Get string representation of value from buffer.</summary>
        /// <param name="buf"></param>
        /// <param name="ptr"></param>
        /// <returns></returns>
        protected internal virtual string GetValue(TextSubString ptr)
        {
            //return Parser.TextBuffer.GetSubString(ptr);
            return ptr == null || ptr.Length() == 0 ? string.Empty : TextBuffer.GetSubString(ptr);
        }

        /// <summary>The element content type is 'OneValue' for all words.</summary>
        /// <returns>OneValue.</returns>
        public override ElementContentType GetElementContent()
        {
            return ElementContentType.OneValue;
        }
        protected const int a = 0;
    }
}