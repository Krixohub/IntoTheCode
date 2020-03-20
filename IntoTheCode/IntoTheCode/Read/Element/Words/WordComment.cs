using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Message;
using System;

namespace IntoTheCode.Read.Element.Words
{
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

        public override bool Load(List<CodeElement> outElements, int level)
        {
            // Read comments on form '// rest of line cr nl'
            const string nl = "\r\n";

            if (TextBuffer.IsEnd(2) || '/' != TextBuffer.GetChar()) return false;

            TextBuffer.IncPointer();
            if ('/' != TextBuffer.GetChar())
            {
                TextBuffer.PointerNextChar--;
                return false;
            }
            TextBuffer.IncPointer();
            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar);

            // add comment
            TextBuffer.SetToIndexOf(subStr, nl);
            if (subStr.To < 0) subStr.To = TextBuffer.Length;

            if (outElements != null)
                outElements.Add(new CodeElement(this, subStr));

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
        public override int ResolveErrorsLast(CodeElement last)
        {
            throw new Exception("todo");
        }


    }
}