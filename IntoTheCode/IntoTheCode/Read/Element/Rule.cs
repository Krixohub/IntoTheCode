using System.Collections.Generic;

using IntoTheCode.Basic;
using IntoTheCode.Buffer;
using IntoTheCode.Basic.Util;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Read.Element
{
    internal class Rule : SequenceBase
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

            // Set 'div' property if this is the last of many elements
        //    IList<TreeNode> siblings = Parent.SubElements;
            if ((elements.Length > 2 && elements[elements.Length - 1] is WordSymbol) ||
                AnyNested(elem => elem is WordSymbol && ((WordSymbol)elem).Value.Length > 2))
                Div = true;


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
        internal bool Div { get; set; }

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
//            TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar.Clone();
            if (!Load2(proces, outElements)) return false;

            //subStr.SetTo(proces.TextBuffer.PointerNextChar);

            // If this is a 'division' set unambiguous
            if (Div && proces.TextBuffer.PointerNextChar.CompareTo(from) > 0) proces.ThisIsUnambiguous(this, (CodeElement)outElements[outElements.Count - 1]);
            return true;

        }

        //
        private bool Load2(LoadProces proces, List<TreeNode> outElements)
        {
            TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar.Clone();
            List<TreeNode> outSubNotes = new List<TreeNode>();
            CodeElement element;

            if (Collapse)
                return LoadSet(proces, outElements);

            string debug1 = "(" + Parent?.Name + ") Rule:" + GetSyntax();

            if (ElementContent == ElementContentType.OneValue)
            {
                if (!(SubElements[0] as ParserElementBase).Load(proces, outSubNotes))
                    return SetPointerBack(proces, from, SubElements[0] as ParserElementBase);

                if (outSubNotes.Count == 1)
                {
                    element = new CodeElement(proces.TextBuffer, this, ((CodeElement)outSubNotes[0]).ValuePointer);
                    element.ValueReader = ((CodeElement)outSubNotes[0]).ValueReader;
                }
                else
                {
                    subStr.SetTo(proces.TextBuffer.PointerNextChar);
                    element = new CodeElement(proces.TextBuffer, this, subStr);
                }

            }

            else
            {
                if (!LoadSet(proces, outSubNotes))
                    return SetPointerBack(proces, from, this);

                subStr.SetTo(proces.TextBuffer.PointerNextChar);
                if (subStr.Length() == 0)
                    return true;  // returns true if this rule is optional and not read.

                element = new CodeElement(proces.TextBuffer, this, subStr);
                element.Add(outSubNotes);
            }

            outElements.Add(element);

            string debug2 = "Out:" + element.ToMarkupProtected(string.Empty);

            return true;
        }

        public override bool ExtractError(LoadProces proces)
        {
            //TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar.Clone();
            //List<TreeNode> outSubNotes = new List<TreeNode>();
            //CodeElement element;

            if (Collapse)
                return LoadSetAnalyze(proces);

            if (ElementContent == ElementContentType.OneValue)
            {
                if (!(SubElements[0] as ParserElementBase).ExtractError(proces))
                    return SetPointerBack(proces, from, SubElements[0] as ParserElementBase);
            }

            else
            {
                if (!LoadSetAnalyze(proces))
                    return SetPointerBack(proces, from, this);

            }
            
            return true;
        }
    }
}