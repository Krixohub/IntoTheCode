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

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new Sequence(CloneSubElementsForParse(buffer)) { TextBuffer = buffer };
        }

        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TreeNode> outElements)
        {
            int p = TextBuffer.PointerNextChar;
            while (LoadSet(outElements) && TextBuffer.PointerNextChar.CompareTo(p) > 0)
                p = TextBuffer.PointerNextChar;
            //TextBuffer.PointerNextChar.CopyTo(p);

            return TextBuffer.Status.Error == null;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int LoadFindLast(CodeElement last)
        {
            string debug = GetSyntax().NL() + last.ToMarkupProtected(string.Empty);

            int rc = TryLastSetAgain(last);

            // If read ok, try to read further.
            if (rc == 2)
            {
                int wordCount = 0;
                return LoadTrackError(ref wordCount) ? 2 : 1;
            }

            return rc;
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            int p = TextBuffer.PointerNextChar;
            while (LoadSetTrackError(ref wordCount) && TextBuffer.PointerNextChar.CompareTo(p) > 0)
                p = TextBuffer.PointerNextChar;
            //TextBuffer.PointerNextChar.CopyTo(p);

            return true;
        }

        public override string GetSyntax() { return "{" + base.GetSyntax() + "}"; }
    }
}