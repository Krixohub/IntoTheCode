using System.Collections.Generic;

using IntoTheCode.Basic;
using IntoTheCode.Buffer;
using IntoTheCode.Basic.Util;
using IntoTheCode.Read.Element.Words;

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
            string syntax = Name.PadRight(Parser.SymbolFixWidth) + " = ";
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

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            TextSubString subStr = proces.TextBuffer.NewSubStringFrom();

            if (Collapse)
            {
                if (!LoadSet(proces, outElements))
                    return false;
            }
            else
            {
                List<TreeNode> outSubNotes = new List<TreeNode>();
                CodeElement element;
                if (ElementContent == ElementContentType.OneValue)
                {
                    if (!(SubElements[0] as ParserElementBase).Load(proces, outSubNotes))
                        return false; // SetPointerBack(proces, from, SubElements[0] as ParserElementBase);

                    if (outSubNotes.Count == 1)
                    {
                        element = new CodeElement(proces.TextBuffer, this, ((CodeElement)outSubNotes[0]).ValuePointer);
                        element.ValueReader = ((CodeElement)outSubNotes[0]).ValueReader;
                    }
                    else // count = 0
                    {
                        subStr.SetTo(proces.TextBuffer.PointerNextChar);
                        element = new CodeElement(proces.TextBuffer, this, subStr);
                    }

                    outElements.Add(element);
                }
                else
                {
                    if (!LoadSet(proces, outSubNotes))
                        return false;

                    element = new CodeElement(proces.TextBuffer, this, subStr);
                    element.Add(outSubNotes);
                    outElements.Add(element);
                }
            }

            // If this is a 'division' set unambiguous
            if (Trust && proces.TextBuffer.PointerNextChar.CompareTo(subStr.GetFrom()) > 0) proces.ThisIsUnambiguous(this, (CodeElement)outElements[outElements.Count - 1]);
            return true;

        }

        public override bool ExtractError(LoadProces proces)
        {
            //TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar.Clone();
            //List<TreeNode> outSubNotes = new List<TreeNode>();
            //CodeElement element;

            if (Collapse)
                return ExtractErrorSet(proces);

            if (ElementContent == ElementContentType.OneValue)
            {
                if (!(SubElements[0] as ParserElementBase).ExtractError(proces))
                    return SetPointerBackError(proces, from);
            }

            else
            {
                if (!ExtractErrorSet(proces))
                    return SetPointerBackError(proces, from);

            }
            
            return true;
        }
    }
}