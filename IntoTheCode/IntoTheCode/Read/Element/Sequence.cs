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

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            TextPointer p = proces.TextBuffer.PointerNextChar.Clone();
            while (LoadSet(proces, outElements) && proces.TextBuffer.PointerNextChar.CompareTo(p) > 0) 
                proces.TextBuffer.PointerNextChar.CopyTo(p);

            return !proces.Error;
        }

        public override bool ExtractError(LoadProces proces)
        {
            TextPointer p = proces.TextBuffer.PointerNextChar.Clone();
            while (ExtractErrorSet(proces) && proces.TextBuffer.PointerNextChar.CompareTo(p) > 0)
                proces.TextBuffer.PointerNextChar.CopyTo(p);

            return true;
        }

        public override string GetSyntax() { return "{" + base.GetSyntax() + "}"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }
    }
}