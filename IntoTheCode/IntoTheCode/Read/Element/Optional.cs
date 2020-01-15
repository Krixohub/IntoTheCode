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

        public override ParserElementBase CloneForParse(ITextBuffer buffer)
        {
            var element = new Optional(SubElements.Select(r => ((ParserElementBase)r).CloneForParse(buffer)).ToArray());// { Parser = Parser };
            element.TextBuffer = buffer;
            return element;
        }

        public override string GetSyntax() { return "[" + base.GetSyntax() + "]"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TreeNode> outElements)
        {
            List<TreeNode> elements = new List<TreeNode>();
            if (LoadSet(outElements))
                outElements.AddRange(elements);
            return TextBuffer.Status.Error == null;
        }

        public override bool TryLastAgain(CodeElement last)
        {
            TryLastSetAgain(last);
            return true;
        }

        public override bool ExtractError(ref int wordCount)
        {
            ExtractErrorSet(ref wordCount);
            return true;
        }
    }
}