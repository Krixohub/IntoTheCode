﻿using System;
using System.Collections.Generic;
using System.Linq;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Read.Element
{
    internal abstract class SequenceBase : ParserElementBase
    {
        protected SequenceBase(params ParserElementBase[] elements)
        {
            if (elements.Count() == 0)
                throw new Exception("A sequence of syntax elements must contain at least one element");
            //            Elements = new List<SyntaxElement>();
            Add(elements);
        }

        /// <summary>To create unlinked syntax.</summary>
        protected SequenceBase()
        {
        }

        public override ElementContentType GetElementContent()
        {
            return ElementContentType.Many;
        }

        protected bool LoadSet(LoadProces proces, List<TreeNode> outElements)
        {
            //TextSubString ptr = new TextSubString { From = buf.pointer };
            //TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar.Clone();
            List<TreeNode> elements = new List<TreeNode>();
            //int i = SubElements.Count;
            foreach (var item in SubElements.OfType<ParserElementBase>())
                if (!item.Load(proces, elements))
                    //if (!item.Load(buf, elements))
                    return SetPointerBack(proces, from, item);
                //else
                //    from = proces.TextBuffer.PointerNextChar.Clone();

            foreach (var item in elements)
                outElements.Add(item);

            return true;
        }

        protected bool LoadSetAnalyze(LoadProces proces)
        {
            //TextSubString ptr = new TextSubString { From = buf.pointer };
            //TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar.Clone();
            List<WordBase> elements = new List<WordBase>();
            //int i = SubElements.Count;
            foreach (var item in SubElements.OfType<ParserElementBase>())
                if (!item.ExtractError(proces))
                    //if (!item.Load(buf, elements))
                    return SetPointerBack(proces, from, item);
            

            return true;
        }

        public override string GetSyntax()
        {
            string syntax = string.Join(" ", SubElements.OfType<ParserElementBase>().Select(s => s.GetSyntax()).ToArray());
            return syntax;
        }
    }
}
