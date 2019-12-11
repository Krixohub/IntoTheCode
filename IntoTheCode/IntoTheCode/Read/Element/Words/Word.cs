using System;

using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Read;

namespace IntoTheCode.Read.Element.Words
{
    /// <summary>A Syntax element that can read charecters (a word).</summary>
    public abstract class Word : ParserElementBase
    {
        /// <summary>Get string representation of value from buffer.</summary>
        /// <param name="buf"></param>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public virtual string GetValue(ITextBuffer buf, TextSubString ptr)
        {
            //return Parser.TextBuffer.GetSubString(ptr);
            return ptr == null || ptr.Length() == 0 ? string.Empty : buf.GetSubString(ptr);
        }

        /// <summary>The element content type is 'OneValue' for all words.</summary>
        /// <returns>OneValue.</returns>
        public override ElementContentType GetElementContent()
        {
            return ElementContentType.OneValue;
        }
    }
}