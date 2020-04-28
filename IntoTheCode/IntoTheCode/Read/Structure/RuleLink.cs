using System.Collections.Generic;
using IntoTheCode.Basic;
using IntoTheCode.Buffer;

namespace IntoTheCode.Read.Element
{
    /// <summary>
    /// Base class for Grammar symbols.
    /// </summary>
    /// <remarks>Inherids <see cref="ParserElementBase"/></remarks>
    public class RuleLink : ParserElementBase
    {
        /// <summary>Creator for <see cref="RuleLink"/>.</summary>
        internal RuleLink(string value)
        {
            Name = "name";
            _value = value;
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new RuleLink(_value) { Name = "name", TextBuffer = buffer };
        }

        /// <summary>The Reader has the current pointer of reading, and the context.</summary>
        internal Rule RuleElement;

        public override string GetGrammar()
        {
            return GetValue();
        }

        public bool Recursive;

        public override bool Load(List<TextElement> outElements, int level)
        {
            // End too many recursive calls
            if (Recursive)
            {
                LoopLevel loop = TextBuffer.GetLoopLevel(this);
                if (loop.LastInvokePos == TextBuffer.PointerNextChar &&
                    level > loop.LastInvokeLevel) return false;
                loop.LastInvokePos = TextBuffer.PointerNextChar;
                loop.LastInvokeLevel = level;
            }

            return RuleElement.Load(outElements, level + 1);
        }

        public override bool ResolveErrorsForward()
        {
            return RuleElement.ResolveErrorsForward();
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(TextElement last)
        {
            return RuleElement.ResolveErrorsLast(last);
        }

        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            bool ok = false;
            path.Add(this);
            int ruleNo = path.IndexOf(RuleElement);
            if (ruleNo > -1)
            {
                // The link is recursive
                for (int i = ruleNo; i < path.Count; i++)
                {
                    if (path[i] is RuleLink) ((RuleLink)path[i]).Recursive = true;
                    if (path[i] is Rule) ok = ok || ((Rule)path[i]).LoopHasEnd;
                }
            }
            else
                ok = RuleElement.InitializeLoop(rules, path, status);

            path.Remove(this);
            return ok;
        }
    }
}