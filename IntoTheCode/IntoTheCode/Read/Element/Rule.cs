using System.Collections.Generic;

using IntoTheCode.Basic;
using IntoTheCode.Buffer;
using IntoTheCode.Basic.Util;
using IntoTheCode.Read.Element.Words;
using System.Linq;

namespace IntoTheCode.Read.Element
{
    internal class Rule : SetOfElementsBase
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
                if (ElementContent == ElementContentType.OneValue)
                {
                    if (!(SubElements[0] as ParserElementBase).Load(outSubNotes, level))
                        return false;

                    if (outSubNotes.Count == 1)
                    {
                        element = new CodeElement(this, ((CodeElement)outSubNotes[0]).SubString);
                        element.WordParser = ((CodeElement)outSubNotes[0]).WordParser;
                    }
                    else
                    {
                        subStr.To = TextBuffer.PointerNextChar;
                        element = new CodeElement(this, subStr);
                    }

                    outElements.Add(element);
                }
                else
                {
                    if (!LoadSet(outSubNotes, level))
                        return false;

                    element = new CodeElement(this, subStr);
                    element.Add(outSubNotes);
                    outElements.Add(element);
                }
            }

            // If this is a 'division' set unambiguous
            if (Trust && TextBuffer.PointerNextChar.CompareTo(subStr.From) > 0 && outElements.Count > 0)
                TextBuffer.Status.ThisIsUnambiguous(this, (CodeElement)outElements[outElements.Count - 1]);
            return true;

        }

        /// <summary>Find the Rule/ 'read element', that correspond to the
        /// last CodeElement, and read it again with error tracking. 
        /// If no error, try to read further.</summary>
        /// <param name="last">Not null, not empty.</param>
        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int LoadFindLast(CodeElement last)
        {
            string debug = GetGrammar().NL() + last.ToMarkupProtected(string.Empty);

            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar);

            if (Collapse)
                 return TryLastSetAgain(last);

            int rc = 0;
            if (last.Name != Name) return 0;

            if (ElementContent == ElementContentType.OneValue)
                rc = (SubElements[0] as ParserElementBase).LoadFindLast(last);
            else if (last.SubElements != null && last.SubElements.Count() > 0)
                // if succes finding a deeper element, return true.
                rc = TryLastSetAgain(last.SubElements.Last() as CodeElement);

            return rc;
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            int from = TextBuffer.PointerNextChar;
            int fromWordCount = wordCount;

            if (Collapse)
                return LoadSetTrackError(ref wordCount);

            if (ElementContent == ElementContentType.OneValue && 
                !(SubElements[0] as ParserElementBase).LoadTrackError(ref wordCount))
                    return SetPointerBackError(from, ref wordCount, fromWordCount);

            if (ElementContent != ElementContentType.OneValue && 
                !LoadSetTrackError(ref wordCount))
                return SetPointerBackError(from, ref wordCount, fromWordCount);

            return true;
        }
    }
}