using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Message;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="WordBase"/></remarks>
    internal class WordBool : WordBase
    {
        internal WordBool()
        {
            Name = MetaParser.WordBool___;
            _valueTrue = "true";
            _valueFalse = "false";

        }

        private readonly string _valueTrue;
        private readonly string _valueFalse;

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new WordBool() { Name = Name, TextBuffer = buffer };
        }

        public override string GetGrammar() { return MetaParser.WordBool___; }

        public override bool Load(List<TextElement> outElements, int level)
        {
            TextBuffer.FindNextWord(null, false);
            int to = 0;
            bool ok = true;

            // Try reading 'true' value
            if (TextBuffer.IsEnd(_valueTrue.Length - 1))
                ok = false;
            else
            {
                foreach (char ch in _valueTrue)
                    if ((TextBuffer.GetChar(to) == ch))
                        to++;
                    else
                        ok = false;
            }

            if (!ok)
            {
                to = 0;
                if (TextBuffer.IsEnd(_valueFalse.Length - 1))
                    return false;

                foreach (char ch in _valueFalse)
                    if ((TextBuffer.GetChar(to) == ch))
                        to++;
                    else
                        return false;
            }

            outElements.Add(new CodeElement(this,
                new TextSubString(TextBuffer.PointerNextChar) { To = TextBuffer.PointerNextChar + to }));

            TextBuffer.PointerNextChar += to;
            return true;
        }

        public override bool ResolveErrorsForward(int level)
        {
            TextBuffer.FindNextWord(null, false);
            int to = 0;
            bool ok = true;

            // Try reading 'true' value
            if (TextBuffer.IsEnd(_valueTrue.Length - 1))
            {
                TextBuffer.Status.AddSyntaxError(this, TextBuffer.Length, 0, () => MessageRes.itc10, GetGrammar(), "EOF");
                ok = false;
            }
            else
            {
                foreach (char ch in _valueTrue)
                    if ((TextBuffer.GetChar(to) == ch))
                        to++;
                    else
                    {
                        if (to > 0)
                            TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerNextChar, 0, () => MessageRes.itc10,
                            GetGrammar(), "'" + TextBuffer.GetSubString(to) + "'");
                        ok = false;
                        break;
                    }
            }

            if (!ok)
            {
                to = 0;
                if (TextBuffer.IsEnd(_valueFalse.Length - 1))
                    return TextBuffer.Status.AddSyntaxError(this, TextBuffer.Length, 0, () => MessageRes.itc10, GetGrammar(), "EOF");

                foreach (char ch in _valueFalse)
                    if ((TextBuffer.GetChar(to) == ch))
                        to++;
                    else
                        return TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerNextChar, 0, () => MessageRes.itc10,
                            GetGrammar(), "'" + TextBuffer.GetSubString(to) + "'");
            }

            TextBuffer.PointerNextChar += to;
            TextBuffer.FindNextWord(null, true);
            return true;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last, int level)
        {
            if (last != null && last.ParserElement == this)
            {
                // found!
                TextBuffer.PointerNextChar = last.SubString.To + a; // + 1
                return 2;
            }
            return 0;
        }
    }
}