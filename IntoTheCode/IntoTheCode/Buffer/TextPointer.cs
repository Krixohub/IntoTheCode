namespace IntoTheCode.Buffer
{
    public abstract class TextPointer
    {
        public abstract TextPointer Clone();
        public abstract void CopyTo(TextPointer value);
        public abstract int CompareTo(TextPointer value);
        public abstract bool IsValid();
    }
}
