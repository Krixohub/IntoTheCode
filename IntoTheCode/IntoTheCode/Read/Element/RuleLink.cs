using System.Collections.Generic;
using IntoTheCode.Basic;

namespace IntoTheCode.Read.Element
{
    /// <summary>
    /// Base class for syntax symbols.
    /// </summary>
    internal class RuleLink : ParserElementBase
    {
        /// <summary>Creator for <see cref="RuleLink"/>.</summary>
        internal RuleLink(string name)
        {
            Name = "name";
            _value = name;
        }

        /// <summary>The Reader has the current pointer of reading, and the context.</summary>
        internal ParserElementBase SymbolElement;

        public override ElementContentType GetElementContent()
        {
            return SymbolElement != null ?
                SymbolElement.ElementContent :
                ElementContentType.NotSet;
        }

        public override string GetSyntax()
        {
            return GetValue();
        }

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            return SymbolElement.Load(proces, outElements);
        }

        public override bool LoadAnalyze(LoadProces proces, List<CodeElement> errorWords)
        {
            return SymbolElement.LoadAnalyze(proces, errorWords);
        }
    }
}