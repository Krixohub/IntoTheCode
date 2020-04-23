using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Message;
using System;

namespace IntoTheCode.Read.Element.Words
{
    /// <remarks>Inherids <see cref="WordBase"/></remarks>
    internal class WordComment : WordBase
    {
        internal WordComment()
        {
            Name = MetaParser.Comment____;
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return null;
        }

        public override string GetGrammar() { return string.Empty; }

        public override bool Load(List<ReadElement> outElements, int level)
        {
            // Read comments on form '// rest of line cr nl'
            const string nl = "\r\n";

            if (TextBuffer.IsEnd(1) || '/' != TextBuffer.GetChar() || '/' != TextBuffer.GetChar(1)) return false;

            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar + 2);

            // add comment
            TextBuffer.SetToIndexOf(subStr, nl);
            if (subStr.To < 0) subStr.To = TextBuffer.Length;

            if (outElements != null)
                outElements.Add(new CommentElement(this, subStr));

            if (subStr.To != TextBuffer.Length)
                TextBuffer.PointerNextChar = subStr.To + 2;
            else
                TextBuffer.PointerNextChar = TextBuffer.Length;

            return true;
        }

        public override bool ResolveErrorsForward()
        {
            throw new Exception("todo");
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(ReadElement last)
        {
            throw new Exception("todo");
        }


    }
}