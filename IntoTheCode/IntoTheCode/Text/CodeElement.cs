using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element.Words;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode
{
    /// <summary>The Elements that build up a CodeDokument.</summary>
    public class CodeElement : TreeNode
    {
        private readonly ITextBuffer _buffer;
        private readonly ParserElementBase _syntaxElement;
        private readonly TextSubString _valuePointer;

        internal CodeElement(ITextBuffer buffer, ParserElementBase element, TextSubString pointer, string error = null)
        {
            Name = element.Name;
            _buffer = buffer;
            _syntaxElement = element;
            _valuePointer = pointer;
            ValueReader = element as WordBase;
            Error = error;
        }

        internal TextSubString ValuePointer { get { return _valuePointer; } }
        internal WordBase ValueReader { get; set; }
        //internal int ValueLength { get { return ValuePointer == null ? 0 : ValuePointer.Length(); }}

        public override string GetValue() {
            //ParserElement reader = ValueReader == null ? _syntaxElement : ValueReader;
            return ValueReader == null ? string.Empty : ValueReader.GetValue(_buffer, ValuePointer); }

        public string Error { get; private set; }

        //public bool HasColor
        //{
        //    get { return SyntaxElement != null && SyntaxElement.HasColor; }
        //}

        //public SolidColorBrush TextBrush
        //{
        //    get { return SyntaxElement == null ? Brushes.Black : SyntaxElement.TextBrush; }
        //}

        //internal byte Color
        //{
        //    get { return _color > 0 ? _color : _syntaxElement == null ? (byte)0 : _syntaxElement.Color; }
        //    set { _color = value; }
        //}
        //private byte _color= 0;
    }
}
