using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Read
{
    public class LoadError
    {
        public LoadError(WordBase element, TextPointer errorPoint, int wordCount, string error)
        {
            Element = element;
            ErrorPoint = errorPoint;
            //StartPoint = startPoint;
            WordCount = wordCount;
            Error = error;
        }
        public TextPointer ErrorPoint;
        //public TextPointer StartPoint;
        public ParserElementBase Element;
        public int WordCount; //?
        public string Error; //?
    }
}
