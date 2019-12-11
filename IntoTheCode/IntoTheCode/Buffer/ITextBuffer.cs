namespace IntoTheCode.Buffer
{
    public interface ITextBuffer
    {
        //string Buf { get; set; }

        /// <summary>Pointing at the next char to read. When end is reached Buf.Length == pointer.</summary>
        //int pointer { get; set; }
        TextPointer PointerNextChar { get; }

        void SetPointerBackToFrom(TextSubString sub);
        void SetPointerTo(TextSubString sub);
        void SetPointer(TextPointer txtPtr);

        int Length { get; }

        TextPointer NewPointer();

        /// <summary>Instantiates a new substring with 'from' value set to current pointer.</summary>
        /// <returns></returns>
        TextSubString NewSubStringFrom();

        void IncPointer();
        void DecPointer();

        char GetChar();

        //// todo: get char and increase pointer         
        //string ReadChar();

        string GetSubString(int length);
        string GetSubString(TextSubString sub);
        string GetSubString(TextPointer from, int length);
        string GetSubString(TextPointer from, int offset, int length);

        TextPointer GetIndexAfter(string find, TextPointer start);
        void SetToIndexOf(TextSubString sub, string find);
        //void SetToIndexOf(TextSubString sub, string find, TextPointer from);
        //bool IsEnd();
        bool IsEnd(int length = 1);
        string GetLineAndColumn(TextPointer from = null);
    }
}
