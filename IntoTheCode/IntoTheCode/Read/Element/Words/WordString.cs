using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Message;

namespace IntoTheCode.Read.Element.Words
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

        public override bool Load(List<CodeElement> outElements, int level)
        {
            if (TextBuffer.IsEnd(1) || TextBuffer.GetChar() != '\'') return false;

            int to = TextBuffer.GetIndexAfter("'", TextBuffer.PointerNextChar + 1);
            if (to <= TextBuffer.PointerNextChar) return false;
            
            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar + 1) { To = to - 1 };
            outElements.Add(new CodeElement(this, subStr));

            TextBuffer.PointerNextChar = to;
            TextBuffer.FindNextWord(outElements, level);
            return true;
        }

        public override bool ResolveErrorsForward()
        {
            if (TextBuffer.IsEnd(1))
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.Length, 0, () => MessageRes.pe03);

            if (TextBuffer.GetChar() != '\'')
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerNextChar, 0, () => MessageRes.pe10, "\'", TextBuffer.GetChar());


            int to = TextBuffer.GetIndexAfter("'", TextBuffer.PointerNextChar + 1);
            if (to <= TextBuffer.PointerNextChar) 
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerNextChar + 1, 0, () => MessageRes.pe05);

            TextBuffer.PointerNextChar = to;
            TextBuffer.FindNextWord(null, 0);

            return true;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last)
        {
            if (last.WordParser == this)
            {
                // found!
                TextBuffer.PointerNextChar = last.SubString.To + a + 1;
                return 2;
            }
            return 0;
        }


    }
}