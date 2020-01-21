namespace IntoTheCode.Buffer
{
    public class TextSubString
    {
        public const int notValidPtr = -1;
        public int From { get; internal set; }
        public int To { get; set; }

        public int Length() { return To - From; }

        public bool FromGtEqPointer(int ptr) { return From >= ptr; }

        public bool ToGtPointer(int ptr) { return To > ptr; }

            // todo remove this function?
        public bool ToIsValid() { return To != notValidPtr; }
    }
}
