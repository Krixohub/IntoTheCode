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

        /// <summary>Only if this symbol is transformed to an binary operator.</summary>
        public int Precedence { get; internal set; }

        /// <summary>Only if this symbol is transformed to an binary operator.</summary>
        public bool RightAssociative { get; internal set; }

        /// <summary>Override this to set a property from grammar.</summary>
        /// <param name="property">CodeElement with property name.</param>
        /// <param name="value">Value string.</param>
        /// <param name="status">If error add to this.</param>
        /// <returns>True: property set. False: not set.</returns>
        public override bool SetProperty(CodeElement property, string value, ParserStatus status)
        {
            if (property.Value == nameof(Precedence))
            {
                int val;
                if (int.TryParse(value, out val) && (val > 0))
                {
                    Precedence = val;
                    return true;
                }
                else
                    status.AddBuildError(() => MessageRes.pb10, property);
            }
            if (property.Value == nameof(RightAssociative))
            {
                RightAssociative = value != "false";
                return true;
            }

            return false;
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new WordSymbol(_value) {
                Name = Name,
                TextBuffer = buffer,
                Precedence = this.Precedence,
                RightAssociative = this.RightAssociative
            };
        }

        protected internal override string GetValue(TextSubString ptr) { return _value; }
        
        public override string GetGrammar() { return "'" + _value + "'"; }

        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<CodeElement> outElements, int level)
        {
            SkipWhiteSpace();
            int from = TextBuffer.PointerNextChar;
            if (TextBuffer.IsEnd(_value.Length))
                return false;// SetPointerBack(proces, from, this);

            foreach (char ch in _value)
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