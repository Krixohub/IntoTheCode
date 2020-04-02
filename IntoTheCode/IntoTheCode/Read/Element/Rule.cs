using System.Collections.Generic;

using IntoTheCode.Basic;
using IntoTheCode.Buffer;
using IntoTheCode.Basic.Util;
using IntoTheCode.Read.Element.Words;
using System.Linq;
using IntoTheCode.Message;

namespace IntoTheCode.Read.Element
{
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
            //    IList<TreeNode> siblings = Parent.SubElements;
            if ((elements.Length > 2 && elements[elements.Length - 1] is WordSymbol) ||
                AnyNested(elem => elem is WordSymbol && ((WordSymbol)elem).Value.Length > 2))
                Trust = true;
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

        public override ElementContentType GetElementContent()
        {
            return SubElements.Count == 1 ?
                (SubElements[0] as ParserElementBase).ElementContent :
                ElementContentType.Many;
        }

        //public override ElementContentType SetElementContent(ParserElementBase origin)
        //{
        //    if (_elementContent == ElementContentType.NotSet && origin != this)
        //    {
        //        if (SubElements.Count > 1) _elementContent = ElementContentType.Many;
        //        else _elementContent = (SubElements[0] as ParserElementBase).SetElementContent(null);
        //    }
        //    return _elementContent;
        //}

        internal bool Collapse { get; set; }
        internal bool Trust { get; set; }
        internal int Precendence { get; set; }

        public override string GetGrammar()
        {
            string Grammar = Name.PadRight(Parser != null ? Parser.SymbolFixWidth : 4) + " = ";
            //string Grammar = Identifier.Name.PadRight(Grammar.SymbolFixWidth) + (Tag ? " => " : " =  ");
            Grammar += base.GetGrammar();
            Grammar += ";";
            return Grammar;
        }

        public override string GetSettings()
        {
            //string setting = Tag ? string.Empty : Name.PadRight(Parser.SymbolFixWidth) + " tag = 'false';";
            string setting = Collapse ? string.Empty : Name.PadRight(Parser.SymbolFixWidth) + " collapse = 'true';";
            return setting;
        }

        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<CodeElement> outElements, int level)
        {

            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar);
            if (Collapse)
            {
                if (!LoadSet(outElements, level))
                    return false;
            }
            else
            {

                var outSubNotes = new List<CodeElement>();
                CodeElement element;
                if (!LoadSet(outSubNotes, level))
                    return false;

                if (ElementContent == ElementContentType.OneValue)
                {
                    //if (!(SubElements[0] as ParserElementBase).Load(outSubNotes, level))
                    //    return false;

                    CodeElement theOne = outSubNotes.FirstOrDefault(n => n.GetType() != typeof(CommentElement));
                    if (theOne != null)
                    {
                        //if (theOne.)
                        theOne.Name = Name;
                        element = theOne;
                    }
                    else
                    {
                        subStr.To = TextBuffer.PointerNextChar;
                        element = new CodeElement(this, subStr);
                    }

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
        public override int ResolveErrorsLast(CodeElement last)
        {
            string debug = GetGrammar().NL() + last.ToMarkupProtected(string.Empty);

            if (Collapse)
                 return ResolveSetErrorsLast(last);

            int rc = 0;
            if (last.Name != Name) return 0;

            if (ElementContent == ElementContentType.OneValue)
                rc = ((ParserElementBase)SubElements[0]).ResolveErrorsLast(last);
            else if (last.SubElements != null && last.SubElements.Count() > 0)
                // if succes finding a deeper element, return true.
                rc = ResolveSetErrorsLast(last.SubElements.Last() as CodeElement);

            return rc;
        }

        public override bool ResolveErrorsForward()
        {
            int from = TextBuffer.PointerNextChar;

            if (Collapse)
                return ResolveSetErrorsForward();

            if (ElementContent == ElementContentType.OneValue &&
                !(SubElements[0] as ParserElementBase).ResolveErrorsForward())
                return SetPointerBack(from);

            else if (ElementContent != ElementContentType.OneValue &&
                !ResolveSetErrorsForward())
                return SetPointerBack(from);

            return true;
        }

        public bool LoopHasEnd;


        public override bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status)
        {
            //bool ok = true;
            if (!rules.Contains(this)) rules.Add(this);
            path.Add(this);

            LoopHasEnd = LoopHasEnd | base.InitializeLoop(rules, path, status);

            path.Remove(this);

            return LoopHasEnd;
        }
    }
}