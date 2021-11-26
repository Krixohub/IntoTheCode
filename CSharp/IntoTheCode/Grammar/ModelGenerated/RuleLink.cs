using System.Collections.Generic;
using IntoTheCode.Basic;
using IntoTheCode.Buffer;

namespace IntoTheCode.Grammar
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
            return new RuleLink(_value) { Name = "name", TextBuffer = buffer, Recursive = Recursive };
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
                string name = Value;

                LoopLevel loop = TextBuffer.GetLoopLoad(this);
                if (loop.LastInvokePos == TextBuffer.PointerNextChar &&
                    level > loop.LastInvokeLevel) return false;
                loop.LastInvokePos = TextBuffer.PointerNextChar;
                loop.LastInvokeLevel = level;
            }

            return RuleElement.Load(outElements, level + 1);
        }

        public override bool ResolveErrorsForward(int level)
        {
            // End too many recursive calls
            if (Recursive)
            {
                string name = Value;

                LoopLevel loop = TextBuffer.GetLoopForward(this);
                if (loop.LastInvokePos == TextBuffer.PointerNextChar &&
                    level > loop.LastInvokeLevel) return false;
                loop.LastInvokePos = TextBuffer.PointerNextChar;
                loop.LastInvokeLevel = level;
            }

            return RuleElement.ResolveErrorsForward(level + 1);
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last, int level)
        {
            // End too many recursive calls
            if (Recursive)
            {
                string name = Value;

                LoopLevel loop = TextBuffer.GetLoopLast(this);
                if (loop.LastInvokePos == TextBuffer.PointerNextChar &&
                    level > loop.LastInvokeLevel) return 1;
                loop.LastInvokePos = TextBuffer.PointerNextChar;
                loop.LastInvokeLevel = level;
            }

            return RuleElement.ResolveErrorsLast(last, level + 1);
        }

        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            bool LoopHasEnd = false;
            bool LoopHasWord = false;
            path.Add(this);
            int ruleNo = path.IndexOf(RuleElement);
            if (ruleNo > -1)
            {
                // The link is recursive
                for (int i = ruleNo; i < path.Count; i++)
                {
                    if (path[i] is RuleLink) ((RuleLink)path[i]).Recursive = true;
                    if (path[i] is Rule)
                    {
                        var rule = path[i] as Rule;
                        LoopHasEnd = LoopHasEnd || rule.LoopHasEnd;
                        var subPath = new List<RuleLink>();
                        bool linkFound = false;
                        LoopHasWord = LoopHasWord || rule.InitializeLoopHasWord(((RuleLink)path[i + 1]), subPath, ref linkFound);
                    }
                }

                if (!LoopHasWord)
                    ((Rule)path[ruleNo]).LoopLeftRecursive = true;
            }
            else
                LoopHasEnd = RuleElement.InitializeLoop(rules, path, status);

            path.Remove(this);
            return LoopHasEnd;
        }

        public override bool InitializeLoopHasWord(RuleLink link, List<RuleLink> subPath, ref bool linkFound)
        {
            if (link != null && this == link)
            {
                linkFound = true;
                return false;
            }

            int linkNo = subPath.IndexOf(this);
            if (linkNo > -1)
                return false; // no words, just recursive
           
            subPath.Add(this);
            bool hasWord = RuleElement.InitializeLoopHasWord(null, subPath, ref linkFound);
            subPath.Remove(this);

            return hasWord;
        }
    }
}