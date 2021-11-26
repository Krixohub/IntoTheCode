using System.Collections.Generic;
using System.Linq;
using IntoTheCode.Buffer;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="SetOfElementsBase"/></remarks>
    internal class Optional : SetOfElementsBase
    {
        /// <summary>Creator for <see cref="Optional"/>.</summary>
        internal Optional(params ParserElementBase[] elements)
            : base(elements)
        {
            //Attributter = new ObservableCollection<Attribute>();
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            var element = new Optional(ChildNodes.Select(r => ((ParserElementBase)r).CloneForParse(buffer)).ToArray());// { Parser = Parser };
            element.TextBuffer = buffer;
            return element;
        }

        public override string GetGrammar() { return "[" + base.GetGrammar() + "]"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TextElement> outElements, int level)
        {
            var elements = new List<CodeElement>();
            if (LoadSet(outElements, level))
                outElements.AddRange(elements);
            return TextBuffer.Status.Error == null;
        }

        /// <summary>Find the Rule/ 'read element', that correspond to the
        /// last CodeElement, and read it again with error resolving. 
        /// If no error, try to read further.</summary>
        /// <param name="last">Not null, not empty.</param>
        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last, int level)
        {
            return ResolveSetErrorsLast(last, level);
        }

        public override bool ResolveErrorsForward(int level)
        {
            ResolveSetErrorsForward(level);
            return true;
        }

        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            base.InitializeLoop(rules, path, status);
            return true;
        }

        public override bool InitializeLoopHasWord(RuleLink link, List<RuleLink> subPath, ref bool linkFound)
        {
            bool hasWord = false;
            foreach (ParserElementBase item in this.ChildNodes)
            {
                if (item.InitializeLoopHasWord(link, subPath, ref linkFound))
                    hasWord = true;
                    
                if (linkFound)
                    return hasWord;
            }

            return false;
        }

    }
}