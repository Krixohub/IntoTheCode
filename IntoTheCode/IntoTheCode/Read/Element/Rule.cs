using System.Collections.Generic;

using IntoTheCode.Basic;
using IntoTheCode.Buffer;
using IntoTheCode.Basic.Util;

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
        }

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
            TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar;
            List<TreeNode> outSubNotes = new List<TreeNode>();
            CodeElement element;

            if (Collapse)
                return LoadSet(proces, outElements);

            string debug1 = "(" + Parent?.Name + ") Rule:" + GetSyntax();

            if (ElementContent == ElementContentType.OneValue)
            {
                if (!(SubElements[0] as ParserElementBase).Load(proces, outSubNotes))
                    return SetPointerBack(proces, from);

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
                    return SetPointerBack(proces, from);

                subStr.SetTo(proces.TextBuffer.PointerNextChar);
                if (subStr.Length() == 0)
                    return true;

                element = new CodeElement(proces.TextBuffer, this, subStr);
                element.Add(outSubNotes);
            }

            outElements.Add(element);

            string debug2 = "Out:" + element.ToMarkupProtected(string.Empty);

            return true;
        }
    }
}