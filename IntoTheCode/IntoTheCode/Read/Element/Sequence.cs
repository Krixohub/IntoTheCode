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

        public override ParserElementBase CloneForParse(ITextBuffer buffer)
        {
            return new Sequence(CloneSubElementsForParse(buffer)) { TextBuffer = buffer };
        }

        public override bool Load(List<TreeNode> outElements)
        {
            TextPointer p = TextBuffer.PointerNextChar.Clone();
            while (LoadSet(outElements) && TextBuffer.PointerNextChar.CompareTo(p) > 0)
                TextBuffer.PointerNextChar.CopyTo(p);

            return TextBuffer.Status.Error == null;
        }

        public override bool ExtractError()
        {
            TextPointer p = TextBuffer.PointerNextChar.Clone();
            while (ExtractErrorSet() && TextBuffer.PointerNextChar.CompareTo(p) > 0)
                TextBuffer.PointerNextChar.CopyTo(p);

            return true;
        }

        public override string GetSyntax() { return "{" + base.GetSyntax() + "}"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }
    }
}