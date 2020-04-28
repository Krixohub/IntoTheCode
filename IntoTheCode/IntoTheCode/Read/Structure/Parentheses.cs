using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read;
using System.Linq;

namespace IntoTheCode.Read.Element
{
    /// <remarks>Inherids <see cref="SetOfElementsBase"/></remarks>
    internal class Parentheses : SetOfElementsBase
    {
        /// <summary>Creator for <see cref="Parentheses"/>.</summary>
        internal Parentheses(params ParserElementBase[] elements)
            : base(elements)
        {
            //Attributter = new ObservableCollection<Attribute>();
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new Parentheses(CloneSubElementsForParse(buffer)) { TextBuffer = buffer };
        }

        public override string GetGrammar() { return "(" + base.GetGrammar() + ")"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TextElement> outElements, int level)
        {
            return LoadSet(outElements, level);
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(TextElement last)
        {
            return ResolveSetErrorsLast(last);
        }

        public override bool ResolveErrorsForward()
        {
            return ResolveSetErrorsForward();
        }
    }
}