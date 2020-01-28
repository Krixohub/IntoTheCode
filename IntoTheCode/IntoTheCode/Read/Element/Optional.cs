using System.Collections.Generic;

using IntoTheCode.Read;
using IntoTheCode.Basic;
using System.Linq;
using IntoTheCode.Buffer;

namespace IntoTheCode.Read.Element
{
    internal class Optional : SetOfElementsBase
    {
        /// <summary>Creator for <see cref="Optional"/>.</summary>
        internal Optional(params ParserElementBase[] elements)
            : base(elements)
        {
            //Attributter = new ObservableCollection<Attribute>();
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            var element = new Optional(SubElements.Select(r => ((ParserElementBase)r).CloneForParse(buffer)).ToArray());// { Parser = Parser };
            element.TextBuffer = buffer;
            return element;
        }

        public override string GetGrammar() { return "[" + base.GetGrammar() + "]"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TreeNode> outElements, int level)
        {
            List<TreeNode> elements = new List<TreeNode>();
            if (LoadSet(outElements, level))
                outElements.AddRange(elements);
            return TextBuffer.Status.Error == null;
        }

        /// <summary>Find the Rule/ 'read element', that correspond to the
        /// last CodeElement, and read it again with error tracking. 
        /// If no error, try to read further.</summary>
        /// <param name="last">Not null, not empty.</param>
        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int LoadFindLast(CodeElement last)
        {
            return TryLastSetAgain(last);
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            LoadSetTrackError(ref wordCount);
            return true;
        }
    }
}