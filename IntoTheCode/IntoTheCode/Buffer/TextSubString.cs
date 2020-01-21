namespace IntoTheCode.Buffer
{
    public class TextSubString
    {
        public TextSubString( int from )
        {
            From = from;
            To = FlatBuffer.NotValidPtr;
        }

        public int From { get; private set; }
        public int To { get; set; }
        public int Length() { return To - From; }

        // todo remove this function?
        public bool ToIsValid() { return To != FlatBuffer.NotValidPtr; }
    }
}
