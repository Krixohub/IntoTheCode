using System;
using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Read.Structure;

namespace IntoTheCode.Read
{
    /// <summary>A basic element (or symbol) of a Grammar. Can read a peace of code/text.</summary>
    /// <remarks>Inherids <see cref="TreeNode"/></remarks>
    public abstract class ParserElementBase : TreeNode<ParserElementBase>
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
            foreach (ParserElementBase item in ChildNodes)
                ok = ok | item.SetProperty(property, value, status);

            return ok;
        }

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
            {
                TextBuffer.GetLoopForward(null);
                ResolveErrorsForward(0);
            }

            return false;
        }

        protected List<AmbiguousDef> AmbiguousList = new List<AmbiguousDef>();

        public abstract string GetGrammar();

        public virtual void GetSettings(List<Tuple<string, string>> settings) 
        {
            if (ChildNodes == null) return;

            foreach (ParserElementBase item in ChildNodes)
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
        /// <param name="outElements">Read elements.</param>
        /// <param name="level">Level of rules.</param>
        /// <returns>True = succes.</returns>
        public abstract bool Load(List<TextElement> outElements, int level);

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
        /// <param name="level">Level of rules.</param>
        /// <returns>0: Not found, 1: Found-read error, 2: Found and read ok.</returns>
        public abstract int ResolveErrorsLast(CodeElement last, int level);

        /// <summary>Find errors in following syntax.
        /// </summary>
        /// <returns>True = no error.</returns>
        public abstract bool ResolveErrorsForward(int level);

        /// <summary>Find which RuleLinks are recursive.
        /// Called by the ParserFactory after Initializing grammar.
        /// Each path though the grammar must be followed, and each rule must have an ending.</summary>
        /// <param name="rules">Build list of analysed rules.</param>
        /// <param name="path">The path to the current element; only rules and RuleLinks.</param>
        /// <param name="status"></param>
        /// <returns>True: one path has an ending.</returns>
        public abstract bool InitializeLoop(List<Rule> rules, List<ParserElementBase> path, ParserStatus status);

        /// <summary>Find out if there is a mandatory word before a RuleLink.</summary>
        /// <param name="link">The RuleLink.</param>
        /// <returns>0: Link found (No words). 1: Mandatory word before link. 2: No words, no link. </returns>
        public abstract bool InitializeLoopHasWord(RuleLink link, List<RuleLink> subPath, ref bool linkFound);

        public List<ParserElementBase> Next;

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