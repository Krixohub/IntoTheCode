using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element;
using System;

namespace IntoTheCode.Read.Element.Words
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
                return SetPointerBack(proces, from, this);

            proces.TextBuffer.IncPointer();
            TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            proces.TextBuffer.IncPointer();
            proces.TextBuffer.SetToIndexOf(subStr, "'");

            if (subStr.ToIsValid())
                return SetPointerBack(proces, from, this);

            outElements.Add(new CodeElement(proces.TextBuffer, this, subStr));
            proces.TextBuffer.SetPointerTo(subStr);
            proces.TextBuffer.IncPointer();

            return true;
        }

        public override bool LoadAnalyze(LoadProces proces, List<CodeElement> errorWords)
        {
            //TextSubString subStr1 = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar;
            SkipWhiteSpace(proces);
            TextSubString subStr1 = proces.TextBuffer.NewSubStringFrom();

            if (proces.TextBuffer.IsEnd(2))
            {
                subStr1.SetTo(subStr1.GetFrom());
                var element = new CodeElement(proces.TextBuffer, this, subStr1, "Expecting quote, found EOF");
                errorWords.Add(element);
                return false;
            };


            if (proces.TextBuffer.GetChar() != '\'')
            {
                subStr1.SetTo(subStr1.GetFrom());
                errorWords.Add(new CodeElement(proces.TextBuffer, this, subStr1, "Expecting starting \"'\""));
                return SetPointerBack(proces, from, this);
            }

            proces.TextBuffer.IncPointer();
            TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            proces.TextBuffer.IncPointer();
            proces.TextBuffer.SetToIndexOf(subStr, "'");

            if (subStr.ToIsValid())
            {
                subStr.SetTo(subStr.GetFrom());
                errorWords.Add(new CodeElement(proces.TextBuffer, this, subStr, "Expecting ending \"'\""));
                return SetPointerBack(proces, from, this);
            }

            //outElements.Add(new CodeElement(proces.TextBuffer, this, subStr));
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