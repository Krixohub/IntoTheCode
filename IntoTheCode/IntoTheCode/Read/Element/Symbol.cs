using System.Collections.Generic;
using IntoTheCode.Basic;

namespace IntoTheCode.Read.Element
{
    /// <summary>
    /// Base class for syntax symbols.
    /// </summary>
    internal class Symbol : ParserElementBase
    {
        /// <summary>Creator for <see cref="Symbol"/>.</summary>
        internal Symbol(string name)
        {
            Name = name;
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
            return Name;
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