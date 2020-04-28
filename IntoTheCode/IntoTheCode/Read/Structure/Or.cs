﻿using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System;

namespace IntoTheCode.Read.Structure
{
    /// <remarks>Inherids <see cref="ParserElementBase"/></remarks>
    internal class Or : ParserElementBase
    {
        /// <summary>Creator for <see cref="Or"/>.</summary>
        internal Or(ParserElementBase element1, ParserElementBase element2)
        {
            Add(element1);
            Add(element2);
        }

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

        public override bool ResolveErrorsForward()
        {
            bool ok = ChildNodes[0].ResolveErrorsForward();
            ok = ok || ChildNodes[1].ResolveErrorsForward();

            return ok;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(TextElement last)
        {
            int rc = ChildNodes[0].ResolveErrorsLast(last);
            if (rc < 2)
                rc = ChildNodes[1].ResolveErrorsLast(last);

            return rc;
        }

        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            return ((ParserElementBase)ChildNodes[0]).InitializeLoop(rules, path, status) |
                    ((ParserElementBase)ChildNodes[1]).InitializeLoop(rules, path, status);
        }
    }
}