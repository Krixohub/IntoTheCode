﻿using IntoTheCode.Read;
using IntoTheCode.Read.Structure;
using IntoTheCode.Read.Words;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCode.Buffer
{
    public abstract class TextBuffer
    {
        public const int NotValidPtr = -1;

        private Dictionary<RuleLink, LoopLevel> _recursiveCalls = new Dictionary<RuleLink, LoopLevel>();

        public TextBuffer()
        {
            PointerNextChar = 0;
            Status = new ParserStatus(this);
            //Comments = new List<CodeElement>();
        }

        /// <summary>Pointing at the next char to read. When end is reached Length == PointerNextChar.</summary>
        public int PointerNextChar { get { return _pointerNextChar; }
                set
                    {
                if (_pointerNextChar > value)
                    for (int i = ReaderComment.CommentBuffer.Count - 1; i >= 0 ; i--)
                        if (ReaderComment.CommentBuffer[i].SubString.From >= value)
                            ReaderComment.CommentBuffer.Remove(ReaderComment.CommentBuffer[i]);
                    _pointerNextChar = value;
                }
            }
        private int _pointerNextChar;

        public ParserStatus Status { get; protected set; }

        /// <summary>Each RuleLink has a level of recursive calls to stop infite loops.</summary>
        /// <param name="link">The RuleLink.</param>
        /// <returns>Loop level.</returns>
        internal LoopLevel GetLoopLevel(RuleLink link)
        {
            LoopLevel level;
            if (!_recursiveCalls.TryGetValue(link, out level))
            {
                level = new LoopLevel() { LastInvokePos = NotValidPtr };
                _recursiveCalls.Add(link, level);
            }
            return level;
        }

        /// <summary>Function for skipping white spaces and reading comments.</summary>
        public void FindNextWord(List<TextElement> outElements, bool inline)
        {
             

            // todo comments can be inserted multiple times
            do
            {
                // Skip whitespaces.
                //                ReaderWhitespace.Load(outElements, level);
                //string ws = " \r\n\t";
                string ws = inline ? " \t" : " \r\n\t";

                // Read white spaces
                while (!IsEnd() && ws.Contains(GetChar()))
                    IncPointer();

                // todo: Read comments
            } while (ReaderComment.Load(outElements, inline) && !inline);

            if (outElements != null) InsertComments(outElements);
        }

        /// <summary>Insert the preceding comments to output.</summary>
        public void InsertComments(List<TextElement> outElements)
        {
            outElements.AddRange(ReaderComment.CommentBuffer);
            ReaderComment.CommentBuffer.Clear();
        }

        /// <summary>Code element for adding comments.</summary>
        //public List<CodeElement> Comments { get; set; }
        //internal WordWhitespace ReaderWhitespace { get; set; }
        internal Comment ReaderComment { get; set; }

        public bool IsEnd(int pos) { return PointerNextChar + pos >= Length; }
        public bool IsEnd() { return PointerNextChar >= Length; }

        public void IncPointer(int length = 1) { PointerNextChar += length; }

        #region abstract functions

        public abstract int Length { get; }
        public abstract char GetChar();
        public abstract char GetChar(int pos);

        public abstract string GetSubString(int length);
        public abstract string GetSubString(TextSubString sub);
        public abstract string GetSubString(int from, int length);
        public abstract string GetSubString(int from, int offset, int length);

        public abstract int GetIndexAfter(string find, int start);

        public abstract void SetToIndexOf(TextSubString sub, string find);

        public abstract string GetLineAndColumn(out int line, out int column, int pos = NotValidPtr);

        #endregion abstract functions
    }
}
