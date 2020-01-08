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

        public override ParserElementBase CloneWithProces(LoadProces proces)
        {
            return new Parentheses(CloneSubElementsWithProces(proces)) { Proces = proces };
        }

        public override string GetSyntax() { return "(" + base.GetSyntax() + ")"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            return LoadSet(proces, outElements);
        }

        public override bool ExtractError(LoadProces proces)
        {
            return ExtractErrorSet(proces);
        }
    }
}