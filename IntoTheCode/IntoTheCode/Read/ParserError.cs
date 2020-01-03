using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Read
{
    public class ParserError
    {
        private string _error;

        /// <summary>Syntax error.</summary>
        /// <param name="element"></param>
        /// <param name="errorPoint"></param>
        /// <param name="wordCount"></param>
        /// <param name="error"></param>
        public ParserError(WordBase element, TextPointer errorPoint, int wordCount, string error)
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
