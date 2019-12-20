using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Read
{
    public class LoadError
    {
        private string _error;

        public LoadError(WordBase element, TextPointer errorPoint, int wordCount, string error)
        {
            ErrorPoint = errorPoint;
            //StartPoint = startPoint;
            WordCount = wordCount;
            _error = error;
        }
        public TextPointer ErrorPoint;
        //public TextPointer StartPoint;
        public int WordCount; //?
        public string Error { get { return _error; } } //?

    }
}
