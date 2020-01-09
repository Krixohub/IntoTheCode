using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System;

namespace IntoTheCode.Read.Element.Words
{
    internal class WordIdent : WordBase
    {
        internal WordIdent(string name)
        {
            Name = name;
            //ElementContent = ElementContentType.OneValue;
            //Color = 1; //  Brushes.Blue;

            // Only for unittest, when syntax is not linked.
            //Reader = parser;
        }

        public override ParserElementBase CloneForParse(ITextBuffer buffer)
        {
            return new WordIdent(Name) { TextBuffer = buffer };
        }

        public override string GetSyntax() { return MetaParser.WordIdent__; }
        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        private const string AllowedCharsFirst = "abcdefghijklmnopqrstuvwxyz";
        private const string AllowedCharsNext = "abcdefghijklmnopqrstuvwxyz0123456789";

        public override bool Load(List<TreeNode> outElements)
        {
            SkipWhiteSpace();

            if (TextBuffer.IsEnd(1)) return false;

            TextSubString subStr = TextBuffer.NewSubStringFrom();

            if (!AllowedCharsFirst.Contains(TextBuffer.GetChar().ToString().ToLower()))
                return false;// SetPointerBack(proces, TextBuffer.PointerNextChar, this);
            else
                TextBuffer.IncPointer();

            while (!TextBuffer.IsEnd() && AllowedCharsNext.Contains(TextBuffer.GetChar().ToString().ToLower()))
            { TextBuffer.IncPointer(); }
            
            subStr.SetTo(TextBuffer.PointerNextChar);

            var element = new CodeElement(this, subStr);
            outElements.Add(element);

            return true;
        }

        public override bool ExtractError()
        {
            SkipWhiteSpace();

            //TextSubString subStr = TextBuffer.NewSubStringFrom();
            TextPointer from = TextBuffer.PointerNextChar.Clone();

            if (TextBuffer.IsEnd(1))
            {
                //subStr.SetTo(subStr.GetFrom());
                //proces.Errors.Add(new ParserError(this, TextBuffer.PointerEnd, 2, "Expecting identifier, found EOF."));
                TextBuffer.Proces.AddSyntaxError(this, TextBuffer.PointerEnd, 2, "Expecting identifier, found EOF.");
                return false; // SetPointerBack(proces, from, this);
            }

            // todo brug getchar()
            if (!AllowedCharsFirst.Contains(TextBuffer.GetChar().ToString().ToLower()))
            {
                //subStr.SetTo(TextBuffer.PointerNextChar);
                //var element = new CodeElement(proces.TextBuffer, this, subStr, "First charactor is not allowed.");
                //errorWords.Add(element);
                //proces.Errors.Add(new ParserError(this, from, 2, "First charactor is not allowed."));
                TextBuffer.Proces.AddSyntaxError(this, from, 2, "First charactor is not allowed.");
                return false; // SetPointerBack(proces, from, this);
            }
            else
                TextBuffer.IncPointer();


            // todo brug getchar()
            while (!TextBuffer.IsEnd() && AllowedCharsNext.Contains(TextBuffer.GetSubString(TextBuffer.PointerNextChar, 1).ToLower()))
            { TextBuffer.IncPointer(); }

            return true;
        }
    }
}