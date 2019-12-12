using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;

namespace IntoTheCode.Read.Element.Words
{
    internal class WordSymbol : WordBase
    {
        internal WordSymbol(string value)
        {
            _value = value;
            Name = MetaParser.WordSymbol_;
            //Color = 2;

            // Only for unittest, when syntax is not linked.
            //Reader = reader;
        }
        
        public override string GetValue(ITextBuffer buf, TextSubString ptr) { return _value; }
        
        public override string GetSyntax() { return "'" + Value + "'"; }
        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {

            // todo: think: Some elements sets 'from' after white spaces

            TextPointer from = proces.TextBuffer.PointerNextChar;
            SkipWhiteSpace(proces);
            if (proces.TextBuffer.IsEnd(Value.Length))
                return SetPointerBack(proces, from, this);

            foreach (char ch in Value)
                if ((proces.TextBuffer.GetChar() == ch))
                    proces.TextBuffer.IncPointer();
                else
                    return SetPointerBack(proces, from, this);

            return true;
        }

        public override bool ExtractError(LoadProces proces, List<CodeElement> errorWords)
        {
            TextSubString subStr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar;
            SkipWhiteSpace(proces);
            if (proces.TextBuffer.IsEnd(Value.Length))
            {
                subStr.SetTo(subStr.GetFrom());
                var element = new CodeElement(proces.TextBuffer, this, subStr, "Expecting symbol, found EOF.");
                errorWords.Add(element);
                proces.LoadErrors.Add(new LoadError(this, subStr.GetFrom(), 2, "Expecting symbol, found EOF."));
                return SetPointerBack(proces, from, this);
            }

            foreach (char ch in Value)
                if ((proces.TextBuffer.GetChar() == ch))
                    proces.TextBuffer.IncPointer();
                else
                {
                    subStr.SetTo(proces.TextBuffer.PointerNextChar);
                    var element = new CodeElement(proces.TextBuffer, this, subStr, string.Format("reading '{0}', expecting '{1}', found '{2}'.", Value, ch, proces.TextBuffer.GetChar()));
                    errorWords.Add(element);
                    proces.LoadErrors.Add(new LoadError(this, proces.TextBuffer.PointerNextChar.Clone(), 2, string.Format("reading '{0}', expecting '{1}', found '{2}'.", Value, ch, proces.TextBuffer.GetChar())));
                    return SetPointerBack(proces, from, this);
                }

            return true;
        }
    }
}