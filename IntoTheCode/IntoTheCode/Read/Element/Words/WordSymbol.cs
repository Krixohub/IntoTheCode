using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Message;

namespace IntoTheCode.Read.Element.Words
{
    internal class WordSymbol : WordBase
    {
        internal WordSymbol(string value)
        {
            _value = value;
            Name = MetaParser.WordSymbol_;
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new WordSymbol(_value) { Name = Name, TextBuffer = buffer };
        }

        protected internal override string GetValue(TextSubString ptr) { return _value; }
        
        public override string GetSyntax() { return "'" + Value + "'"; }
        
        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TreeNode> outElements)
        {
            SkipWhiteSpace();
            int from = TextBuffer.PointerNextChar;
            if (TextBuffer.IsEnd(Value.Length))
                return false;// SetPointerBack(proces, from, this);

            foreach (char ch in Value)
                if ((TextBuffer.GetChar() == ch))
                    TextBuffer.IncPointer();
                else
                    return SetPointerBack(from, this);

            return true;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int LoadFindLast(CodeElement last)
        {
            if (last.WordParser == this)
            {
                // found!
                //TextBuffer.SetPointer(last.SubString.GetTo().Clone(a));
                TextBuffer.PointerNextChar = last.SubString.To + a;
                return 2;
            }
            return 0;
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            SkipWhiteSpace();
            int from = TextBuffer.PointerNextChar;
            int fromWordCount = wordCount;
            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar);

            if (TextBuffer.IsEnd(Value.Length))
            {
                TextBuffer.Status.AddSyntaxError(this, TextBuffer.Length, wordCount, () => MessageRes.pe06, Value);
                return SetPointerBackError(subStr.From, ref wordCount, fromWordCount);
            }

            foreach (char ch in Value)
                if ((TextBuffer.GetChar() == ch))
                    TextBuffer.IncPointer();
                else
                {
                    subStr.To = subStr.From + Value.Length;
                    TextBuffer.Status.AddSyntaxError(this, subStr.From, wordCount, () => MessageRes.pe07, Value, TextBuffer.GetSubString(subStr));
                    return SetPointerBackError(from, ref wordCount, fromWordCount);
                }

            wordCount++;
            return true;
        }
    }
}