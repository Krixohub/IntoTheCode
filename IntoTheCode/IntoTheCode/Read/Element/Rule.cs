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

        public override ParserElementBase CloneWithProces(LoadProces proces)
        {
            var element = new Rule(Name, SubElements.Select(r => ((ParserElementBase)r).CloneWithProces(proces)).ToArray());// { Parser = Parser };
            element.Proces = proces;
            element.Collapse = Collapse;
            element.Trust = Trust;
            //element._elementContent = ElementContent;
            return element;
        }

        //public override void Initialize()
        //{


        //}

        public override ElementContentType GetElementContent()
        {
            return SubElements.Count == 1 ?
                (SubElements[0] as ParserElementBase).ElementContent :
                ElementContentType.Many;
        }


        internal bool Collapse { get; set; }
        internal bool Trust { get; set; }

        public override string GetSyntax()
        {
            string syntax = Name.PadRight(Parser != null ? Parser.SymbolFixWidth : 4) + " = ";
            //string syntax = Identifier.Name.PadRight(Syntax.SymbolFixWidth) + (Tag ? " => " : " =  ");
            syntax += base.GetSyntax();
            syntax += ";";
            return syntax;
        }

        public override string GetSettings()
        {
            //string setting = Tag ? string.Empty : Name.PadRight(Parser.SymbolFixWidth) + " tag = 'false';";
            string setting = Collapse ? string.Empty : Name.PadRight(Parser.SymbolFixWidth) + " collapse = 'true';";
            return setting;
        }

        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TreeNode> outElements)
        {
            TextSubString subStr = Proces.TextBuffer.NewSubStringFrom();

            if (Collapse)
            {
                if (!LoadSet(outElements))
                    return false;
            }
            else
            {
                List<TreeNode> outSubNotes = new List<TreeNode>();
                CodeElement element;
                if (ElementContent == ElementContentType.OneValue)
                {
                    if (!(SubElements[0] as ParserElementBase).Load(outSubNotes))
                        return false; // SetPointerBack(proces, from, SubElements[0] as ParserElementBase);

                    if (outSubNotes.Count == 1)
                    {
                        element = new CodeElement(Proces.TextBuffer, this, ((CodeElement)outSubNotes[0]).ValuePointer);
                        element.ValueReader = ((CodeElement)outSubNotes[0]).ValueReader;
                    }
                    else // count = 0
                    {
                        subStr.SetTo(Proces.TextBuffer.PointerNextChar);
                        element = new CodeElement(Proces.TextBuffer, this, subStr);
                    }

                    outElements.Add(element);
                }
                else
                {
                    if (!LoadSet(outSubNotes))
                        return false;

                    element = new CodeElement(Proces.TextBuffer, this, subStr);
                    element.Add(outSubNotes);
                    outElements.Add(element);
                }
            }

            // If this is a 'division' set unambiguous
            if (Trust && Proces.TextBuffer.PointerNextChar.CompareTo(subStr.GetFrom()) > 0) Proces.ThisIsUnambiguous(this, (CodeElement)outElements[outElements.Count - 1]);
            return true;

        }

        public override bool ExtractError()
        {
            //TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = Proces.TextBuffer.PointerNextChar.Clone();
            //List<TreeNode> outSubNotes = new List<TreeNode>();
            //CodeElement element;

            if (Collapse)
                return ExtractErrorSet();

            if (ElementContent == ElementContentType.OneValue)
            {
                if (!(SubElements[0] as ParserElementBase).ExtractError())
                    return SetPointerBackError(from);
            }

            else
            {
                if (!ExtractErrorSet())
                    return SetPointerBackError(from);

            }
            
            return true;
        }
    }
}