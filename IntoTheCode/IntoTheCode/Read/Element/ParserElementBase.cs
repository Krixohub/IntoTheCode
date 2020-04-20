using System;
using System.Collections.Generic;
using System.Linq;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Read.Element
{
    /// <summary>A basic element (or symbol) of a Grammar. Can read a peace of code/text.</summary>
    /// <remarks>Inherids <see cref="TreeNode"/></remarks>
    public abstract class ParserElementBase : TreeNode
    {
        /// <summary>Clone this parser element, with sub elements, and set proces.</summary>
        /// <param name="proces">The load proces.</param>
        /// <returns>The new clone.</returns>
        public abstract ParserElementBase CloneForParse(TextBuffer buffer);

        /// <summary>Override this to set a property from grammar.</summary>
        /// <param name="property">CodeElement with property name.</param>
        /// <param name="value">Value string.</param>
        /// <param name="status">If error add to this.</param>
        /// <returns>True: property set. False: not set.</returns>
        public virtual bool SetProperty(CodeElement property, string value, ParserStatus status)
        {
            bool ok = false;
            foreach (ParserElementBase item in SubElements)
                ok = ok | item.SetProperty(property, value, status);

            return ok;
        }

        public override string GetValue() { return _value; }

        protected internal TextBuffer TextBuffer;

        protected internal CodeElement DefinitionCodeElement;

        /// <summary>If the element cant read; use this to reset (set pointer back):</summary>
        /// <param name="txtPtr">Pointer to set.</param>
        /// <returns>Always return false.</returns>
        public bool SetPointerBack(int txtPtr)
        {
            TextBuffer.PointerNextChar = txtPtr;

            // Is the error fatal?
            if (txtPtr < TextBuffer.Status.UnambiguousPointer && TextBuffer.Status.Error == null)
                ResolveErrorsForward();

            return false;
        }

        protected List<AmbiguousDef> AmbiguousList = new List<AmbiguousDef>();

        public abstract string GetGrammar();

        // todo implement GetSettings on decendants
        public virtual void GetSettings(List<Tuple<string, string>> settings) 
        {
            if (SubElements == null) return;

            foreach (ParserElementBase item in SubElements)
                item.GetSettings(settings);
        }

        //protected abstract string Read(int begin, ITextBuffer buffer);

        /// <summary>
        /// Load/read an element from the buffer, and increase the buffer pointer if element is ok.
        /// Insert output in 'outElements'.
        /// If loading fails the buffer pointer must be set back to previus point.
        /// The Buffer.Status.UnambiguousPointer is a point of no return.
        /// If the buffer pointer is set back before 'UnambiguousPointer'. Errors must be resolved.
        /// </summary>
        /// <param name="proces"></param>
        /// <param name="outElements">Read elements.</param>
        /// <returns>True = succes.</returns>
        public abstract bool Load(List<CodeElement> outElements, int level);

        /// <summary>Find the Rule/ 'read element', that correspond to the
        /// last CodeElement, and read it again with error resolving.
        /// These recursive functions are initialy called from the parser
        /// (or a SetOfElements), when
        /// the parsing is failed without any identified errors. 
        /// The unfinished part of the CodeDocument is used 
        /// to pin whitch element was the last to succeed AND reestablish a
        /// belonging chain of recursive calls. 
        /// From that point errors is tracked with the LoadTrackError function.
        /// </summary>
        /// <param name="last">Not null, not empty.</param>
        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public abstract int ResolveErrorsLast(CodeElement last);

        /// <summary>Find errors in following syntax.
        /// </summary>
        /// <returns>True = no error.</returns>
        public abstract bool ResolveErrorsForward();

        /// <summary>Find which RuleLinks are recursive.
        /// Called by the ParserFactory after Initializing grammar.
        /// Each path though the grammar must be followed, and have an ending.</summary>
        /// <param name="elem">The element to analyse.</param>
        /// <param name="path">The path to the element; only rules and RuleLinks.</param>
        /// <param name="status"></param>
        /// <returns>True: one path has an ending.</returns>
        public abstract bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status);

        
        
        public List<ParserElementBase> Next;

        //public abstract void InitializeNext(List<ParserElementBase> nextList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="org"></param>
        /// <param name="firstList"></param>
        /// <param name="followingList"></param>
        /// <returns>true: The element is optional (the list must be added to the following list). False: A word is always read.</returns>
        //public abstract bool GetFirstList(ParserElementBase org, List<ParserElementBase> firstList, List<ParserElementBase> followingList);

        //public abstract void InitializeNext(ParserElementBase org, List<ParserElementBase> nextList);

        internal Rule GetRule(ParserElementBase e)
        {
            if (e is Rule) return (Rule)e;
            else return GetRule((ParserElementBase)e.Parent);
        }
    }

    public class AmbiguousDef
    {
        public int position;
        public string EndAmbiguousChars;
    }
}