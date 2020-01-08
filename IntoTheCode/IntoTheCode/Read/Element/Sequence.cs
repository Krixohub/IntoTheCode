using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;

namespace IntoTheCode.Read.Element
{
    internal class Sequence : SetOfElementsBase
    {
        /// <summary>Creator for <see cref="Sequence"/>.</summary>
        internal Sequence(params ParserElementBase[] elements) : base(elements)
        {
            //Attributter = new ObservableCollection<Attribute>();
        }

        /// <summary>To create an unlinked syntax.</summary>
        internal Sequence()
        {
        }

        public override ParserElementBase CloneWithProces(LoadProces proces)
        {
            return new Sequence(CloneSubElementsWithProces(proces)) { Proces = proces };
        }

        public override bool Load(List<TreeNode> outElements)
        {
            TextPointer p = Proces.TextBuffer.PointerNextChar.Clone();
            while (LoadSet(outElements) && Proces.TextBuffer.PointerNextChar.CompareTo(p) > 0)
                Proces.TextBuffer.PointerNextChar.CopyTo(p);

            return !Proces.Error;
        }

        public override bool ExtractError()
        {
            TextPointer p = Proces.TextBuffer.PointerNextChar.Clone();
            while (ExtractErrorSet() && Proces.TextBuffer.PointerNextChar.CompareTo(p) > 0)
                Proces.TextBuffer.PointerNextChar.CopyTo(p);

            return true;
        }

        public override string GetSyntax() { return "{" + base.GetSyntax() + "}"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }
    }
}