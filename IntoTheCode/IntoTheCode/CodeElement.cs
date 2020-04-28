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
            WordParser = element as WordBase;
        }

        internal TextSubString SubString { get; private set; }
        internal WordBase WordParser { get; set; }

        protected override string GetValue() {
            return WordParser == null ? string.Empty : WordParser.GetWord(SubString); }
    }
}
