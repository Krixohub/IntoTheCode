using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element.Words;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode
{
    /// <summary>The Elements that build up a CodeDokument.</summary>
    /// <remarks>Inherids <see cref="TreeNode"/></remarks>
    public class ReadElement : TopElement
    {
        internal ReadElement(ParserElementBase element, TextSubString pointer)
        {
            if (element != null)
                Name = element.Name;
            SubString = pointer;
            WordParser = element as WordBase;
        }

        internal TextSubString SubString { get; private set; }
        internal WordBase WordParser { get; set; }

        public override string GetValue() {
            return WordParser == null ? string.Empty : WordParser.GetValue(SubString); }
    }
}
