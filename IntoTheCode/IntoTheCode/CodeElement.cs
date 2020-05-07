using IntoTheCode.Buffer;
using IntoTheCode.Read;
using IntoTheCode.Read.Words;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode
{
    /// <summary>The Elements that is read as part of a syntax.</summary>
    /// <remarks>Inherids <see cref="TextElement"/></remarks>
    public class CodeElement : TextElement
    {
        internal CodeElement(ParserElementBase element, TextSubString pointer)
        {
            if (element != null)
                Name = element.Name;
            SubString = pointer;
            ParserElement = element;
        }

        internal TextSubString SubString { get; private set; }
        internal ParserElementBase ParserElement { get; set; }

        protected override string GetValue()
        {
            var _wordParser = ParserElement as WordBase;
            return _wordParser == null ? string.Empty : _wordParser.GetWord(SubString);
        }

        public string GetLineAndColumn()
        {
            int line;
            int col;
            return ParserElement.TextBuffer.GetLineAndColumn(out line, out col, SubString.From);
        }
    }
}
