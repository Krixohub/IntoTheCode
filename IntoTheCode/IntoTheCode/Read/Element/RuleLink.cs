using System.Collections.Generic;
using IntoTheCode.Basic;
using IntoTheCode.Buffer;

namespace IntoTheCode.Read.Element
{
    /// <summary>
    /// Base class for syntax symbols.
    /// </summary>
    internal class RuleLink : ParserElementBase
    {
        /// <summary>Creator for <see cref="RuleLink"/>.</summary>
        internal RuleLink(string value)
        {
            Name = "name";
            _value = value;

            if (value == "expression")
                Recursive = true;
        }

        public override ParserElementBase CloneForParse(ITextBuffer buffer)
        {
            return new RuleLink(_value) { Name = "name", TextBuffer = buffer };
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

        public bool Recursive;
        public TextPointer LastRuleInvoke;

        public override bool Load(List<TreeNode> outElements)
        {
            // End too many recursive calls
            TextPointer from = TextBuffer.PointerNextChar.Clone();
            if (Recursive)
            {
                if (LastRuleInvoke != null && LastRuleInvoke.CompareTo(from) == 0) return false;
                LastRuleInvoke = from;
            }

            return SymbolElement.Load(outElements);
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int LoadFindLast(CodeElement last)
        {
            return SymbolElement.LoadFindLast(last);
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            return SymbolElement.LoadTrackError(ref wordCount);
        }
    }
}