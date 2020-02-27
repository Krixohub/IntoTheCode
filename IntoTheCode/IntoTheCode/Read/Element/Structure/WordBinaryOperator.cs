
using IntoTheCode.Buffer;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Read.Element.Struckture
{
    internal class WordBinaryOperator : WordSymbol
    {
        /// <summary>Creator for <see cref="Or"/>.</summary>
        internal WordBinaryOperator(string symbol, string name) : base(symbol)
        {
            Name = name;
        }

        public int Precedence { get; internal set; }
        public bool RightAssociative { get; internal set; }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return new WordBinaryOperator(_value, Name) { TextBuffer = buffer };
        }

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

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int LoadFindLast(CodeElement last)
        {
            int rc = (SubElements[0] as ParserElementBase).LoadFindLast(last);
            if (rc < 2)
                rc = (SubElements[1] as ParserElementBase).LoadFindLast(last);

            return rc;
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            bool ok = (SubElements[0] as ParserElementBase).LoadTrackError(ref wordCount);
            ok = ok || (SubElements[1] as ParserElementBase).LoadTrackError(ref wordCount);

            return ok;
        }
    }
}