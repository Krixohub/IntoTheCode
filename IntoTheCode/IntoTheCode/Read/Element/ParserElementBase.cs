using System;
using System.Collections.Generic;
using System.Linq;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;

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

        //public byte Color
        //{
        //    get;
        //    protected set;
        //}

        /// <summary>If the element cant read; use this to reset (set pointer back):</summary>
        /// <param name="proces"></param>
        /// <param name="subStr"></param>
        /// <returns></returns>
        protected bool SetPointerBack(LoadProces proces, TextPointer txtPtr)
        {
            if (txtPtr.CompareTo(proces.UnambiguousPointer) < 0)
            {
                throw new SyntaxErrorException("syntax error at ...");
            }
            //proces.TextBuffer.SetPointerBackToFrom(subStr);
            proces.TextBuffer.SetPointer(txtPtr);
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

        public virtual string GetSettings()
        { return string.Empty; }

        //protected abstract string Read(int begin, ITextBuffer buffer);

        /// <summary>
        /// Read an element from the buffer, and increase the buffer pointer if element is ok.
        /// Insert output in 'outElements'.
        /// </summary>
        /// <param name="outElements">Read elements.</param>
        /// <returns>True = succes.</returns>
        public abstract bool Load(LoadProces proces, List<TreeNode> outElements);

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