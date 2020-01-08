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

        public abstract ParserElementBase CloneWithProces(LoadProces proces);

        public override string GetValue() { return _value; }

        public virtual void Initialize() { }

        protected LoadProces Proces;

        /// <summary>Clone this parser element, with sub elements, and set proces.</summary>
        /// <param name="proces">The load proces.</param>
        /// <returns>The new clone.</returns>
        //public ParserElementBase CloneWithProces(LoadProces proces)
        ////element.Proces = proces;
        ////element._elementContent = ElementContent;
        //{
        //    ParserElementBase element;
        //    try
        //    {

        //         element = Clone();
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //    //element.Proces = proces;
        //    //element._elementContent = ElementContent;
        //    //if (SubElements != null)
        //    //    element.Add(SubElements.Select(r => ((ParserElementBase)r).CloneWithProces(proces)));
        //    return element;
        //}

        //public byte Color
        //{
        //    get;
        //    protected set;
        //}

        /// <summary>If the element cant read; use this to reset (set pointer back):</summary>
        /// <param name="proces">The load proces.</param>
        /// <param name="txtPtr">Pointer to set.</param>
        /// <returns>Always return false.</returns>
        protected bool SetPointerBack(LoadProces proces, TextPointer txtPtr, ParserElementBase item)
        {
            proces.TextBuffer.SetPointer(txtPtr);
            if (txtPtr.CompareTo(proces.UnambiguousPointer) < 0 && !proces.Error)
            {
                ExtractError(proces);

                proces.Errors = proces.Errors.OrderByDescending(e => e.ErrorPoint.CompareTo(txtPtr)).ToList();
                var errorMax2 = proces.Errors.FirstOrDefault();

                proces.ErrorMsg = errorMax2.Message;
                //proces.ErrorMsg = "Syntax error (" +
                //    GetRule(this).Name +
                //    "). " + errorMax2.Error + " " + 
                //    proces.TextBuffer.GetLineAndColumn(errorMax2.ErrorPoint);
            }

            return false;
        }

        /// <summary>If the element cant read; use this to reset pointer, after an error has occured.</summary>
        /// <param name="proces">The load proces.</param>
        /// <param name="txtPtr">Pointer to set.</param>
        /// <returns>Always return false.</returns>
        protected bool SetPointerBackError(LoadProces proces, TextPointer txtPtr)
        {
            //if (this is WordBase)
            //    proces.Errors.Add(new LoadError((WordBase)this, proces.TextBuffer.PointerNextChar.Clone(), 2, "Unexpected string."));
            //if (proces.TextBuffer.PointerNextChar.CompareTo())
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
        /// <returns>True = succes.</returns>
        public abstract bool ExtractError(LoadProces proces);

        // todo:2 consider remove this method to parser.
        internal protected void SkipWhiteSpace(LoadProces proces)
        {
            while (!proces.TextBuffer.IsEnd() && " \r\n\t".Contains(proces.TextBuffer.GetChar()))
                proces.TextBuffer.IncPointer();
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