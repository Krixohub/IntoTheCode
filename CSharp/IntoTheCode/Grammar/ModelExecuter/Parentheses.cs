﻿using IntoTheCode.Buffer;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="SetOfElements"/></remarks>
    internal partial class Parentheses : SetOfElements
    {
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