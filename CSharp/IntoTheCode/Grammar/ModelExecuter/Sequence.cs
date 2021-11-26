using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System.Linq;
using IntoTheCode.Basic.Util;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="SetOfElementsBase"/></remarks>
    internal partial class Sequence : SetOfElementsBase
    {
        /// <summary>To create an unlinked Grammar.</summary>
        internal Sequence()
        {
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new Sequence(CloneSubElementsForParse(buffer)) { TextBuffer = buffer };
        }

        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TextElement> outElements, int level)
        {
            int p = TextBuffer.PointerNextChar;
            while (LoadSet(outElements, level) && TextBuffer.PointerNextChar > p)
                p = TextBuffer.PointerNextChar;
            //TextBuffer.PointerNextChar.CopyTo(p);

            return TextBuffer.Status.Error == null;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last, int level)
        {
            string debug = GetGrammar().NL() + last.ToMarkupProtected(string.Empty);

            int rc = ResolveSetErrorsLast(last, level);

            // If read ok, try to read further.
            if (rc == 2)
            {
                TextBuffer.GetLoopForward(null);
                return ResolveErrorsForward(0) ? 2 : 1;
            }

            return rc;
        }

        public override bool ResolveErrorsForward(int level)
        {
            int p = TextBuffer.PointerNextChar;
            while (ResolveSetErrorsForward(level) && TextBuffer.PointerNextChar > p)
                p = TextBuffer.PointerNextChar;

            return true;
        }

        public override bool InitializeLoopHasWord(RuleLink link, List<RuleLink> subPath, ref bool linkFound)
        {
            bool hasWord = false;
            foreach (ParserElementBase item in this.ChildNodes)
            {
                if (item.InitializeLoopHasWord(link, subPath, ref linkFound))
                    hasWord = true;

                if (linkFound)
                    return hasWord;
            }

            return false;
        }

        public override string GetGrammar() { return "{" + base.GetGrammar() + "}"; }
    }
}