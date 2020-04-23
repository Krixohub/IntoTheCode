using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System;

namespace IntoTheCode.Read.Element
{
    /// <remarks>Inherids <see cref="ParserElementBase"/></remarks>
    internal class Or : ParserElementBase
    {
        /// <summary>Creator for <see cref="Or"/>.</summary>
        internal Or(ParserElementBase element1, ParserElementBase element2)
        {
            AddElement(element1);
            AddElement(element2);
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            var element = new Or(((ParserElementBase)SubElements[0]).CloneForParse(buffer),
                ((ParserElementBase)SubElements[1]).CloneForParse(buffer));
            element.TextBuffer = buffer;
            return element;
        }

        public override string GetGrammar()
        {
            return (SubElements[0] as ParserElementBase).GetGrammar() + " | " +
                (SubElements[1] as ParserElementBase).GetGrammar();
        }

        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TextElement> outElements, int level)
        {
            int from = TextBuffer.PointerNextChar;
            var subs = new List<TextElement>();
            if (!(SubElements[0] as ParserElementBase).Load(subs, level) || from == TextBuffer.PointerNextChar)
                if (TextBuffer.Status.Error != null ||
                    (!(SubElements[1] as ParserElementBase).Load(subs, level)
                    || from == TextBuffer.PointerNextChar))
                    return false;

            outElements.AddRange(subs);
            return true;
        }

        public override bool ResolveErrorsForward()
        {
            bool ok = (SubElements[0] as ParserElementBase).ResolveErrorsForward();
            ok = ok || (SubElements[1] as ParserElementBase).ResolveErrorsForward();

            return ok;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(TextElement last)
        {
            int rc = (SubElements[0] as ParserElementBase).ResolveErrorsLast(last);
            if (rc < 2)
                rc = (SubElements[1] as ParserElementBase).ResolveErrorsLast(last);

            return rc;
        }

        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            return ((ParserElementBase)SubElements[0]).InitializeLoop(rules, path, status) |
                    ((ParserElementBase)SubElements[1]).InitializeLoop(rules, path, status);
        }
    }
}