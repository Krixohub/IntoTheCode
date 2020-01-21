using IntoTheCode.Read;

namespace IntoTheCode.Buffer
{
    public interface ITextBuffer
    {
        //string Buf { get; set; }

        /// <summary>Pointing at the next char to read. When end is reached Buf.Length == pointer.</summary>
        //int pointer { get; set; }
        int PointerNextChar { get; }
        int PointerEnd { get; }
        ParserStatus Status { get; }

        void SetPointerBackToFrom(TextSubString sub);
        void SetPointerTo(TextSubString sub);
        void SetPointer(int txtPtr);

        int Length { get; }

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
        string GetSubString(int from, int length);
        string GetSubString(int from, int offset, int length);

        int GetIndexAfter(string find, int start);

        void SetToIndexOf(TextSubString sub, string find);
        //void SetToIndexOf(TextSubString sub, string find, TextPointer from);
        //bool IsEnd();
        bool IsEnd(int length = 1);
        string GetLineAndColumn(out int line, out int column, int pos = -1);
    }
}
