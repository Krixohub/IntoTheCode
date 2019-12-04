using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;

namespace IntoTheCode.Read.Element
{
    internal class Sequence : SequenceBase
    {
        /// <summary>Creator for <see cref="Sequence"/>.</summary>
        internal Sequence(params ParserElementBase[] elements) : base(elements)
        {
            //Attributter = new ObservableCollection<Attribute>();
        }

        /// <summary>To create an unlinked syntax.</summary>
        internal Sequence()
        {
        }

        public override bool Load(LoadProces proces, List<TreeNode> outElements)
        {
            var subs = new List<TreeNode>();

            TextPointer p = proces.TextBuffer.PointerNextChar.Clone();
            while (LoadSet(proces, subs) && proces.TextBuffer.PointerNextChar.CompareTo(p) > 0) 
                proces.TextBuffer.PointerNextChar.CopyTo(p);

            foreach (var item in subs)
                outElements.Add(item);

            return true;
        }

        public override string GetSyntax() { return "{" + base.GetSyntax() + "}"; }
        //internal override string Read(int begin, ITextBuffer buffer) { return ""; }
    }
}