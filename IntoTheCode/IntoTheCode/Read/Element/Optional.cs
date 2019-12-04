using System.Collections.Generic;

using IntoTheCode.Read;
using IntoTheCode.Basic;

namespace IntoTheCode.Read.Element
{
    internal class Optional : SequenceBase
    {
        /// <summary>Creator for <see cref="Optional"/>.</summary>
        internal Optional(params ParserElementBase[] elements)
            : base(elements)
        {
            //Attributter = new ObservableCollection<Attribute>();
        }

        public override string GetSyntax() { return "[" + base.GetSyntax() + "]"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            List<TreeNode> elements = new List<TreeNode>();
            if (LoadSet(proces, outElements))
                outElements.AddRange(elements);
            return true;
        }
    }
}