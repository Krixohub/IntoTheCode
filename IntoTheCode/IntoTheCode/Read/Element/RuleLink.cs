using System.Collections.Generic;
using IntoTheCode.Basic;
using IntoTheCode.Buffer;

namespace IntoTheCode.Read.Element
{
    /// <summary>
    /// Base class for Grammar symbols.
    /// </summary>
    internal class RuleLink : ParserElementBase
    {
        /// <summary>Creator for <see cref="RuleLink"/>.</summary>
        internal RuleLink(string value)
        {
            Name = "name";
            _value = value;
            LastInvokePos = TextBuffer.NotValidPtr;
            //if (value == "expression")
                Recursive = true;
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new RuleLink(_value) { Name = "name", TextBuffer = buffer };
        }

        /// <summary>The Reader has the current pointer of reading, and the context.</summary>
        internal Rule RuleElement;

        public override ElementContentType GetElementContent()
        {
            return RuleElement != null ?
                RuleElement.ElementContent :
                ElementContentType.NotSet;
        }

        public override string GetGrammar()
        {
            return GetValue();
        }

        public bool Recursive;
        public int LastInvokePos;
        public int LastInvokeLevel;

        public override bool Load(List<CodeElement> outElements, int level)
        {
            // End too many recursive calls
            if (Recursive)
            {
                if (LastInvokePos == TextBuffer.PointerNextChar &&
                    level > LastInvokeLevel) return false;
                LastInvokePos = TextBuffer.PointerNextChar;
                LastInvokeLevel = level;
            }

            return RuleElement.Load(outElements, level + 1);
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int LoadFindLast(CodeElement last)
        {
            return RuleElement.LoadFindLast(last);
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            return RuleElement.LoadTrackError(ref wordCount);
        }
    }
}