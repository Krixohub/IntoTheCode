using System;
using System.Collections.Generic;
using System.Linq;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Read.Element
{
    internal abstract class SetOfElementsBase : ParserElementBase
    {
        protected SetOfElementsBase(params ParserElementBase[] elements)
        {
            if (elements.Count() == 0)
                throw new Exception("A sequence of syntax elements must contain at least one element");
            //            Elements = new List<SyntaxElement>();
            Add(elements);
        }

        /// <summary>To create unlinked syntax.</summary>
        protected SetOfElementsBase()
        {
        }

        protected ParserElementBase[] CloneSubElementsWithProces(LoadProces proces)
        {
            return SubElements.Select(r => ((ParserElementBase)r).CloneWithProces(proces)).ToArray();
        }

        public override ElementContentType GetElementContent()
        {
            return ElementContentType.Many;
        }

        protected bool LoadSet(LoadProces proces, List<TreeNode> outElements)
        {
            TextPointer from = proces.TextBuffer.PointerNextChar.Clone();
            List<TreeNode> elements = new List<TreeNode>();
            foreach (var item in SubElements.OfType<ParserElementBase>())
                if (!item.Load(proces, elements))
                    return SetPointerBack(proces, from, item);

            foreach (var item in elements)
                outElements.Add(item);

            return true;
        }

        protected bool ExtractErrorSet(LoadProces proces)
        {
            TextPointer from = proces.TextBuffer.PointerNextChar.Clone();
            List<WordBase> elements = new List<WordBase>();
            foreach (var item in SubElements.OfType<ParserElementBase>())
                if (!item.ExtractError(proces))
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
