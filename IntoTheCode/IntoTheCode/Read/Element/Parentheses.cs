using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read;
using System.Linq;

namespace IntoTheCode.Read.Element
{
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

        public override string GetSyntax() { return "(" + base.GetSyntax() + ")"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TreeNode> outElements)
        {
            return LoadSet(outElements);
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int LoadFindLast(CodeElement last)
        {
            return TryLastSetAgain(last);
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            return LoadSetTrackError(ref wordCount);
        }
    }
}