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

        public override ParserElementBase CloneForParse(ITextBuffer buffer)
        {
            return new WordSymbol(_value) { Name = Name, TextBuffer = buffer };
        }

        protected internal override string GetValue(TextSubString ptr) { return _value; }
        
        public override string GetSyntax() { return "'" + Value + "'"; }
        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TreeNode> outElements)
        {

            // todo: think: Some elements sets 'from' after white spaces

            SkipWhiteSpace();
            TextPointer from = TextBuffer.PointerNextChar.Clone();
            if (TextBuffer.IsEnd(Value.Length))
                return false;// SetPointerBack(proces, from, this);

            foreach (char ch in Value)
                if ((TextBuffer.GetChar() == ch))
                    TextBuffer.IncPointer();
                else
                    return SetPointerBack(from, this);

            return true;
        }

        public override bool ExtractError()
        {
            SkipWhiteSpace();
            TextPointer from = TextBuffer.PointerNextChar.Clone();
            TextSubString subStr = TextBuffer.NewSubStringFrom();

            //TextPointer from = TextBuffer.PointerNextChar.Clone();
            if (TextBuffer.IsEnd(Value.Length))
            {
                //subStr.SetTo(from);
                //proces.Errors.Add(new ParserError(this, TextBuffer.PointerEnd, 2, string.Format("Expecting symbol '{0}', found EOF.", Value)));
                TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerEnd, 2, string.Format("Expecting symbol '{0}', found EOF.", Value));
                return SetPointerBackError(subStr.GetFrom());
            }

            foreach (char ch in Value)
                if ((TextBuffer.GetChar() == ch))
                    TextBuffer.IncPointer();
                else
                {
                    subStr.SetTo(subStr.GetFrom().Clone(Value.Length));
                    //subStr.SetTo(TextBuffer.PointerNextChar);
                    //proces.Errors.Add(new ParserError(this, subStr.GetFrom(), 2, string.Format("Expecting symbol '{0}', found '{1}'.", Value, TextBuffer.GetSubString(subStr))));
                    TextBuffer.Status.AddSyntaxError(this, subStr.GetFrom(), 2, string.Format("Expecting symbol '{0}', found '{1}'.", Value, TextBuffer.GetSubString(subStr)));
                    return SetPointerBackError(from);
                }

            return true;
        }
    }
}