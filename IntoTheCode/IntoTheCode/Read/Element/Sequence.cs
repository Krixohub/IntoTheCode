using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System.Linq;
using IntoTheCode.Basic.Util;

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

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int TryLastAgain(CodeElement last)
        {
            string debug = GetSyntax().NL() + last.ToMarkupProtected(string.Empty);

            int rc = TryLastSetAgain(last);

            // If read ok, try to read further.
            if (rc == 2)
            {
                int wordCount = 0;
                //TextBuffer.SetPointer(last.SubString.GetTo().Clone(1));
                return LoadTrackError(ref wordCount) ? 2 : 1;
            }

            return rc;
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            TextPointer p = TextBuffer.PointerNextChar.Clone();
            while (LoadSetTrackError(ref wordCount) && TextBuffer.PointerNextChar.CompareTo(p) > 0)
                TextBuffer.PointerNextChar.CopyTo(p);

            return true;
        }

        public override string GetSyntax() { return "{" + base.GetSyntax() + "}"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }
    }
}