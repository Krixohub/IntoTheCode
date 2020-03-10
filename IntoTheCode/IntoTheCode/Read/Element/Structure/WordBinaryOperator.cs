
using IntoTheCode.Buffer;
using IntoTheCode.Read.Element.Words;
using System.Collections.Generic;

namespace IntoTheCode.Read.Element.Struckture
{
    internal class WordBinaryOperator : WordSymbol
    {
        /// <summary>Creator for <see cref="Or"/>.</summary>
        internal WordBinaryOperator(WordSymbol symbol, string name, TextBuffer buffer) : base(symbol.Value)
        {
            Name = name;
            TextBuffer = buffer;
            Precedence = symbol.Precedence;
            RightAssociative = symbol.RightAssociative;
        }

        //public override ParserElementBase CloneForParse(TextBuffer buffer)
        //{
        //    return new WordBinaryOperator(_value, Name, buffer);
        //}

        protected internal override string GetValue(TextSubString ptr) { return string.Empty; }
        //public override string GetValue()
        //{
        //    return base.GetValue();
        //}

        //public override ElementContentType GetElementContent()
        //{
        //    // always two elements
        //    return ElementContentType.Many; 
        //}

        public override bool Load(List<CodeElement> outElements, int level)
        {
            SkipWhiteSpace();
            TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar);

            if (!base.Load(outElements, level))
                return false;

            subStr.To = TextBuffer.PointerNextChar;
            var element = new CodeElement(this, subStr);
            outElements.Add(element);

            return true;
        }
    }
}