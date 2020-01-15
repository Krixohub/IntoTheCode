using System.Collections.Generic;

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

        ///// <summary>Just for clone().</summary>
        //internal Or()
        //{
        //}

        public override ParserElementBase CloneForParse(ITextBuffer buffer)
        {
            var element = new Or(((ParserElementBase)SubElements[0]).CloneForParse(buffer),
                ((ParserElementBase)SubElements[1]).CloneForParse(buffer));
            element.TextBuffer = buffer;
            return element;
        }

        //public override ParserElementBase Clone()
        //{
        //    return new Or();
        //}

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

        public override bool Load(List<TreeNode> outElements)
        {
            //TextSubString ptr = TextBuffer.NewSubStringFrom();
            TextPointer from = TextBuffer.PointerNextChar.Clone();
            List<TreeNode> subs = new List<TreeNode>();
            if (!(SubElements[0] as ParserElementBase).Load(subs) || from.CompareTo(TextBuffer.PointerNextChar) == 0)
                if (TextBuffer.Status.Error != null || 
                    (!(SubElements[1] as ParserElementBase).Load(subs) 
                    || from.CompareTo(TextBuffer.PointerNextChar) == 0))
                    return false;
            
            outElements.AddRange(subs);
            return true;
        }

        public override bool ExtractError()
        {
            bool ok = (SubElements[0] as ParserElementBase).ExtractError();
            ok = ok || (SubElements[1] as ParserElementBase).ExtractError();

            return ok;
        }
    }
}