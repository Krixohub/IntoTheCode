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

        public override ParserElementBase CloneWithProces(LoadProces proces)
        {
            return new WordSymbol(_value) { Name = Name, Proces = proces };
        }

        public override string GetValue(ITextBuffer buf, TextSubString ptr) { return _value; }
        
        public override string GetSyntax() { return "'" + Value + "'"; }
        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {

            // todo: think: Some elements sets 'from' after white spaces

            SkipWhiteSpace(proces);
            TextPointer from = proces.TextBuffer.PointerNextChar.Clone();
            if (proces.TextBuffer.IsEnd(Value.Length))
                return false;// SetPointerBack(proces, from, this);

            foreach (char ch in Value)
                if ((proces.TextBuffer.GetChar() == ch))
                    proces.TextBuffer.IncPointer();
                else
                    return SetPointerBack(proces, from, this);

            return true;
        }

        public override bool ExtractError(LoadProces proces)
        {
            SkipWhiteSpace(proces);
            TextPointer from = proces.TextBuffer.PointerNextChar.Clone();
            TextSubString subStr = proces.TextBuffer.NewSubStringFrom();

            //TextPointer from = proces.TextBuffer.PointerNextChar.Clone();
            if (proces.TextBuffer.IsEnd(Value.Length))
            {
                //subStr.SetTo(from);
                //proces.Errors.Add(new ParserError(this, proces.TextBuffer.PointerEnd, 2, string.Format("Expecting symbol '{0}', found EOF.", Value)));
                proces.AddSyntaxError(this, proces.TextBuffer.PointerEnd, 2, string.Format("Expecting symbol '{0}', found EOF.", Value));
                return SetPointerBackError(proces, subStr.GetFrom());
            }

            foreach (char ch in Value)
                if ((proces.TextBuffer.GetChar() == ch))
                    proces.TextBuffer.IncPointer();
                else
                {
                    subStr.SetTo(subStr.GetFrom().Clone(Value.Length));
                    //subStr.SetTo(proces.TextBuffer.PointerNextChar);
                    //proces.Errors.Add(new ParserError(this, subStr.GetFrom(), 2, string.Format("Expecting symbol '{0}', found '{1}'.", Value, proces.TextBuffer.GetSubString(subStr))));
                    proces.AddSyntaxError(this, subStr.GetFrom(), 2, string.Format("Expecting symbol '{0}', found '{1}'.", Value, proces.TextBuffer.GetSubString(subStr)));
                    return SetPointerBackError(proces, from);
                }

            return true;
        }
    }
}