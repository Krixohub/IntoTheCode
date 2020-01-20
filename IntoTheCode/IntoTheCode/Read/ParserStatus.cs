﻿using IntoTheCode.Basic.Util;
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
            UnambiguousPointer = new FlatPointer();
        }

        ///// <summary>Error message after parsing/reading input text.</summary>
        ///// <exclude/>
        //public string ErrorMsg { get; internal set; }

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
        public bool AddSyntaxError(WordBase element, TextPointer errorPoint, int wordCount, Expression<Func<string>> resourceExpression, params object[] parm)
        {

            string error = DotNetUtil.Msg(resourceExpression, parm.Insert(element.GetRule(element).Name));
            //            AddSyntaxError( element,  errorPoint,  wordCount,  error);
            var err = new ParserError();
            err.WordCount = wordCount;
            err.ErrorPoint = errorPoint;
            err.Error = error;

            string s = _textBuffer.GetLineAndColumn(out err.Line, out err.Column, errorPoint);

            err.Message = error + " " + s;

            AddError(err);
            return false;
        }

        //public void AddSyntaxError(WordBase element, TextPointer errorPoint, int wordCount, string error)
        //{
        //    // todo Expression<Func<string>> resourceExpression
        //    var err = new ParserError();
        //    err.WordCount = wordCount;
        //    err.ErrorPoint = errorPoint;
        //    err.Error = error;

        //    string s = _textBuffer.GetLineAndColumn(out err.Line, out err.Column, errorPoint);

        //    err.Message = "Syntax error (" +
        //                    element.GetRule(element).Name +
        //                    "). " + error + " " + s;

        //    AddError(err);

        //}

        public void AddSyntaxErrorEof(Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string error = DotNetUtil.Msg(resourceExpression, parm);
            var err = new ParserError();

            err.ErrorPoint = _textBuffer.PointerNextChar.Clone();
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
                err.ErrorPoint = elem.SubString.GetFrom();
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
            err.ErrorPoint = _textBuffer.PointerNextChar.Clone();
            err.Ex = e;

            AddError(err);
        }

        public void AddParseError(Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string error = DotNetUtil.Msg(resourceExpression, parm);
            var err = new ParserError();

            string s = _textBuffer.GetLineAndColumn(out err.Line, out err.Column);

            err.ErrorPoint = _textBuffer.PointerNextChar.Clone();
            err.Message = error + " " + s;

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
            // 
            if (oldErr == null) return newErr;

            //if (newErr.WordCount > oldErr.WordCount) return newErr;

            if (newErr.ErrorPoint != null)
            {
                if (oldErr.ErrorPoint == null) return newErr;
                if (newErr.ErrorPoint.CompareTo(oldErr.ErrorPoint) > 0) return newErr;
            }

            return oldErr;
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
