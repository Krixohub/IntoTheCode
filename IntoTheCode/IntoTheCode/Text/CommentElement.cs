using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element.Words;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode
{
    /// <summary>The Elements that build up a CodeDokument.</summary>
    public class CommentElement : TreeNode
    {
        private readonly TextSubString _subString;

        internal CommentElement(string comment, TextSubString pointer, bool multiline = false)
        {
            //Name = "Comment";
            //_subString = pointer;
            //WordParser = element as WordBase;
        }

        internal TextSubString SubString { get { return _subString; } }
        internal WordBase WordParser { get; set; }

        public override string GetValue()
        {
            return WordParser == null ? string.Empty : WordParser.GetValue(SubString);
        }
    }
}
