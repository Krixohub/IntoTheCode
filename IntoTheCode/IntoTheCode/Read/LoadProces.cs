using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
using System.Collections.Generic;

namespace IntoTheCode.Read
{
    public class LoadProces
    {
        /// <summary>Field for text buffer.</summary>
        private ITextBuffer _textBuffer;

        internal LoadProces(ITextBuffer buf)
        {
            _textBuffer = buf;
            ErrorMsg = string.Empty;
            UnambiguousPointer = new FlatPointer();
        }

        ///// <summary>The text buffer to read from.</summary>
        //internal ITextBuffer TextBuffer
        //{
        //    get { return _textBuffer; }
        //    set { _textBuffer = value; }
        //}

        /// <summary>Error message after parsing/reading input text.</summary>
        /// <exclude/>
        public string ErrorMsg { get; internal set; }

        /// <summary>Error message after parsing/reading input text.</summary>
        /// <exclude/>
        public bool Error { get { return Errors != null; } }

        /// <summary>Error message after parsing/reading input text.</summary>
        /// <exclude/>
        public List<ParserError> Errors { get; internal set; }

        #region add errors

        public void AddSyntaxError(WordBase element, TextPointer errorPoint, int wordCount, string error)
        {
            var err = new ParserError();
            err.WordCount = wordCount;
            err.ErrorPoint = errorPoint;
            err.Error = error;

            _textBuffer.GetLineAndColumn(out err.Line, out err.Column, errorPoint);
            string s = string.Format("Line {0}, colomn {1}", err.Line, err.Column);

            err.Message = "Syntax error (" +
                            element.GetRule(element).Name +
                            "). " + error + " " + s;

            if (Errors == null) Errors = new List<ParserError>();
            Errors.Add(err);

        }

        public void AddSyntaxErrorEof(string error)
        {
            var err = new ParserError();
            _textBuffer.GetLineAndColumn(out err.Line, out err.Column);
            string s = string.Format("Line {0}, colomn {1}", err.Line, err.Column);

            err.Message = error + " " + s;

            // todo is this ok?
            ErrorMsg = err.Message;
            if (Errors == null) Errors = new List<ParserError>();
            Errors.Add(err);
        }

        public void AddParseError(string error)
        {
            var err = new ParserError();
            _textBuffer.GetLineAndColumn(out err.Line, out err.Column);
            string s = string.Format("Line {0}, colomn {1}", err.Line, err.Column);

            err.Message = error + " " + s;

            // todo is this ok?
            ErrorMsg = err.Message;
            if (Errors == null) Errors = new List<ParserError>();
            Errors.Add(err);
        }

        #endregion add errors

        #region Next 

        public void ThisIsUnambiguous(ParserElementBase reader, CodeElement code)
        {
            // Set SafePointer to char after element
            UnambiguousPointer = code.SubString.GetTo();
            //UnambiguousWordCount = numberOfWords; 
        }

        public TextPointer UnambiguousPointer;
        //public int UnambiguousWordCount;

        #endregion Next 
    }
}
