using System.Collections.Generic;

using IntoTheCode.Read;
using IntoTheCode.Basic;
using System.Linq;

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

        public override ParserElementBase CloneWithProces(LoadProces proces)
        {
            var element = new Optional(SubElements.Select(r => ((ParserElementBase)r).CloneWithProces(proces)).ToArray());// { Parser = Parser };
            element.Proces = proces;
            return element;
        }

        public override string GetSyntax() { return "[" + base.GetSyntax() + "]"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            List<TreeNode> elements = new List<TreeNode>();
            if (LoadSet(proces, outElements))
                outElements.AddRange(elements);
            return !proces.Error;
        }

        public override bool ExtractError(LoadProces proces)
        {
            ExtractErrorSet(proces);
            return true;
        }
    }
}