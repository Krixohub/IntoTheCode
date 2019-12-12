using System;
using System.Collections.Generic;
using System.Linq;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Read.Element
{
    /// <summary>A basic element (or symbol) of a syntax. Can read a peace of code/text.
    /// 
    /// </summary>
    public abstract class ParserElementBase : TreeNode
    {
        private ElementContentType _elementContent;
        public ElementContentType ElementContent
        {
            get
            {
                if (_elementContent == ElementContentType.NotSet)
                    _elementContent = GetElementContent();
                return _elementContent;
            }
        }
        public abstract ElementContentType GetElementContent();

        public override string GetValue() { return _value; }

        public virtual void Initialize() { }

        //public byte Color
        //{
        //    get;
        //    protected set;
        //}

        /// <summary>If the element cant read; use this to reset (set pointer back):</summary>
        /// <param name="proces">The load proces.</param>
        /// <param name="txtPtr">Pointer to set.</param>
        /// <param name="item">The parser item that fails to load.</param>
        /// <returns>Always return false.</returns>
        protected bool SetPointerBack(LoadProces proces, TextPointer txtPtr, ParserElementBase item)
        {
            proces.TextBuffer.SetPointer(txtPtr);
            if (txtPtr.CompareTo(proces.UnambiguousPointer) < 0 && proces.LoadError == string.Empty)
            {
                proces.LoadError = "Syntax error. ";

                // todo: delete errorWords
                var errorWords = new List<CodeElement>();
                proces.LoadErrors = new List<LoadError>();
                //item.LoadAnalyze(proces, errorWords);
                ExtractError(proces, errorWords);

                var errorMax = errorWords.FirstOrDefault();
                foreach (CodeElement word in errorWords)
                {
                    if (word.ValuePointer.ToGtPointer(errorMax.ValuePointer.GetTo()))
                        errorMax = word;
                }

                string err = proces.LoadError;
                var errorMax2 = proces.LoadErrors.FirstOrDefault();
                foreach (LoadError error in proces.LoadErrors)
                {
                    if (error.ErrorPoint.CompareTo(errorMax2.ErrorPoint) > 0)
                        errorMax2 = error;
                }

                proces.LoadError += errorMax.Error + " " + proces.TextBuffer.GetLineAndColumn(errorMax.ValuePointer.GetTo());
                err += errorMax2.Error + " " + proces.TextBuffer.GetLineAndColumn(errorMax2.ErrorPoint);
                proces.LoadError = err;
            }

            //proces.TextBuffer.SetPointerBackToFrom(subStr);
            return false;
        }

        public virtual List<AmbiguousDef> IsAmbiguousChar(ParserElementBase element, bool ws, int pos, string allowed, string disallowed)
        {
            var list = new List<AmbiguousDef>();
            return list;
        }

        public virtual List<AmbiguousDef> IsAmbiguousWord(ParserElementBase element, bool ws, int pos, string word)
        {
            var list = new List<AmbiguousDef>();
            return list;
        }

        protected List<AmbiguousDef> AmbiguousList = new List<AmbiguousDef>();

        public abstract string GetSyntax();

        // todo implement GetSettings on decendants
        public virtual string GetSettings()
        { return string.Empty; }

        //protected abstract string Read(int begin, ITextBuffer buffer);

        /// <summary>
        /// Load/read an element from the buffer, and increase the buffer pointer if element is ok.
        /// Insert output in 'outElements'.
        /// </summary>
        /// <param name="proces"></param>
        /// <param name="outElements">Read elements.</param>
        /// <returns>True = succes.</returns>
        public abstract bool Load(LoadProces proces, List<TreeNode> outElements);

        /// <summary>
        /// Load an element while storing possible errors.
        /// </summary>
        /// <param name="proces"></param>
        /// <param name="errorWords">Read elements.</param>
        /// <returns>True = succes.</returns>
        public abstract bool ExtractError(LoadProces proces, List<CodeElement> errorWords);

        protected void SkipWhiteSpace(LoadProces proces)
        {
            while (!proces.TextBuffer.IsEnd() && " \r\n\t".Contains(proces.TextBuffer.GetChar()))
                proces.TextBuffer.IncPointer();
        }

        //public abstract
    }

    public class AmbiguousDef
    {
        public int position;
        public string EndAmbiguousChars;
    }
}