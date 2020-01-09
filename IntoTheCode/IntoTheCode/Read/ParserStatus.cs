using IntoTheCode.Basic.Util;
using IntoTheCode.Buffer;
using IntoTheCode.Message;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace IntoTheCode.Read
{
    public class ParserStatus
    {
        /// <summary>Field for text buffer.</summary>
        private ITextBuffer _textBuffer;

        internal ParserStatus(ITextBuffer buf)
        {
            _textBuffer = buf;
            ErrorMsg = string.Empty;
            UnambiguousPointer = new FlatPointer();
        }

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
            // todo Expression<Func<string>> resourceExpression
            var err = new ParserError();
            err.WordCount = wordCount;
            err.ErrorPoint = errorPoint;
            err.Error = error;

            string s = _textBuffer.GetLineAndColumn(out err.Line, out err.Column, errorPoint);

            err.Message = "Syntax error (" +
                            element.GetRule(element).Name +
                            "). " + error + " " + s;

            if (Errors == null) Errors = new List<ParserError>();
            Errors.Add(err);

        }

        public void AddSyntaxErrorEof(Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string error = DotNetUtil.Res(resourceExpression, parm);
            var err = new ParserError();
            
            string s = _textBuffer.GetLineAndColumn(out err.Line, out err.Column);

            err.Message = error + " " + s;

            // todo is this ok?
            ErrorMsg = err.Message;
            if (Errors == null) Errors = new List<ParserError>();
            Errors.Add(err);
        }

        public void AddBuildError(Expression<Func<string>> resourceExpression, CodeElement elem, params object[] parm)
        {
            string error = DotNetUtil.Res(resourceExpression, parm);
            var err = new ParserError();
            string s = string.Empty;
            if (elem != null)
                s = " " + _textBuffer.GetLineAndColumn(out err.Line, out err.Column, elem.SubString.GetFrom());

            err.Message = error + s;

            if (string.IsNullOrEmpty(ErrorMsg)) ErrorMsg = err.Message;
            if (Errors == null) Errors = new List<ParserError>();
            Errors.Add(err);
        }

        public void AddParseError(Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string error = DotNetUtil.Res(resourceExpression, parm);
            var err = new ParserError();
           
            string s =  _textBuffer.GetLineAndColumn(out err.Line, out err.Column);

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
