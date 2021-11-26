
using IntoTheCode.Buffer;
using System.Collections.Generic;

namespace IntoTheCode.Grammar
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

        //public bool Complete { get; set; }

        protected internal override string GetWord(TextSubString ptr) { return string.Empty; }

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