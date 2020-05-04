using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Message;
using System.Linq;
using IntoTheCode.Read.Structure;

namespace IntoTheCode.Read.Words
{
    /// <remarks>Inherids <see cref="WordBase"/></remarks>
    internal class WordInt : WordBase
    {
        internal WordInt()
        {
            Name = "int";
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new WordInt() { Name = Name, TextBuffer = buffer };
        }

        public override string GetGrammar() { return MetaParser.WordInt____; }

        private const char Sign = '-';
        private const string AllowedChars = "0123456789";

        public override bool Load(List<TextElement> outElements, int level)
        {
            TextBuffer.FindNextWord(null, false);
            int to = 0;
            if (TextBuffer.IsEnd(to)) return false;
            if (Sign == TextBuffer.GetChar())
            {
                to++;
                if (TextBuffer.IsEnd(to))
                    return false;
            }
            if (!AllowedChars.Contains(TextBuffer.GetChar(to)))
                return false;

            to++;

            while (!TextBuffer.IsEnd(to) && AllowedChars.Contains(TextBuffer.GetChar(to)))
            { to++; }

            if (to > 9 && !int.TryParse(TextBuffer.GetSubString(TextBuffer.PointerNextChar, to), out _))
                    return false;

            //TextBuffer.InsertComments(outElements);
            outElements.Add(new CodeElement(this, 
                new TextSubString(TextBuffer.PointerNextChar) { To = TextBuffer.PointerNextChar + to }));

            TextBuffer.PointerNextChar += to;
            //TextBuffer.FindNextWord(outElements, true);
            return true;
        }

        public override bool ResolveErrorsForward(int level)
        {

            TextBuffer.FindNextWord(null, false);
            int to = 0;
            if (TextBuffer.IsEnd(1))
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.Length, 0, () => MessageRes.itc10, GetGrammar(), "EOF");

            //if (Sign == TextBuffer.GetChar()) to++;
            if (Sign == TextBuffer.GetChar())
            {
                to++;
                if (TextBuffer.IsEnd(to))
                    return TextBuffer.Status.AddSyntaxError(this, TextBuffer.Length, 0, () => MessageRes.itc10, GetGrammar(), "EOF");
            }

            if (!AllowedChars.Contains(TextBuffer.GetChar(to)))
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerNextChar + to, 0, () => MessageRes.itc10, GetGrammar(), TextBuffer.GetChar(to));

            to++;

            while (!TextBuffer.IsEnd(to) && AllowedChars.Contains(TextBuffer.GetChar(to)))
            { to++; }

            if (!int.TryParse(TextBuffer.GetSubString(TextBuffer.PointerNextChar, to), out _))
                return TextBuffer.Status.AddSyntaxError(this,
                    to, 0, () => MessageRes.pe11, TextBuffer.GetSubString(TextBuffer.PointerNextChar, to));

            TextBuffer.PointerNextChar += to;
            TextBuffer.FindNextWord(null, true);
            return true;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last, int level)
        {
            CodeElement code = last as CodeElement;
            if (code != null && code.WordParser == this)
            {
                // found!
                TextBuffer.PointerNextChar = code.SubString.To + a + 1;
                return 2;
            }
            return 0;
        }


    }
}