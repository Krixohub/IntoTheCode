using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System.Linq;
using IntoTheCode.Basic.Util;

namespace IntoTheCode.Read.Element
{
    /// <remarks>Inherids <see cref="SetOfElementsBase"/></remarks>
    internal class Sequence : SetOfElementsBase
    {
        /// <summary>Creator for <see cref="Sequence"/>.</summary>
        internal Sequence(params ParserElementBase[] elements) : base(elements)
        {
            //Attributter = new ObservableCollection<Attribute>();
        }

        /// <summary>To create an unlinked Grammar.</summary>
        internal Sequence()
        {
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new Sequence(CloneSubElementsForParse(buffer)) { TextBuffer = buffer };
        }

        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<CodeElement> outElements, int level)
        {
            int p = TextBuffer.PointerNextChar;
            while (LoadSet(outElements, level) && TextBuffer.PointerNextChar > p)
                p = TextBuffer.PointerNextChar;
            //TextBuffer.PointerNextChar.CopyTo(p);

            return TextBuffer.Status.Error == null;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last)
        {
            string debug = GetGrammar().NL() + last.ToMarkupProtected(string.Empty);

            int rc = ResolveSetErrorsLast(last);

            // If read ok, try to read further.
            if (rc == 2)
                return ResolveErrorsForward() ? 2 : 1;

            return rc;
        }

        public override bool ResolveErrorsForward()
        {
            int p = TextBuffer.PointerNextChar;
            while (ResolveSetErrorsForward() && TextBuffer.PointerNextChar > p)
                p = TextBuffer.PointerNextChar;

            return true;
        }


        public override string GetGrammar() { return "{" + base.GetGrammar() + "}"; }
    }
}