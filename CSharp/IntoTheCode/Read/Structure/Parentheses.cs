﻿using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read;
using System.Linq;

namespace IntoTheCode.Read.Structure
{
    /// <remarks>Inherids <see cref="SetOfElements"/></remarks>
    internal class Parentheses : SetOfElements
    {
        /// <summary>Creator for <see cref="Parentheses"/>.</summary>
        internal Parentheses(params ParserElementBase[] elements)
            : base(elements)
        {
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new Parentheses(CloneSubElementsForParse(buffer)) { TextBuffer = buffer };
        }

        public override string GetGrammar() { return "(" + base.GetGrammar() + ")"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        //public override bool Load(List<TextElement> outElements, int level)
        //{
        //    return LoadSet(outElements, level);
        //}

        ///// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        //public override int ResolveErrorsLast(TextElement last, int level)
        //{
        //    return ResolveSetErrorsLast(last, level);
        //}

        //public override bool ResolveErrorsForward(int level)
        //{
        //    return ResolveSetErrorsForward(level);
        //}
    }
}