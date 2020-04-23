using IntoTheCode.Read;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
using System;
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
        public int PointerNextChar { get; set; }

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
        //public Action<TextBuffer> FindNextWordAction { get; set; }

        /// <summary>Function for skipping white spaces and reading comments.</summary>
        public void FindNextWord(List<TextElement> outElements, int level)
        {
            // todo comments can be inserted multiple times
            do
            {
                // Skip whitespaces.
                ReaderWhitespace.Load(outElements, level);

                // todo: Read comments
            } while (ReaderComment.Load(outElements, level));
        }

        /// <summary>Code element for adding comments.</summary>
        //public List<CodeElement> Comments { get; set; }
        internal WordWhitespace ReaderWhitespace { get; set; }
        internal WordComment ReaderComment { get; set; }

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
