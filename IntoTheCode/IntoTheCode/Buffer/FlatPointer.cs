namespace IntoTheCode.Buffer
{
    internal class FlatPointer: TextPointer
    {
        internal int index;
        public override TextPointer Clone(int addColumns = 0) { return new FlatPointer { index = index + addColumns }; }
        public override int CompareTo(TextPointer value) { return index - ((FlatPointer)value).index; }
        public override void CopyTo(TextPointer value) { ((FlatPointer)value).index = index; }
        public override bool IsValid() { return index > -1; }

    }
}
