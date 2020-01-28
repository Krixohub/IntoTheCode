﻿using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element.Words;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode
{
    /// <summary>The Elements that build up a CodeDokument.</summary>
    public class CodeElement : TreeNode
    {
        private readonly TextSubString _subString;

        internal CodeElement(ParserElementBase element, TextSubString pointer, string error = null)
        {
            Name = element.Name;
            _subString = pointer;
            WordParser = element as WordBase;
            Error = error;
        }

        internal TextSubString SubString { get { return _subString; } }
        internal WordBase WordParser { get; set; }

        public override string GetValue() {
            return WordParser == null ? string.Empty : WordParser.GetValue(SubString); }

        public string Error { get; private set; }

        //public bool HasColor
        //{
        //    get { return GrammarElement != null && GrammarElement.HasColor; }
        //}

        //public SolidColorBrush TextBrush
        //{
        //    get { return GrammarElement == null ? Brushes.Black : GrammarElement.TextBrush; }
        //}

        //internal byte Color
        //{
        //    get { return _color > 0 ? _color : _grammarElement == null ? (byte)0 : _grammarElement.Color; }
        //    set { _color = value; }
        //}
        //private byte _color= 0;
    }
}
