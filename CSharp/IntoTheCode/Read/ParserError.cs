using IntoTheCode.Buffer;
using System;

namespace IntoTheCode.Read
{
    public class ParserError
    {
        public ParserError()
        {
            ErrorPoint = -1;
        }
        public int ErrorPoint;
        public int WordCount; //?
        public int Line; //?
        public int Column; //?
        public Exception Ex;
        public string Error { get; internal set; } //?
        public string Message { get; internal set; } //?


        internal static int Compare(ParserError x, ParserError y)
        {
            return y.ErrorPoint - x.ErrorPoint;
        }
    }
}
