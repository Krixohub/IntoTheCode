using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;

namespace IntoTheCode.Read.Element
{
    internal class Quote : ParserElementBase
    {
        internal Quote(string value)
        {
            _value = value;
            Name = "Quote";
            //Color = 2;

            // Only for unittest, when syntax is not linked.
            //Reader = reader;
        }
        //protected  string Value;

        // todo set it
        public override ElementContentType GetElementContent()
        {
            return ElementContentType.OneValue;
        }



        public override string GetSyntax() { return "'" + Value + "'"; }
        //protected override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            //TextSubString ptr = proces.TextBuffer.NewSubStringFrom();
            TextPointer from = proces.TextBuffer.PointerNextChar;
            SkipWhiteSpace(proces);
            if (proces.TextBuffer.IsEnd(Value.Length))
                return false;

            foreach (char ch in Value)
                if ((proces.TextBuffer.GetChar() == ch))
                    proces.TextBuffer.IncPointer();
                else
                    return SetPointerBack(proces, from);

            return true;
        }
    }
}