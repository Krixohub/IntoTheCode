using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Message;

namespace IntoTheCode.Read.Words
{
    /// <remarks>Inherids <see cref="WordBase"/></remarks>
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

        protected internal override string GetWord(TextSubString ptr) { return _value; }
        
        public override string GetGrammar() { return "'" + _value + "'"; }

        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TextElement> outElements, int level)
        {
            TextBuffer.FindNextWord(outElements, false);
            int from = TextBuffer.PointerNextChar;
            if (TextBuffer.IsEnd(_value.Length - 1))
                return false;// SetPointerBack(proces, from, this);

            foreach (char ch in _value)
                if ((TextBuffer.GetChar() == ch))
                    TextBuffer.IncPointer();
                else
                    //return SetPointerBack(from, this);
                    return SetPointerBack(from);

            TextBuffer.InsertComments(outElements);
            TextBuffer.FindNextWord(outElements, true);
            return true;
        }

        public override bool ResolveErrorsForward()
        {
            TextBuffer.FindNextWord(null, false);
            int from = TextBuffer.PointerNextChar;
            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar);

            if (TextBuffer.IsEnd(Value.Length -1))
            {
                TextBuffer.Status.AddSyntaxError(this, TextBuffer.Length, 0, () => MessageRes.pe10, MetaParser.WordSymbol_ +  " " + GetGrammar(), "EOF");
                return SetPointerBack(subStr.From);
            }

            foreach (char ch in Value)
                if ((TextBuffer.GetChar() == ch))
                    TextBuffer.IncPointer();
                else
                {
                    subStr.To = subStr.From + Value.Length;
                    TextBuffer.Status.AddSyntaxError(this, subStr.From, 0, () => MessageRes.pe07, Value, TextBuffer.GetSubString(subStr));
                    return SetPointerBack(subStr.From);
                }

            TextBuffer.FindNextWord(null, true);
            return true;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(TextElement last)
        {
            CodeElement code = last as CodeElement;
            if (code != null && code.WordParser == this)
            {
                // found!
                TextBuffer.PointerNextChar = code.SubString.To + a;
                return 2;
            }
            return 0;
        }
    }
}