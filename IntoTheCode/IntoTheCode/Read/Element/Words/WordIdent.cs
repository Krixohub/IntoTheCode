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

        public override ParserElementBase CloneWithProces(LoadProces proces)
        {
            return new WordIdent(Name) { Proces = proces };
        }

        public override string GetSyntax() { return MetaParser.WordIdent__; }
        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        private const string AllowedCharsFirst = "abcdefghijklmnopqrstuvwxyz";
        private const string AllowedCharsNext = "abcdefghijklmnopqrstuvwxyz0123456789";

        public override bool Load(List<TreeNode> outElements)
        {
            SkipWhiteSpace();

            if (Proces.TextBuffer.IsEnd(1)) return false;

            TextSubString subStr = Proces.TextBuffer.NewSubStringFrom();

            if (!AllowedCharsFirst.Contains(Proces.TextBuffer.GetChar().ToString().ToLower()))
                return false;// SetPointerBack(proces, proces.TextBuffer.PointerNextChar, this);
            else
                Proces.TextBuffer.IncPointer();

            while (!Proces.TextBuffer.IsEnd() && AllowedCharsNext.Contains(Proces.TextBuffer.GetChar().ToString().ToLower()))
            { Proces.TextBuffer.IncPointer(); }
            
            subStr.SetTo(Proces.TextBuffer.PointerNextChar);

            var element = new CodeElement(Proces.TextBuffer, this, subStr);
            outElements.Add(element);

            return true;
        }

        public override bool ExtractError()
        {
            SkipWhiteSpace();

            //TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = Proces.TextBuffer.PointerNextChar.Clone();

            if (Proces.TextBuffer.IsEnd(1))
            {
                //subStr.SetTo(subStr.GetFrom());
                //proces.Errors.Add(new ParserError(this, proces.TextBuffer.PointerEnd, 2, "Expecting identifier, found EOF."));
                Proces.AddSyntaxError(this, Proces.TextBuffer.PointerEnd, 2, "Expecting identifier, found EOF.");
                return false; // SetPointerBack(proces, from, this);
            }

            // todo brug getchar()
            if (!AllowedCharsFirst.Contains(Proces.TextBuffer.GetChar().ToString().ToLower()))
            {
                //subStr.SetTo(proces.TextBuffer.PointerNextChar);
                //var element = new CodeElement(proces.TextBuffer, this, subStr, "First charactor is not allowed.");
                //errorWords.Add(element);
                //proces.Errors.Add(new ParserError(this, from, 2, "First charactor is not allowed."));
                Proces.AddSyntaxError(this, from, 2, "First charactor is not allowed.");
                return false; // SetPointerBack(proces, from, this);
            }
            else
                Proces.TextBuffer.IncPointer();


            // todo brug getchar()
            while (!Proces.TextBuffer.IsEnd() && AllowedCharsNext.Contains(Proces.TextBuffer.GetSubString(Proces.TextBuffer.PointerNextChar, 1).ToLower()))
            { Proces.TextBuffer.IncPointer(); }

            return true;
        }
    }
}