using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Message;

namespace IntoTheCode.Read.Element.Words
{
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
            int from = TextBuffer.PointerNextChar;

            if (TextBuffer.IsEnd(2)) return false;

            if (TextBuffer.GetChar() != '\'')
                return false;

            TextBuffer.IncPointer();
            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar);
            TextBuffer.IncPointer();
            TextBuffer.SetToIndexOf(subStr, "'");

            if (!subStr.ToIsValid())
                //return SetPointerBack(from, this);
                return SetPointerBack(from);

            outElements.Add(new CodeElement(this, subStr));
            TextBuffer.PointerNextChar = subStr.To + 1;
            //TextBuffer.IncPointer();
            
            TextBuffer.FindNextWord(outElements, level);
            return true;
        }

        public override bool ResolveErrorsForward()
        {
            //SkipWhiteSpace();
            int from = TextBuffer.PointerNextChar;
            //int fromWordCount = wordCount;

            if (TextBuffer.IsEnd(2))
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.Length, 0, () => MessageRes.pe03);

            if (TextBuffer.GetChar() != '\'')
                return TextBuffer.Status.AddSyntaxError(this, from, 0, () => MessageRes.pe04);

            TextBuffer.IncPointer();
            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar);
            TextBuffer.IncPointer();
            TextBuffer.SetToIndexOf(subStr, "'");

            if (!subStr.ToIsValid())
            {
                TextBuffer.Status.AddSyntaxError(this, from + 1, 0, () => MessageRes.pe05);
                return SetPointerBack(from);
            }

            TextBuffer.PointerNextChar = subStr.To;
            TextBuffer.IncPointer();
            TextBuffer.FindNextWord(null, 0);

            //wordCount++;
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