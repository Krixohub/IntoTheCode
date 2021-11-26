using IntoTheCode.Buffer;
using System.Collections.Generic;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="SetOfElementsBase"/></remarks>
    internal class SetOfElements : SetOfElementsBase
    {
        /// <summary>Creator for <see cref="SetOfElements"/>.</summary>
        internal SetOfElements(params ParserElementBase[] elements)
            : base(elements)
        {
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new SetOfElements(CloneSubElementsForParse(buffer)) { TextBuffer = buffer };
        }

        public override string GetGrammar() { return base.GetGrammar(); }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TextElement> outElements, int level)
        {
            return LoadSet(outElements, level);
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last, int level)
        {
            return ResolveSetErrorsLast(last, level);
        }

        public override bool ResolveErrorsForward(int level)
        {
            return ResolveSetErrorsForward(level);
        }
    }
}