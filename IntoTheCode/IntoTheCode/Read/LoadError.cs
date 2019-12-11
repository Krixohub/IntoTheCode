using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;

namespace IntoTheCode.Read
{
    public class LoadError
    {
        public TextPointer ErrorPoint;
        public TextPointer StartPoint;
        public ParserElementBase Element;
        public int WordCount; //?
    }
}
