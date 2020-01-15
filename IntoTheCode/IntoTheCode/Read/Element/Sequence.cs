using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System.Linq;

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

        public override bool TryLastAgain(CodeElement last)
        {
            //wordCount = 0;
            //if (!found && item.TryLastAgain(last))
            //    found = true;
            //if (SubNotes.SubElements == null || SubNotes.SubElements.Count() == 0)
            //{
            //}
            return TryLastSetAgain(last);
//            return true;
        }

        public override bool ExtractError(ref int wordCount)
        {
            TextPointer p = TextBuffer.PointerNextChar.Clone();
            while (ExtractErrorSet(ref wordCount) && TextBuffer.PointerNextChar.CompareTo(p) > 0)
                TextBuffer.PointerNextChar.CopyTo(p);

            return true;
        }

        public override string GetSyntax() { return "{" + base.GetSyntax() + "}"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }
    }
}