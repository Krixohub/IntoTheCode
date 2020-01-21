using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Message;

namespace IntoTheCode.Read.Element.Words
{
    internal class WordString : WordBase
    {
        internal WordString()
        {
            Name = "string";
        }

        public override ParserElementBase CloneForParse(ITextBuffer buffer)
        {
            return new WordString() { Name = Name, TextBuffer = buffer };
        }

        public override string GetSyntax() { return MetaParser.WordString_; }
        
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<TreeNode> outElements)
        {
            SkipWhiteSpace();

            TextPointer from = TextBuffer.PointerNextChar.Clone();

            if (TextBuffer.IsEnd(2)) return false;

            if (TextBuffer.GetChar() != '\'')
                return false;

            TextBuffer.IncPointer();
            TextSubString subStr = TextBuffer.NewSubStringFrom();
            TextBuffer.IncPointer();
            TextBuffer.SetToIndexOf(subStr, "'");

            if (!subStr.ToIsValid())
                return SetPointerBack(from, this);

            outElements.Add(new CodeElement(this, subStr));
            TextBuffer.SetPointerTo(subStr);
            TextBuffer.IncPointer();

            return true;
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int LoadFindLast(CodeElement last)
        {
            if (last.WordParser == this)
            {
                // found!
                TextBuffer.SetPointer(last.SubString.GetTo().Clone( a + 1));
                return 2;
            }
            return 0;
        }

        public override bool LoadTrackError(ref int wordCount)
        {
            SkipWhiteSpace();
            TextPointer from = TextBuffer.PointerNextChar.Clone();
            int fromWordCount = wordCount;

            if (TextBuffer.IsEnd(2))
                return TextBuffer.Status.AddSyntaxError(this, TextBuffer.PointerEnd, wordCount, () => MessageRes.pe03);

            if (TextBuffer.GetChar() != '\'')
                return TextBuffer.Status.AddSyntaxError(this, from, wordCount, () => MessageRes.pe04);

            TextBuffer.IncPointer();
            TextSubString subStr = TextBuffer.NewSubStringFrom();
            TextBuffer.IncPointer();
            TextBuffer.SetToIndexOf(subStr, "'");

            if (!subStr.ToIsValid())
            {
                TextBuffer.Status.AddSyntaxError(this, from.Clone(1), wordCount, () => MessageRes.pe05);
                return SetPointerBackError(from, ref wordCount, fromWordCount);
            }

            TextBuffer.SetPointerTo(subStr);
            TextBuffer.IncPointer();

            wordCount++;
            return true;
        }
    }
}