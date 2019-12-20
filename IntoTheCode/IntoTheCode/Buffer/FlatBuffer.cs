namespace IntoTheCode.Buffer
{
    internal class FlatBuffer : ITextBuffer
    {
        private readonly string _buf;

        public FlatBuffer(string text)
        {
            _buf = text;
            PointerNextChar = new FlatPointer { index = 0 };
            PointerEnd = new FlatPointer { index = _buf.Length };
        }


        /// <summary>Pointing at the next char to read. When end is reached Buf.Length == pointer.</summary>
        public TextPointer PointerNextChar { get; private set; }
        public TextPointer PointerEnd { get; private set; }

        //public bool IsEnd()
        //{
        //    return ((FlatPointer)PointerTodo).index == _buf.Length;
        //}
        // : merge IsEnd functions

        public bool IsEnd(int length = 1)
        {
            return ((FlatPointer)PointerNextChar).index + length > _buf.Length;
        }

        public int Length { get { return _buf.Length; } }

        public void SetPointer(TextPointer ptr)
        {
            ((FlatPointer)PointerNextChar).index = ((FlatPointer)ptr).index;
        }
        public void SetPointerBackToFrom(TextSubString sub)
        {
            ((FlatPointer)PointerNextChar).index = ((FlatSubString)sub).From;
        }
        public void SetPointerTo(TextSubString sub) { ((FlatPointer)PointerNextChar).index = ((FlatSubString)sub).To; }

        public void IncPointer() { ++((FlatPointer)PointerNextChar).index; }
        public void DecPointer() { --((FlatPointer)PointerNextChar).index; }

        public TextPointer NewPointer() { return new FlatPointer { index = 0 }; }

        public TextSubString NewSubStringFrom()
        {
            return new FlatSubString() { From = ((FlatPointer)PointerNextChar).index };
        }

        //// redundant function
        //void SetSubStringTo(TextSubString ss) { ((FlatSubString)ss).To = ((FlatPointer)PointerNextChar).index; }

        public char GetChar() { return _buf[((FlatPointer)PointerNextChar).index]; }
        public string GetSubString(int length) { return _buf.Substring(((FlatPointer)PointerNextChar).index, length); }
        public string GetSubString(TextSubString sub) { var ss = sub as FlatSubString; return _buf.Substring(ss.From, ss.To - ss.From); }
        public string GetSubString(TextPointer from, int length) { return _buf.Substring(((FlatPointer)from).index, length); }
        public string GetSubString(TextPointer from, int offset, int length) { return _buf.Substring(((FlatPointer)from).index + offset, length); }

        public TextPointer GetIndexAfter(string find, TextPointer start)
        {
            int startIndex = start == null ? 0 : ((FlatPointer)start).index;
            int pos = _buf.IndexOf(find, startIndex, System.StringComparison.Ordinal);
            if (pos == -1) return null;
            return new FlatPointer { index = pos + find.Length };
        }

        public void SetToIndexOf(TextSubString sub, string find) { SetToIndexOf(sub, find, PointerNextChar); }
        private void SetToIndexOf(TextSubString sub, string find, TextPointer start)
        { ((FlatSubString)sub).To = _buf.IndexOf(find, ((FlatPointer)start).index, System.StringComparison.Ordinal); }

        public string GetLineAndColumn(TextPointer pos = null)
        {
            if (pos == null)
                pos = PointerNextChar.Clone();
            int index = ((FlatPointer)pos).index;
            string find = "\n";
            int nlPos = 0;
            int line = 1;
            int findPos = _buf.IndexOf(find, nlPos, System.StringComparison.Ordinal);
            while (_buf.Length > nlPos && findPos > 0 && index > findPos)
            {
                line++;
                nlPos = findPos + find.Length;
                findPos = _buf.IndexOf(find, nlPos, System.StringComparison.Ordinal);
            }
            // add 1; the line starts with column 1.
            return string.Format("Line {0}, colomn {1}", line, index - nlPos + 1); 
        }

    }
}