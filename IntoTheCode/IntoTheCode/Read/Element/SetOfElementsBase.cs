using System;
using System.Collections.Generic;
using System.Linq;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read;
using IntoTheCode.Read.Element.Words;
using IntoTheCode.Basic.Util;

namespace IntoTheCode.Read.Element
{
    internal abstract class SetOfElementsBase : ParserElementBase
    {
        protected SetOfElementsBase(params ParserElementBase[] elements)
        {
            if (elements.Count() == 0)
                throw new Exception("A sequence of syntax elements must contain at least one element");
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
            int from = TextBuffer.PointerNextChar;
            List<TreeNode> elements = new List<TreeNode>();
            foreach (var item in SubElements.OfType<ParserElementBase>())
                if (!item.Load(elements))
                    return SetPointerBack(from, item);

            foreach (var item in elements)
                outElements.Add(item);

            return true;
        }

        /// <summary>Find the Rule/ 'read element', that correspond to the
        /// last CodeElement, and read it again with error tracking. 
        /// If no error, try to read further.</summary>
        /// <param name="last">Not null, not empty.</param>
        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        protected internal int TryLastSetAgain(CodeElement last)
        {
            string debug = GetSyntax().NL() + last.ToMarkupProtected(string.Empty);

            int wordCount;
            int rc = 0;
            foreach (var item in SubElements.OfType<ParserElementBase>())
            {
                wordCount = 0;
                if (rc == 0)
                    rc = item.LoadFindLast(last);

                // if 'Found-read ok' then track errors further.
                else if (rc == 2 &&
                    !item.LoadTrackError(ref wordCount))
                        return 1;
            }
            
            return rc;
        }

        protected bool LoadSetTrackError(ref int wordCount)
        {
            int from = TextBuffer.PointerNextChar;
            List<WordBase> elements = new List<WordBase>();
            foreach (var item in SubElements.OfType<ParserElementBase>())
                if (!item.LoadTrackError(ref wordCount))
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
