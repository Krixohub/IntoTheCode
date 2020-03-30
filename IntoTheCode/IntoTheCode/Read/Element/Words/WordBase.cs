using System;

using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Read;
using System.Collections.Generic;

namespace IntoTheCode.Read.Element.Words
{
    /// <summary>A Grammar element that can read charecters (a word).</summary>
    public abstract class WordBase : ParserElementBase
    {
        /// <summary>Get string representation of value from buffer.</summary>
        /// <param name="buf"></param>
        /// <param name="ptr"></param>
        /// <returns></returns>
        protected internal virtual string GetValue(TextSubString ptr)
        {
            return ptr == null || ptr.Length() == 0 ? string.Empty : TextBuffer.GetSubString(ptr);
        }

        /// <summary>The element content type is 'OneValue' for all words.</summary>
        /// <returns>OneValue.</returns>
        public override ElementContentType GetElementContent()
        {
            return ElementContentType.OneValue;
        }
        ///// <summary>The element content type is 'OneValue' for all words.</summary>
        ///// <returns>OneValue.</returns>
        //public override ElementContentType SetElementContent(ParserElementBase origin)
        //{
        //    _elementContent = ElementContentType.OneValue;
        //    return _elementContent;
        //}
        protected const int a = 0;

        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            // If a word contains recursiveness, this must be overriden
            return true;
        }

    }
}