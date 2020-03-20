using IntoTheCode.Message;
using IntoTheCode.Read;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Buffer
{
    internal class FlatBuffer : TextBuffer
    {
        private readonly string _buf;

        public FlatBuffer(string text) : base()
        {
            _buf = text;
            ReaderComment = new WordComment() { TextBuffer = this };
            ReaderWhitespace = new WordWhitespace() { TextBuffer = this};
        }

        public override int Length { get { return _buf.Length; } }
        public override char GetChar() { return _buf[PointerNextChar]; }

        public override string GetSubString(int length) { return _buf.Substring(PointerNextChar, length); }
        public override string GetSubString(TextSubString sub) { return _buf.Substring(sub.From, sub.To - sub.From); }
        public override string GetSubString(int from, int length) { return _buf.Substring(from, length); }
        public override string GetSubString(int from, int offset, int length) { return _buf.Substring(from + offset, length); }

        public override int GetIndexAfter(string find, int start)
        {
            int startIndex = start == NotValidPtr ? 0 : start;
            int pos = _buf.IndexOf(find, startIndex, System.StringComparison.Ordinal);
            if (pos == -1) return NotValidPtr;
            return pos + find.Length;
        }

        public override void SetToIndexOf(TextSubString sub, string find) { SetToIndexOf(sub, find, PointerNextChar); }
        private void SetToIndexOf(TextSubString sub, string find, int start)
        { sub.To = _buf.IndexOf(find, start, System.StringComparison.Ordinal); }

        public override string GetLineAndColumn(out int line, out int column, int pos = NotValidPtr)
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