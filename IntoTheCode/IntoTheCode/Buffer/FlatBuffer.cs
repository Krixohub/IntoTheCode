using IntoTheCode.Message;
using IntoTheCode.Read;

namespace IntoTheCode.Buffer
{
    internal class FlatBuffer : ITextBuffer
    {
        private readonly string _buf;

        //public static int NotValidPtr { get { return -1; } }
        public const int NotValidPtr = -1;

        public FlatBuffer(string text)
        {
            _buf = text;
            PointerNextChar = 0;
            PointerEnd = _buf.Length;
            Status = new ParserStatus(this);
        }


        /// <summary>Pointing at the next char to read. When end is reached Buf.Length == pointer.</summary>
        public int PointerNextChar { get; private set; }
        public int PointerEnd { get; private set; }
        public ParserStatus Status { get; private set; }

        //public bool IsEnd()
        //{
        //    return ((FlatPointer)PointerTodo).index == _buf.Length;
        //}
        // : merge IsEnd functions

        public bool IsEnd(int length = 1)
        {
            return PointerNextChar + length > _buf.Length;
        }

        public int Length { get { return _buf.Length; } }

        // todo remove
        public void SetPointer(int ptr)
        {
            PointerNextChar = ptr;
        }
        // todo remove
        public void SetPointerBackToFrom(TextSubString sub)
        {
            PointerNextChar = sub.From;
        }
        public void SetPointerTo(TextSubString sub) { PointerNextChar = ((TextSubString)sub).To; }

        public void IncPointer() { ++PointerNextChar; }
        public void DecPointer() { --PointerNextChar; }

        //public TextPointer NewPointer() { return new FlatPointer { index = 0 }; }

        public TextSubString NewSubStringFrom()
        {
            return new TextSubString(PointerNextChar);
        }

        //// redundant function
        //void SetSubStringTo(TextSubString ss) { ((TextSubString)ss).To = PointerNextChar; }

        public char GetChar() { return _buf[PointerNextChar]; }
        public string GetSubString(int length) { return _buf.Substring(PointerNextChar, length); }
        public string GetSubString(TextSubString sub) { var ss = sub as TextSubString; return _buf.Substring(ss.From, ss.To - ss.From); }
        public string GetSubString(int from, int length) { return _buf.Substring(from, length); }
        public string GetSubString(int from, int offset, int length) { return _buf.Substring(from + offset, length); }

        public int GetIndexAfter(string find, int start)
        {
            int startIndex = start == FlatBuffer.NotValidPtr ? 0 : start;
            int pos = _buf.IndexOf(find, startIndex, System.StringComparison.Ordinal);
            if (pos == -1) return FlatBuffer.NotValidPtr;
            return pos + find.Length;
        }

        public void SetToIndexOf(TextSubString sub, string find) { SetToIndexOf(sub, find, PointerNextChar); }
        private void SetToIndexOf(TextSubString sub, string find, int start)
        { ((TextSubString)sub).To = _buf.IndexOf(find, start, System.StringComparison.Ordinal); }

        public string GetLineAndColumn(out int line, out int column, int pos = NotValidPtr)
        {
            if (pos == NotValidPtr)
                pos = PointerNextChar;
            int index = pos;
            string find = "\n";
            int nlPos = 0;
            line = 1;
            int findPos = _buf.IndexOf(find, nlPos, System.StringComparison.Ordinal);
            while (_buf.Length > nlPos && findPos > 0 && index > findPos)
            {
                line++;
                nlPos = findPos + find.Length;
                findPos = _buf.IndexOf(find, nlPos, System.StringComparison.Ordinal);
            }
            // add 1; the line starts with column 1.
            column = index - nlPos + 1;
            return string.Format(MessageRes.LineAndCol, line, column);
        }

    }
}