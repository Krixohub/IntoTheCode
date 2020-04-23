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
    /// <remarks>Inherids <see cref="ParserElementBase"/></remarks>
    public abstract class SetOfElementsBase : ParserElementBase
    {
        protected SetOfElementsBase(params ParserElementBase[] elements)
        {
            if (elements.Count() == 0)
                throw new Exception("A sequence of Grammar elements must contain at least one element");
            Add(elements);
        }

        /// <summary>To create unlinked Grammar.</summary>
        protected SetOfElementsBase()
        {
        }

        protected ParserElementBase[] CloneSubElementsForParse(TextBuffer buffer)
        {
            return SubElements.Select(r => ((ParserElementBase)r).CloneForParse(buffer)).ToArray();
        }

        protected bool LoadSet(List<TextElement> outElements, int level)
        {
            int from = TextBuffer.PointerNextChar;
            var elements = new List<TextElement>();
            foreach (var item in SubElements.OfType<ParserElementBase>())
                if (!item.Load(elements, level))
                    return SetPointerBackSet(from, item, outElements);

            foreach (var item in elements)
                outElements.Add(item);

            return true;
        }

        private bool SetPointerBackSet(int from, ParserElementBase failItem, List<TextElement> outElements)
        {
            int ptrFail = TextBuffer.PointerNextChar;

            // todo: Is the error fatal?
            if (from < TextBuffer.Status.UnambiguousPointer && TextBuffer.Status.Error == null)
            {
                int failIndex = SubElements.IndexOf(failItem);

                TextElement last = outElements.LastOrDefault();
                if (last != null)
                    for (int i = failIndex - 1; i > -1; i--)
                        if (((ParserElementBase)SubElements[i]).ResolveErrorsLast(last) != 0)
                            break;

                TextBuffer.PointerNextChar = ptrFail;

                for (int i = failIndex; i < SubElements.Count; i++)
                    if (!((ParserElementBase)SubElements[i]).ResolveErrorsForward())
                        break;
            }

            TextBuffer.PointerNextChar = from;
            return false;
        }


        /// <summary>Find the Rule/ 'read element', that correspond to the
        /// last CodeElement, and read it again with error resolving. 
        /// If no error, try to read further.</summary>
        /// <param name="last">Not null, not empty.</param>
        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        protected internal int ResolveSetErrorsLast(TextElement last)
        {
            string debug = GetGrammar().NL() + last.ToMarkupProtected(string.Empty);

            int rc = 0;
            foreach (var item in SubElements.OfType<ParserElementBase>())
            {
                if (rc == 0)
                    rc = item.ResolveErrorsLast(last);

                // if 'Found-read ok' then track errors further.
                else if (rc == 2 &&
                    !item.ResolveErrorsForward())
                    //!item.LoadTrackError(ref wordCount))
                    return 1;
            }

            return rc;
        }

        public bool ResolveSetErrorsForward()
        {
            int from = TextBuffer.PointerNextChar;

            foreach (var item in SubElements.OfType<ParserElementBase>())
                if (!item.ResolveErrorsForward())
                    return SetPointerBack(from);
            return true;
        }

        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            bool ok = true;
            foreach (ParserElementBase item in this.SubElements.OfType<ParserElementBase>())
                ok = ok && item.InitializeLoop(rules, path, status);
            return ok;
        }

        public /*override*/ bool GetFirstList(ParserElementBase org, List<ParserElementBase> firstList, List<ParserElementBase> followingList)
        {
            // this set is optional if alle elements are optional.
            bool isOption = true;

            for (int i = SubElements.Count - 1; i >= 0; i--)
            {
               // isOption = isOption && ((ParserElementBase)SubElements[i]).GetFirstList(org, firstList, followingList);

                // todo
            }

            return isOption;
        }

        public override string GetGrammar()
        {
            string Grammar = string.Join(" ", SubElements.OfType<ParserElementBase>().Select(s => s.GetGrammar()).ToArray());
            return Grammar;
        }
    }
}
