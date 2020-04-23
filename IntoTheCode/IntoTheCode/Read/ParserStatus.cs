using IntoTheCode.Basic.Util;
using IntoTheCode.Buffer;
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
        private TextBuffer _textBuffer;

        internal ParserStatus(TextBuffer buf)
        {
            _textBuffer = buf;
            UnambiguousPointer = TextBuffer.NotValidPtr;
        }

        /// <summary>Error message after parsing/reading input text.</summary>
        /// <exclude/>
        public ParserError Error { get; private set; }

        /// <summary>Error message after parsing/reading input text.</summary>
        /// <exclude/>
        public List<ParserError> AllErrors { get; internal set; }

        #region add errors

        /// <summary>Add an error from parserElement.</summary>
        /// <param name="element"></param>
        /// <param name="errorPoint"></param>
        /// <param name="wordCount"></param>
        /// <param name="resourceExpression"></param>
        /// <param name="parm"></param>
        /// <returns>Always false.</returns>
        public bool AddSyntaxError(WordBase element, int errorPoint, int wordCount, Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string error = DotNetUtil.Msg(resourceExpression, parm.Insert(element.GetRule(element).Name));
            //            AddSyntaxError( element,  errorPoint,  wordCount,  error);
            var err = new ParserError();
            err.WordCount = 0;
            err.ErrorPoint = errorPoint;
            err.Error = error;

            string s = _textBuffer.GetLineAndColumn(out err.Line, out err.Column, errorPoint);

            err.Message = error + " " + s;

            AddError(err);
            return false;
        }

        public void AddSyntaxErrorEof(Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string error = DotNetUtil.Msg(resourceExpression, parm);
            var err = new ParserError();

            err.ErrorPoint = _textBuffer.PointerNextChar;
            string s = _textBuffer.GetLineAndColumn(out err.Line, out err.Column);

            err.Message = error + " " + s;

            AddError(err);
        }

        public void AddBuildError(Expression<Func<string>> resourceExpression, CodeElement elem, params object[] parm)
        {
            string error = DotNetUtil.Msg(resourceExpression, parm);
            var err = new ParserError();
            string s = string.Empty;
            if (elem != null)
            {
                err.ErrorPoint = elem.SubString.From;
                s = " " + _textBuffer.GetLineAndColumn(out err.Line, out err.Column, err.ErrorPoint);
            }
            err.Message = error + s;

            AddError(err);
        }

        public void AddException(Exception e, Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string error = DotNetUtil.Msg(resourceExpression, parm);
            var err = new ParserError();
            string s = _textBuffer.GetLineAndColumn(out err.Line, out err.Column);
            err.Message = error + " " + s;
            err.ErrorPoint = _textBuffer.PointerNextChar;
            err.Ex = e;

            AddError(err);
        }

        public void AddParseError(Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string error = DotNetUtil.Msg(resourceExpression, parm);
            var err = new ParserError();

            string s = _textBuffer.GetLineAndColumn(out err.Line, out err.Column);

            err.ErrorPoint = _textBuffer.PointerNextChar;
            err.Message = error + " " + s;

            AddError(err);
        }

        public void AddFlatError(Expression<Func<string>> resourceExpression, params object[] parm)
        {
            var err = new ParserError()
            {
                Message = DotNetUtil.Msg(resourceExpression, parm),
                ErrorPoint = TextBuffer.NotValidPtr
            };

            AddError(err);
        }

        private void AddError(ParserError err)
        {
            if (AllErrors == null) AllErrors = new List<ParserError>();
            AllErrors.Add(err);
            Error = RankingError(err, Error);
        }

        /// <summary>Find highest ranking error.</summary>
        /// <param name="newErr">Not null error.</param>
        /// <param name="oldErr">Previus ranking error.</param>
        /// <returns>Ranking error.</returns>
        private ParserError RankingError(ParserError newErr, ParserError oldErr)
        {
            if (oldErr == null) return newErr;

            return ParserError.Compare(newErr, oldErr) < 0 ? newErr : oldErr;
        }

        #endregion add errors

        #region Next 

        public void ThisIsUnambiguous(ParserElementBase reader, TextElement code)
        {
            // Set SafePointer to char after element
            if (code is CodeElement) UnambiguousPointer = ((CodeElement)code).SubString.To;
            if (code is CommentElement) UnambiguousPointer = ((CommentElement)code).SubString.To;
        }

        public int UnambiguousPointer;

        #endregion Next 
    }
}
