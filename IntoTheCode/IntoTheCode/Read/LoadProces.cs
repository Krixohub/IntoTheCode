using IntoTheCode.Buffer;
using IntoTheCode.Read.Element;
using System.Collections.Generic;

namespace IntoTheCode.Read
{
    public class LoadProces
    {
        /// <summary>Field for text buffer.</summary>
        private ITextBuffer _textBuffer;
        
        internal LoadProces(ITextBuffer buf)
        {
            TextBuffer = buf;
            ErrorMsg = string.Empty;
            UnambiguousPointer = new FlatPointer();
        }

        /// <summary>The text buffer to read from.</summary>
        internal ITextBuffer TextBuffer
        {
            get { return _textBuffer; }
            set { _textBuffer = value; TextBuffer.NewPointer(); }
        }

        /// <summary>Error message after parsing/reading input text.</summary>
        /// <exclude/>
        public string ErrorMsg { get; internal set; }

        /// <summary>Error message after parsing/reading input text.</summary>
        /// <exclude/>
        public bool Error { get; internal set; }

        /// <summary>Error message after parsing/reading input text.</summary>
        /// <exclude/>
        public List<LoadError> Errors { get; internal set; }

        #region Next 

        public void ThisIsUnambiguous(ParserElementBase reader, CodeElement code)
        {
            // Set SafePointer to char after element
            UnambiguousPointer = code.ValuePointer.GetTo();
            //UnambiguousWordCount = numberOfWords; 
        }

        public TextPointer UnambiguousPointer;
        //public int UnambiguousWordCount;

        #endregion Next 
    }
}
