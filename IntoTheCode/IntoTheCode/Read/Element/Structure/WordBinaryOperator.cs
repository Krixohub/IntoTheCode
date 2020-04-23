
using IntoTheCode.Buffer;
using IntoTheCode.Read.Element.Words;
using System.Collections.Generic;

namespace IntoTheCode.Read.Element.Struckture
{
    /// <remarks>Inherids <see cref="WordSymbol"/></remarks>
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

        protected internal override string GetValue(TextSubString ptr) { return string.Empty; }

        public override bool Load(List<TextElement> outElements, int level)
        {
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