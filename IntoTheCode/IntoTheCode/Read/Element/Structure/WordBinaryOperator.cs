
using IntoTheCode.Buffer;
using IntoTheCode.Read.Element.Words;
using System.Collections.Generic;

namespace IntoTheCode.Read.Element.Struckture
{
    internal class WordBinaryOperator : WordSymbol
    {
        /// <summary>Creator for <see cref="Or"/>.</summary>
        internal WordBinaryOperator(string symbol, string name, TextBuffer buffer) : base(symbol)
        {
            Name = name;
            TextBuffer = buffer;
        }

        public int Precedence { get; internal set; }
        public bool RightAssociative { get; internal set; }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new WordBinaryOperator(_value, Name, buffer);
        }

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

        /// <summary>Set property of Binary operator.</summary>
        /// <param name="property">Property name.</param>
        /// <param name="value">Value string.</param>
        /// <returns>True: property set. False: not set.</returns>
        public override bool SetProperty(string property, string value)
        {
            switch (property)
            {
                case "Precedence":
                    int val;
                    if (!int.TryParse(value, out val))
                        return false;
                    Precedence = val;
                    break;
                case "RightAssosiative":
                    RightAssociative = Value != "false";
                    return true;
            }

            return false;
        }

        public override bool Load(List<CodeElement> outElements, int level)
        {
            //SkipWhiteSpace();

            //TextSubString subStr = new TextSubString(TextBuffer.PointerNextChar);

            if (!base.Load(outElements, level))
                return false;

            //subStr.To = TextBuffer.PointerNextChar;

            //int from = TextBuffer.PointerNextChar;

            var element = new CodeElement(this, null);
            outElements.Add(element);

            return true;
        }
    }
}