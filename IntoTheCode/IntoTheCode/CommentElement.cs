using IntoTheCode.Buffer;
using IntoTheCode.Read;
using IntoTheCode.Read.Element.Words;
using System.Xml.Linq;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode
{
    /// <summary>Comments from the Text.</summary>
    /// <remarks>Inherids <see cref="CodeElement"/></remarks>
    public class CommentElement : TextElement
    {
        private readonly bool _multiline;

        internal CommentElement(TextBuffer buffer, TextSubString pointer, bool multiline = false) 
        {
            _multiline = multiline;
            Name = MetaParser.Comment____;
            SubString = pointer;
            Buffer = buffer;
        }

        internal TextSubString SubString { get; private set; }

        internal TextBuffer Buffer { get; private set; }

        protected override string GetValue() { return Buffer.GetSubString(SubString); }

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
