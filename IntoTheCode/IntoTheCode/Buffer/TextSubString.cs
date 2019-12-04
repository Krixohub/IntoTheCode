namespace IntoTheCode.Buffer
{
    public abstract class TextSubString
    {
        //public abstract TextPointer GetFrom();
        public abstract void SetTo(TextPointer ptr);
        public abstract TextPointer GetTo();
        public abstract TextPointer GetFrom();
        public abstract int Length();
        public abstract bool FromGtEqPointer(TextPointer ptr);
        public abstract bool ToGtPointer(TextPointer ptr);
        public abstract bool ToIsValid();
    }
}
