using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Message;
using System;

namespace IntoTheCode.Grammar
{
    internal class Comment
    {
        internal Comment()
        {
            CommentBuffer = new List<CommentElement>();
        }

        internal TextBuffer TextBuffer;

        public List<CommentElement> CommentBuffer { get; private set; }
        public  bool Load(IList<TextElement> outElements, bool lineEnd)
        {
            // Read comments on form '// rest of line [cr] nl'
            const string nl = "\n";

            if (TextBuffer.IsEnd(1) || '/' != TextBuffer.GetChar() || '/' != TextBuffer.GetChar(1)) return false;

            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar + 2);

            // add comment
            int nlLenght = 1;
            TextBuffer.SetToIndexOf(subStr, nl);
            if (subStr.To < 0)
            {
                subStr.To = TextBuffer.Length;
                TextBuffer.PointerNextChar = TextBuffer.Length;
            }
            else
            { 
                if ('\r' == TextBuffer.GetChar(1 + subStr.To - subStr.From))
                {
                    subStr.To--;
                    nlLenght = 2;
                }
                TextBuffer.PointerNextChar = subStr.To + nlLenght;
            }

            if (!lineEnd)
                CommentBuffer.Add(new CommentElement(TextBuffer, subStr));
            else if (outElements != null)
                outElements.Add(new CommentElement(TextBuffer, subStr));

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