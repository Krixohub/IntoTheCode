using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Message;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="WordBase"/></remarks>
    internal class WordString : WordBase
    {
        internal WordString()
        {
            Name = "string";
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new WordString() { Name = Name, TextBuffer = buffer };
        }

        public override string GetGrammar() { return MetaParser.WordString_; }

        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TextElement> outElements, int level)
        {
            TextBuffer.FindNextWord(null, false);
            if (TextBuffer.IsEnd(1) || TextBuffer.GetChar() != '\'') return false;

            int to = TextBuffer.GetIndexAfter("'", TextBuffer.PointerNextChar + 1);
            if (to <= TextBuffer.PointerNextChar) return false;
            
            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar + 1) { To = to - 1 };
            //TextBuffer.InsertComments(outElements);
            outElements.Add(new CodeElement(this, subStr));

            TextBuffer.PointerNextChar = to;
            TextBuffer.FindNextWord(null, true);
            return true;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last, int level)
        {
            CodeElement code = last as CodeElement;
            if (code != null && code.ParserElement == this)
            {
                // found!
                TextBuffer.PointerNextChar = code.SubString.To + a + 1;
                return 2;
            }
            return 0;
        }

        public override bool ResolveErrorsForward(int level)
        {
            TextBuffer.FindNextWord(null, false);
            if (TextBuffer.IsEnd(1))
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.Length, 0, () => MessageRes.itc03);

            if (TextBuffer.GetChar() != '\'')
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerNextChar, 0, () => MessageRes.itc10, "\'", TextBuffer.GetChar());


            int to = TextBuffer.GetIndexAfter("'", TextBuffer.PointerNextChar + 1);
            if (to <= TextBuffer.PointerNextChar)
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerNextChar + 1, 0, () => MessageRes.itc05);

            TextBuffer.PointerNextChar = to;
            //TextBuffer.FindNextWord(null, true);

            return true;
        }
    }
}