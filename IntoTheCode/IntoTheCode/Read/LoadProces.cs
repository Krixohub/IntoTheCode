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
            LoadError = string.Empty;
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
        public string LoadError { get; internal set; }

        /// <summary>Error message after parsing/reading input text.</summary>
        /// <exclude/>
        public List<LoadError> LoadErrors { get; internal set; }

        #region Next 

        public void ThisIsUnambiguous(ParserElementBase reader, CodeElement code, int numberOfWords)
        {
            // Set SafePointer to char after element
            UnambiguousPointer = code.ValuePointer.GetTo();
            UnambiguousWordCount = numberOfWords; 
        }

        public TextPointer UnambiguousPointer;
        public int UnambiguousWordCount;

        #endregion Next 
    }
}
