using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element;

namespace IntoTheCode.Read.Word
{
    internal class WordString : Word
    {
        internal WordString()
        {
            Name = "string";
            //ElementContent = ElementContentType.OneValue;
            //Color = 2;

            // Only for unittest, when syntax is not linked.
            //Reader = reader;
        }

        public override string GetSyntax() { return MetaParser.WordString_; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            //if (Reader == null)
            //{
            //    SkipWhiteSpace(proces);
            //}

            //TextSubString subStr1 = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar;
            SkipWhiteSpace(proces);
            if (proces.TextBuffer.IsEnd(2)) return false;


            if (proces.TextBuffer.GetChar() != '\'')
                return SetPointerBack(proces, from);

            proces.TextBuffer.IncPointer();
            TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            proces.TextBuffer.IncPointer();
            proces.TextBuffer.SetToIndexOf(subStr, "'");

            if (subStr.ToIsValid())
                return SetPointerBack(proces, from);

            outElements.Add(new CodeElement(proces.TextBuffer, this, subStr));
            proces.TextBuffer.SetPointerTo(subStr);
            proces.TextBuffer.IncPointer();

            return true;
        }

        public override List<AmbiguousDef> IsAmbiguousChar(ParserElementBase element, bool ws, int pos, string allowed, string disallowed)
        {
            var list = new List<AmbiguousDef>();
            string st =    /*
          */ "hej";
            if (st == "hej")
                return null;
            return list;
        }


    }
}