﻿using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System;

namespace IntoTheCode.Read.Element
{
    internal class Or : ParserElementBase
    {
        /// <summary>Creator for <see cref="Sequence"/>.</summary>
        internal Or(ParserElementBase element1, ParserElementBase element2) 
        {
            AddElement(element1);
            AddElement(element2);
            //if (Element1.ElementContent == ElementContentType.OneValue && Element2.ElementContent == ElementContentType.OneValue)
            //    ElementContent = ElementContentType.OneValue;
            //else
            //    ElementContent = ElementContentType.Many;
        }

        public override ElementContentType GetElementContent()
        {
            // todo fejl ved Rule.Tag = true
            return
                //Element1.ElementContent == ElementContentType.OneValue &&
                //Element2.ElementContent == ElementContentType.OneValue ?
                //ElementContentType.OneValue :
                ElementContentType.Many;
        }

        public override string GetSyntax() {
            return (SubElements[0] as ParserElementBase).GetSyntax() + " | " +
                (SubElements[1] as ParserElementBase).GetSyntax();
        }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            //TextSubString ptr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar;
            List<TreeNode> subs = new List<TreeNode>();
            if (!(SubElements[0] as ParserElementBase).Load(proces, subs))
                if (!(SubElements[1] as ParserElementBase).Load(proces, subs))
                    return false;

            outElements.AddRange(subs);
            return true;
        }

        public override bool LoadAnalyze(LoadProces proces, List<CodeElement> errorWords)
        {
            bool ok = (SubElements[0] as ParserElementBase).LoadAnalyze(proces, errorWords);
            ok = ok || (SubElements[1] as ParserElementBase).LoadAnalyze(proces, errorWords);

            return ok;
        }
    }
}