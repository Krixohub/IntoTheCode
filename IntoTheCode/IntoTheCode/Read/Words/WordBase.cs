
using IntoTheCode.Buffer;
using System.Collections.Generic;

namespace IntoTheCode.Read.Element.Words
{
    /// <summary>A Grammar element that can read charecters (a word).</summary>
    /// <remarks>Inherids <see cref="ParserElementBase"/></remarks>
    public abstract class WordBase : ParserElementBase
    {
        /// <summary>Get string representation of value from buffer.</summary>
        /// <param name="buf"></param>
        /// <param name="ptr"></param>
        /// <returns></returns>
        protected internal virtual string GetWord(TextSubString ptr)
        {
            return ptr == null || ptr.Length() == 0 ? string.Empty : TextBuffer.GetSubString(ptr);
        }

        protected const int a = 0;

        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            // If a word contains recursiveness, this must be overriden
            return true;
        }

    }
}