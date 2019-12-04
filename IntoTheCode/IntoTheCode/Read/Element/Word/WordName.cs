using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;

namespace IntoTheCode.Read.Word
{
    internal class WordName : Word
    {
        internal WordName(string name)
        {
            Name = name;
            //ElementContent = ElementContentType.OneValue;
            //Color = 1; //  Brushes.Blue;

            // Only for unittest, when syntax is not linked.
            //Reader = parser;
        }

        public override string GetSyntax() { return MetaParser.WordName___; }
        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        private const string AllowedCharsFirst = "abcdefghijklmnopqrstuvwxyz";
        private const string AllowedCharsNext = "abcdefghijklmnopqrstuvwxyz0123456789";

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            SkipWhiteSpace(proces);

            if (proces.TextBuffer.IsEnd(1)) return false;

            TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar;

            if (!AllowedCharsFirst.Contains(proces.TextBuffer.GetSubString(proces.TextBuffer.PointerNextChar, 1).ToLower()))
                return SetPointerBack(proces, from);
            else
                proces.TextBuffer.IncPointer();


            while (!proces.TextBuffer.IsEnd() && AllowedCharsNext.Contains(proces.TextBuffer.GetSubString(proces.TextBuffer.PointerNextChar, 1).ToLower()))
            { proces.TextBuffer.IncPointer(); }
            
            subStr.SetTo(proces.TextBuffer.PointerNextChar);

            var element = new CodeElement(proces.TextBuffer, this, subStr);
            outElements.Add(element);

            return true;
        }
    }
}