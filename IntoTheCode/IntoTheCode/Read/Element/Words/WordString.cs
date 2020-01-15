using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element;
using System;
using IntoTheCode.Message;

namespace IntoTheCode.Read.Element.Words
{
    internal class WordString : WordBase
    {
        internal WordString()
        {
            Name = "string";
            //ElementContent = ElementContentType.OneValue;
            //Color = 2;

            // Only for unittest, when syntax is not linked.
            //Reader = reader;
        }

        public override ParserElementBase CloneForParse(ITextBuffer buffer)
        {
            return new WordString() { Name = Name, TextBuffer = buffer };
        }

        public override string GetSyntax() { return MetaParser.WordString_; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TreeNode> outElements)
        {
            SkipWhiteSpace();

            TextPointer from = TextBuffer.PointerNextChar.Clone();

            if (TextBuffer.IsEnd(2)) return false;

            if (TextBuffer.GetChar() != '\'')
                return false; // SetPointerBack(Proces, from, this);

            TextBuffer.IncPointer();
            TextSubString subStr = TextBuffer.NewSubStringFrom();
            TextBuffer.IncPointer();
            TextBuffer.SetToIndexOf(subStr, "'");

            if (!subStr.ToIsValid())
                return SetPointerBack(from, this);

            outElements.Add(new CodeElement(this, subStr));
            TextBuffer.SetPointerTo(subStr);
            TextBuffer.IncPointer();

            return true;
        }

        public override bool ExtractError(ref int wordCount)
        {
            SkipWhiteSpace();
            TextPointer from = TextBuffer.PointerNextChar.Clone();
            int fromWordCount = wordCount;

            if (TextBuffer.IsEnd(2))
            {
                //subStr1.SetTo(subStr1.GetFrom());
                //var element = new CodeElement(Proces.TextBuffer, this, subStr1, "Expecting string, found EOF.");
                //errorWords.Add(element);
                //Proces.Errors.Add(new ParserError(this, TextBuffer.PointerEnd, 2, "Expecting string, found EOF."));
                //TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerEnd, 2, "Expecting string, found EOF.");
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerEnd, 2, () => MessageRes.pe03);
            };


            if (TextBuffer.GetChar() != '\'')
            {
                //subStr1.SetTo(subStr1.GetFrom());
                //errorWords.Add(new CodeElement(Proces.TextBuffer, this, subStr1, "Expecting string."));
                //Proces.Errors.Add(new ParserError(this, from, 2, "Expecting string."));
               // TextBuffer.Status.AddSyntaxError(this, from, 2, "Expecting string.");
                return TextBuffer.Status.AddSyntaxError(this, from, 2, () => MessageRes.pe04);
            }

            TextBuffer.IncPointer();
            TextSubString subStr = TextBuffer.NewSubStringFrom();
            TextBuffer.IncPointer();
            TextBuffer.SetToIndexOf(subStr, "'");

            if (!subStr.ToIsValid())
            {
                //TextBuffer.Status.AddSyntaxError(this, from.Clone(1), 2, "Expecting string ending.");
                TextBuffer.Status.AddSyntaxError(this, from.Clone(1), 2, () => MessageRes.pe05);
                return SetPointerBackError(from, ref wordCount, fromWordCount);
            }

            TextBuffer.SetPointerTo(subStr);
            TextBuffer.IncPointer();

            wordCount++;
            return true;
        }

        //public override List<AmbiguousDef> IsAmbiguousChar(ParserElementBase element, bool ws, int pos, string allowed, string disallowed)
        //{
        //    var list = new List<AmbiguousDef>();
        //    string st =    /*
        //  */ "hej";
        //    if (st == "hej")
        //        return null;
        //    return list;
        //}

    }
}