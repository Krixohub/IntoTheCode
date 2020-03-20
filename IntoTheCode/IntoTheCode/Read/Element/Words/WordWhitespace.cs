using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Message;
using System;
using System.Linq;

namespace IntoTheCode.Read.Element.Words
{
    internal class WordWhitespace : WordBase
    {
        internal WordWhitespace()
        {
            Name = "comment";
        }

        public override ParserElementBase CloneForParse(TextBuffer buffer)
        {
            return null;
        }

        public override string GetGrammar() { return string.Empty; }

        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }

        public override bool Load(List<CodeElement> outElements, int level)
        {
            // Read white spaces
            while (!TextBuffer.IsEnd() && " \r\n\t".Contains(TextBuffer.GetChar()))
                TextBuffer.IncPointer();

            return true;
        }

        public override bool ResolveErrorsForward()
        {
            throw new Exception("todo");
        }

        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public override int ResolveErrorsLast(CodeElement last)
        {
            throw new Exception("todo");
        }


    }
}