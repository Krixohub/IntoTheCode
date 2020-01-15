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

        protected ParserElementBase[] CloneSubElementsForParse(ITextBuffer buffer)
        {
            return SubElements.Select(r => ((ParserElementBase)r).CloneForParse(buffer)).ToArray();
        }

        public override ElementContentType GetElementContent()
        {
            return ElementContentType.Many;
        }

        protected bool LoadSet(List<TreeNode> outElements)
        {
            TextPointer from = TextBuffer.PointerNextChar.Clone();
            List<TreeNode> elements = new List<TreeNode>();
            foreach (var item in SubElements.OfType<ParserElementBase>())
                if (!item.Load(elements))
                    return SetPointerBack(from, item);

            foreach (var item in elements)
                outElements.Add(item);

            return true;
        }

        protected internal bool TryLastSetAgain(CodeElement last)
        {
            bool found = false;
            //CodeElement last = outElements as CodeElement;
            int wordCount;
            foreach (var item in SubElements.OfType<ParserElementBase>())
            {
                wordCount = 0;
                if (!found && item.TryLastAgain(last))
                    found = true;
                if (found && last.SubElements == null || last.SubElements.Count() == 0)
                {
                    TextBuffer.SetPointerBackToFrom(last.SubString);
                    if (!item.ExtractError(ref wordCount)) return true;
                }
            }
            

            return true;
        }

        protected bool ExtractErrorSet(ref int wordCount)
        {
            TextPointer from = TextBuffer.PointerNextChar.Clone();
            List<WordBase> elements = new List<WordBase>();
            foreach (var item in SubElements.OfType<ParserElementBase>())
                if (!item.ExtractError(ref wordCount))
                    return SetPointerBack(from, item);

            return true;
        }

        public override string GetSyntax()
        {
            string syntax = string.Join(" ", SubElements.OfType<ParserElementBase>().Select(s => s.GetSyntax()).ToArray());
            return syntax;
        }
    }
}
