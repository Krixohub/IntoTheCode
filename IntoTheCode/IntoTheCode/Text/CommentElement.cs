using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element.Words;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode
{
    /// <summary>The Elements that build up a CodeDokument.</summary>
    public class CommentElement : CodeElement
    {
        private readonly bool _multiline;

        internal CommentElement(WordComment reader, TextSubString pointer, bool multiline = false) :
            base (reader, pointer)
        {
            _multiline = multiline;
        }

        internal protected override string ToMarkupProtected(string indent)
        {
            string s = indent + "<!--" + Value + "--!>\r\n";
            return s;
        }
    }
}
