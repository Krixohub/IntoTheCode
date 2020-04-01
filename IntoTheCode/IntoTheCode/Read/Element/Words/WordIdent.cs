using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System;
using IntoTheCode.Message;

namespace IntoTheCode.Read.Element.Words
{
    internal class WordIdent : WordBase
    {
        internal WordIdent(string name)
        {
            Name = name;
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new WordIdent(Name) { TextBuffer = buffer };
        }

        public override string GetGrammar() { return MetaParser.WordIdent__; }

        private const string AllowedCharsFirst = "abcdefghijklmnopqrstuvwxyz";
        private const string AllowedCharsNext = "abcdefghijklmn+<&opqrstuvwxyz0123456789";

        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<CodeElement> outElements, int level)
        {
            //SkipWhiteSpace();

            if (TextBuffer.IsEnd(1)) return false;

            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar);

            if (!AllowedCharsFirst.Contains(TextBuffer.GetChar().ToString().ToLower()))
                return false;
            else
                TextBuffer.IncPointer();

            while (!TextBuffer.IsEnd() && AllowedCharsNext.Contains(TextBuffer.GetChar().ToString().ToLower()))
            { TextBuffer.IncPointer(); }
            
            subStr.To = TextBuffer.PointerNextChar;

            outElements.Add(new CodeElement(this, subStr));

            TextBuffer.FindNextWord(outElements, level);
            return true;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last)
        {
            if (last.WordParser == this)
            {
                // found!
                TextBuffer.PointerNextChar = last.SubString.To + a;
                return 2;
            }
            return 0;
        }

        public override bool ResolveErrorsForward()
        {
            int from = TextBuffer.PointerNextChar;

            if (TextBuffer.IsEnd(1))
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.Length, 0, () => MessageRes.pe01);

            if (!AllowedCharsFirst.Contains(TextBuffer.GetChar().ToString().ToLower()))
                return TextBuffer.Status.AddSyntaxError(this, from, 0, () => MessageRes.pe10, GetGrammar(), TextBuffer.GetChar());
            else
                TextBuffer.IncPointer();

            while (!TextBuffer.IsEnd() && AllowedCharsNext.Contains(TextBuffer.GetSubString(TextBuffer.PointerNextChar, 1).ToLower()))
            { TextBuffer.IncPointer(); }

            TextBuffer.FindNextWord(null, 0);
            return true;
        }
    }
}