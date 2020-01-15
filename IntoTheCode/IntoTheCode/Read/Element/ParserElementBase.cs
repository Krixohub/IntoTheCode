﻿using System;
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

        /// <summary>Clone this parser element, with sub elements, and set proces.</summary>
        /// <param name="proces">The load proces.</param>
        /// <returns>The new clone.</returns>
        public abstract ParserElementBase CloneForParse(ITextBuffer buffer);

        public override string GetValue() { return _value; }

       // public virtual void Initialize() { }

        protected internal ITextBuffer TextBuffer;

        protected internal CodeElement DefinitionCodeElement;

        //public byte Color
        //{
        //    get;
        //    protected set;
        //}

        /// <summary>If the element cant read; use this to reset (set pointer back):</summary>
        /// <param name="proces">The load proces.</param>
        /// <param name="txtPtr">Pointer to set.</param>
        /// <returns>Always return false.</returns>
        protected bool SetPointerBack(TextPointer txtPtr, ParserElementBase item)
        {
            TextBuffer.SetPointer(txtPtr);
            if (txtPtr.CompareTo(TextBuffer.Status.UnambiguousPointer) < 0 && TextBuffer.Status.Error == null)
            {
                int wordCount = 0; // count words from this point
                ExtractError(ref wordCount);
            }

            return false;
        }

        /// <summary>If the element cant read; use this to reset pointer, after an error has occured.</summary>
        /// <param name="proces">The load proces.</param>
        /// <param name="txtPtr">Pointer to set.</param>
        /// <returns>Always return false.</returns>
        protected bool SetPointerBackError(TextPointer txtPtr, ref int wordCount, int previusWordCount)
        {
            wordCount = previusWordCount;
            TextBuffer.SetPointer(txtPtr);
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
        public abstract bool Load(List<TreeNode> outElements);


        /// <summary>find errors from start, when eof isn't reached and no other error is found.</summary>
        /// <param name="last">Not null, not empty.</param>
        /// <returns>True if load ok. False if load error.</returns>
        public abstract bool TryLastAgain(CodeElement last);

        /// <summary>
        /// Load an element while storing possible errors.
        /// </summary>
        /// <param name="proces"></param>
        /// <returns>True = succes.</returns>
        public abstract bool ExtractError(ref int wordCount);

        // todo:2 consider remove this method to parser.
        internal protected void SkipWhiteSpace()
        {
            while (!TextBuffer.IsEnd() && " \r\n\t".Contains(TextBuffer.GetChar()))
                TextBuffer.IncPointer();
        }

        internal Rule GetRule(ParserElementBase e)
        {
            if (e is Rule) return (Rule)e;
            else return GetRule((ParserElementBase)e.Parent);
        }
    }

    public class AmbiguousDef
    {
        public int position;
        public string EndAmbiguousChars;
    }
}