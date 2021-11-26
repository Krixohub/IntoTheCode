using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="ParserElementBase"/></remarks>
    internal partial class Or : ParserElementBase
    {
        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            var element = new Or(((ParserElementBase)ChildNodes[0]).CloneForParse(buffer),
                ((ParserElementBase)ChildNodes[1]).CloneForParse(buffer));
            element.TextBuffer = buffer;
            return element;
        }

        public override string GetGrammar()
        {
            return ChildNodes[0].GetGrammar() + " | " +
                ChildNodes[1].GetGrammar();
        }

        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TextElement> outElements, int level)
        {
            int from = TextBuffer.PointerNextChar;
            var subs = new List<TextElement>();
            if (!ChildNodes[0].Load(subs, level) || from == TextBuffer.PointerNextChar)
                if (TextBuffer.Status.Error != null ||
                    (!ChildNodes[1].Load(subs, level)
                    || from == TextBuffer.PointerNextChar))
                    return false;

            outElements.AddRange(subs);
            return true;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last, int level)
        {
            int rc = ChildNodes[0].ResolveErrorsLast(last, level);
            if (rc < 2)
                rc = ChildNodes[1].ResolveErrorsLast(last, level);

            return rc;
        }

        public override bool ResolveErrorsForward(int level)
        {
            bool ok = ChildNodes[0].ResolveErrorsForward(level);
            ok = ok || ChildNodes[1].ResolveErrorsForward(level);

            return ok;
        }

        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            return ChildNodes[0].InitializeLoop(rules, path, status) |
                    ChildNodes[1].InitializeLoop(rules, path, status);
        }

        public override bool InitializeLoopHasWord(RuleLink link, List<RuleLink> subPath, ref bool linkFound)
        {
            bool hasWord0 = ChildNodes[0].InitializeLoopHasWord(link, subPath, ref linkFound);
            if (linkFound) return hasWord0;

            bool hasWord1 = ChildNodes[1].InitializeLoopHasWord(link, subPath, ref linkFound);
            if (linkFound) return hasWord1;

            return hasWord0 && hasWord1;
        }
    }
}