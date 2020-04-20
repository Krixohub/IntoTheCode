using IntoTheCode.Buffer;
using IntoTheCode.Read.Element.Words;
using System.Xml.Linq;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode
{
    /// <summary>Comments from the code.</summary>
    /// <remarks>Inherids <see cref="CodeElement"/></remarks>
    public class CommentElement : CodeElement
    {
        private readonly bool _multiline;

        internal CommentElement(WordComment reader, TextSubString pointer, bool multiline = false) :
            base (reader, pointer)
        {
            _multiline = multiline;
        }

        //internal protected override string ToMarkupProtected(string indent)
        //{
        internal protected override string ToMarkupProtected(string indent, bool xmlEncode = false)
        {
            string value;
            if (xmlEncode)
            {
                XComment elem = new XComment(Value);
                value = elem.Value;
            }
            else
                value = Value;

            string s = indent + "<!--" + value + "--!>\r\n";
            return s;
        }
    }
}
