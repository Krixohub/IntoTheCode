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
            //Color = 2;

            // Only for unittest, when syntax is not linked.
            //Reader = reader;
        }

        public override ParserElementBase CloneForParse(ITextBuffer buffer)
        {
            return new WordSymbol(_value) { Name = Name, TextBuffer = buffer };
        }

        protected internal override string GetValue(TextSubString ptr) { return _value; }
        
        public override string GetSyntax() { return "'" + Value + "'"; }
        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TreeNode> outElements)
        {

            // todo: think: Some elements sets 'from' after white spaces

            SkipWhiteSpace();
            TextPointer from = TextBuffer.PointerNextChar.Clone();
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
        public override int TryLastAgain(CodeElement last)
        {
            if (last.WordParser == this)
            {
                // found!
                TextBuffer.SetPointer(last.SubString.GetTo().Clone(a));
                return 2;
            }
            return 0;
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            SkipWhiteSpace();
            TextPointer from = TextBuffer.PointerNextChar.Clone();
            int fromWordCount = wordCount;
            TextSubString subStr = TextBuffer.NewSubStringFrom();

            //TextPointer from = TextBuffer.PointerNextChar.Clone();
            if (TextBuffer.IsEnd(Value.Length))
            {
                //subStr.SetTo(from);
//                TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerEnd, 2, string.Format("Expecting symbol '{0}', found EOF.", Value));
                TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerEnd, wordCount, () => MessageRes.pe06, Value);
                return SetPointerBackError(subStr.GetFrom(), ref wordCount, fromWordCount);
            }

            foreach (char ch in Value)
                if ((TextBuffer.GetChar() == ch))
                    TextBuffer.IncPointer();
                else
                {
                    subStr.SetTo(subStr.GetFrom().Clone(Value.Length));
                    //TextBuffer.Status.AddSyntaxError(this, subStr.GetFrom(), 2, string.Format("Expecting symbol '{0}', found '{1}'.", Value, TextBuffer.GetSubString(subStr)));
                    TextBuffer.Status.AddSyntaxError(this, subStr.GetFrom(), wordCount, () => MessageRes.pe07, Value, TextBuffer.GetSubString(subStr));
                    return SetPointerBackError(from, ref wordCount, fromWordCount);
                }

            wordCount++;
            return true;
        }
    }
}