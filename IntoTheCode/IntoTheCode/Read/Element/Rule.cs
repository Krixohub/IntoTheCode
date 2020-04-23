using System.Collections.Generic;

using IntoTheCode.Basic;
using IntoTheCode.Buffer;
using IntoTheCode.Basic.Util;
using IntoTheCode.Read.Element.Words;
using System.Linq;
using IntoTheCode.Message;
using System;

namespace IntoTheCode.Read.Element
{
    /// <remarks>Inherids <see cref="SetOfElementsBase"/></remarks>
    public class Rule : SetOfElementsBase
    {
        internal Parser Parser { get; set; }

        /// <summary>
        /// Statement, Action, 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="elements"></param>
        internal Rule(string name, params ParserElementBase[] elements) : base(elements)
        {
            Name = name;

            // Set 'trust' property if this is the last of many elements
            Trust = GetTrustAuto();

            _simplify = elements.Length == 1 && elements[0] is WordBase;
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            var element = new Rule(Name, SubElements.Select(r => ((ParserElementBase)r).CloneForParse(buffer)).ToArray())
            {
                DefinitionCodeElement = DefinitionCodeElement,
                TextBuffer = buffer,
                Collapse = Collapse,
                Trust = Trust
            };

            return element;
        }

        internal bool Collapse { get; set; }
        internal bool Trust { get; set; }

        private bool GetTrustAuto()
        {
            // Set 'trust' property if this is the last of many elements
            return (SubElements.Count > 2 && SubElements[SubElements.Count - 1] is WordSymbol) ||
                AnyNested(elem => elem is WordSymbol && ((WordSymbol)elem).Value.Length > 2);

        }

        public override string GetGrammar()
        {
            string Grammar = Name.PadRight(Parser != null ? Parser.SymbolFixWidth : 4) + " = ";
            //string Grammar = Identifier.Name.PadRight(Grammar.SymbolFixWidth) + (Tag ? " => " : " =  ");
            Grammar += base.GetGrammar();
            Grammar += ";";
            return Grammar;
        }

        public override void GetSettings(List<Tuple<string, string>> settings)
        {
            ////string setting = Tag ? string.Empty : Name.PadRight(Parser.SymbolFixWidth) + " tag = 'false';";
            //string setting = Collapse ? string.Empty : Name.PadRight(Parser.SymbolFixWidth) + " collapse = 'true';";
            if (Trust != GetTrustAuto()) 
                settings.Add(new Tuple<string, string>(Name, nameof(Trust).ToLower() + (Trust ? string.Empty : " = 'false'")));
            if (Collapse) 
                settings.Add(new Tuple<string, string>(Name, nameof(Collapse).ToLower()));

            foreach (ParserElementBase item in SubElements)
                item.GetSettings(settings);
        }

        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        private bool _simplify;

        public override bool Load(List<TextElement> outElements, int level)
        {

            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar);

            if (Collapse)
            {
                if (!LoadSet(outElements, level))
                    return false;
            }
            else
            {

                var outSubNotes = new List<TextElement>();
                TextElement element;
                if (!LoadSet(outSubNotes, level))
                    return false;

                subStr.To = TextBuffer.PointerNextChar;

                // _simplify is true when a rule just contains a single word
                // The word value is inserted directly (collapsed)
                if (_simplify)
                {
                    element = outSubNotes.FirstOrDefault(n => n.GetType() != typeof(CommentElement));
                    if (element != null)
                        element.Name = Name;
                    else
                        element = new CodeElement(this, subStr);

                    outElements.Add(element);
                    // Add comments
                    outElements.AddRange(outSubNotes.OfType<CommentElement>());
                }
                else
                {
                    element = new CodeElement(this, subStr);
                    element.Add(outSubNotes);
                    outElements.Add(element);
                }
            }

            // If this is a 'division' set unambiguous and insert comments
            if (Trust && TextBuffer.PointerNextChar > subStr.From && outElements.Count > 0)
            {
                TextBuffer.Status.ThisIsUnambiguous(this, (CodeElement)outElements[outElements.Count - 1]);

                //// insert comments
                //foreach (CodeElement elem in TextBuffer.Comments) elem.Name = "Comment";
                //outElements.AddRange(TextBuffer.Comments);
                //TextBuffer.Comments.Clear();
            }
            return true;

        }

        /// <summary>Find the Rule/ 'read element', that correspond to the
        /// last CodeElement, and read it again with error resolving. 
        /// If no error, try to read further.</summary>
        /// <param name="last">Not null, not empty.</param>
        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(TextElement last)
        {
            string debug = GetGrammar().NL() + last.ToMarkupProtected(string.Empty);

            if (Collapse)
                 return ResolveSetErrorsLast(last);

            int rc = 0;
            if (last.Name != Name) return 0;

            
            if (_simplify)
                rc = ((ParserElementBase)SubElements[0]).ResolveErrorsLast(last);
            else if (last.SubElements != null && last.SubElements.Count() > 0)
                // if succes finding a deeper element, return true.
                rc = ResolveSetErrorsLast(last.SubElements.Last() as CodeElement);
            else if (!ResolveErrorsForward())
                return 1;

            return 2;
        }

        public override bool ResolveErrorsForward()
        {
            int from = TextBuffer.PointerNextChar;

            if (Collapse)
                return ResolveSetErrorsForward();

            if (_simplify &&
                !(SubElements[0] as ParserElementBase).ResolveErrorsForward())
                return SetPointerBack(from);

            else if (!_simplify &&
                !ResolveSetErrorsForward())
                return SetPointerBack(from);

            return true;
        }

        public bool LoopHasEnd;

        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            if (!rules.Contains(this)) rules.Add(this);
            path.Add(this);

            LoopHasEnd = LoopHasEnd | base.InitializeLoop(rules, path, status);

            path.Remove(this);

            return LoopHasEnd;
        }
    }
}