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

        public override string GetSyntax() { return MetaParser.WordIdent__; }
        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        private const string AllowedCharsFirst = "abcdefghijklmnopqrstuvwxyz";
        private const string AllowedCharsNext = "abcdefghijklmnopqrstuvwxyz0123456789";

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            SkipWhiteSpace(proces);

            if (proces.TextBuffer.IsEnd(1)) return false;

            TextSubString subStr = proces.TextBuffer.NewSubStringFrom();

            if (!AllowedCharsFirst.Contains(proces.TextBuffer.GetChar().ToString().ToLower()))
                return false;// SetPointerBack(proces, proces.TextBuffer.PointerNextChar, this);
            else
                proces.TextBuffer.IncPointer();

            while (!proces.TextBuffer.IsEnd() && AllowedCharsNext.Contains(proces.TextBuffer.GetChar().ToString().ToLower()))
            { proces.TextBuffer.IncPointer(); }
            
            subStr.SetTo(proces.TextBuffer.PointerNextChar);

            var element = new CodeElement(proces.TextBuffer, this, subStr);
            outElements.Add(element);

            return true;
        }

        public override bool ExtractError(LoadProces proces)
        {
            SkipWhiteSpace(proces);

            //TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar.Clone();

            if (proces.TextBuffer.IsEnd(1))
            {
                //subStr.SetTo(subStr.GetFrom());
                //proces.Errors.Add(new ParserError(this, proces.TextBuffer.PointerEnd, 2, "Expecting identifier, found EOF."));
                proces.AddSyntaxError(this, proces.TextBuffer.PointerEnd, 2, "Expecting identifier, found EOF.");
                return false; // SetPointerBack(proces, from, this);
            }

            // todo brug getchar()
            if (!AllowedCharsFirst.Contains(proces.TextBuffer.GetChar().ToString().ToLower()))
            {
                //subStr.SetTo(proces.TextBuffer.PointerNextChar);
                //var element = new CodeElement(proces.TextBuffer, this, subStr, "First charactor is not allowed.");
                //errorWords.Add(element);
                //proces.Errors.Add(new ParserError(this, from, 2, "First charactor is not allowed."));
                proces.AddSyntaxError(this, from, 2, "First charactor is not allowed.");
                return false; // SetPointerBack(proces, from, this);
            }
            else
                proces.TextBuffer.IncPointer();


            // todo brug getchar()
            while (!proces.TextBuffer.IsEnd() && AllowedCharsNext.Contains(proces.TextBuffer.GetSubString(proces.TextBuffer.PointerNextChar, 1).ToLower()))
            { proces.TextBuffer.IncPointer(); }

            return true;
        }
    }
}