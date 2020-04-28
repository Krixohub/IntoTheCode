using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Message;
using System;

namespace IntoTheCode.Read.Structure.Structure
{
    internal class Comment
    {
        internal Comment()
        {
            CommentBuffer = new List<CommentElement>();
        }

        internal TextBuffer TextBuffer;

        public List<CommentElement> CommentBuffer { get; private set; }
        public  bool Load(List<TextElement> outElements, bool lineEnd)
        {
            // Read comments on form '// rest of line cr nl'
            const string nl = "\r\n";

            if (TextBuffer.IsEnd(1) || '/' != TextBuffer.GetChar() || '/' != TextBuffer.GetChar(1)) return false;

            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar + 2);

            // add comment
            TextBuffer.SetToIndexOf(subStr, nl);
            if (subStr.To < 0) subStr.To = TextBuffer.Length;

            if (lineEnd && outElements != null)
                outElements.Add(new CommentElement(TextBuffer, subStr));

            if (!lineEnd)
                CommentBuffer.Add(new CommentElement(TextBuffer, subStr));

            if (subStr.To != TextBuffer.Length)
                TextBuffer.PointerNextChar = subStr.To + 2;
            else
                TextBuffer.PointerNextChar = TextBuffer.Length;

            return true;
        }

        public bool ResolveErrorsForward()
        {
            throw new Exception("todo");
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public int ResolveErrorsLast(TextElement last)
        {
            throw new Exception("todo");
        }


    }
}