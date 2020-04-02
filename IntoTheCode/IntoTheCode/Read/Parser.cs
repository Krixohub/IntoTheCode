using System;
using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Message;
using IntoTheCode.Read.Element;
using System.Linq;

namespace IntoTheCode.Read
{
    /// <summary>Read text according to Grammar definition.
    /// The parser holds the parser elements to read a code.
    /// Parser = a program that reads code, acording to a Grammar.
    /// ParserElement = an element of a program that reads code elements, acording to a Grammar element.</summary>
    public partial class Parser
    {
        internal int SymbolFixWidth;


        /// <summary>Create <see cref="Parser"/> parser.</summary>
        /// <param name="Grammar">Create parser for proces Grammar definition. If empty a meta Grammar is created.</param>
        public Parser(string Grammar)
        {
            SetGrammar(string.IsNullOrWhiteSpace(Grammar) ? MetaParser.SoftMetaGrammarAndSettings : Grammar);
        }

        /// <summary>Create <see cref="Parser"/>. Only for MetaParser and test.</summary>
        /// <exclude/>
        internal Parser()
        {
        }

        #region properties

        /// <summary>Grammar to read input.</summary>
        //internal Grammar Grammar;

        /// <summary>parser.Level == 1 : HardParser; parser.Level == 2: MetaParser; parser.Level == 3: SoftParser </summary>
        internal int Level = 3;
        public virtual string Name { get { return Rules == null || Rules.Count == 0 ? MetaParser.Grammar : Rules[0].Name; } }

        /// <summary>Property for parser elements.</summary>
        internal List<Rule> Rules { get; set; }

        internal string GetGrammar()
        {
            if (Rules == null || Rules.Count == 0) return MessageRes.p04;
            SymbolFixWidth = Rules.Max(eq => eq.Name.Length);
            string Grammar = Rules.Aggregate(string.Empty, (ud, r) => (ud.Length == 0 ? ud : ud + "\r\n") + r.GetGrammar());
            return Grammar;
        }

        #endregion properties

        /// <summary>Read a Grammar definition from text. </summary>
        /// <param name="Grammar">Grammar text.</param>
        /// <returns>Grammar is Ok.</returns>
        /// <exclude/>
        private void SetGrammar(string Grammar)
        {
            CodeDocument grammarDoc = null;

            TextBuffer buffer = new FlatBuffer(Grammar);
            grammarDoc = MetaParser.Instance.ParseString(buffer);

            if (buffer.Status.Error != null || !ParserFactory.BuildRules(this, grammarDoc, buffer.Status))
            {
                // only place to throw exception is CodeDocument.Load and Parser.SetGrammar (and MetaGrammar)
                var error = new ParserException(buffer.Status.Error.Message);
                error.AllErrors.AddRange(buffer.Status.AllErrors);
                throw error;
            }
        }

        /// <summary>Read from a text buffer and create a text document.</summary>
        /// <param name="buffer">The text buffer.</param>
        /// <returns>A text document, or null if error.</returns>
        /// <exclude/>
        internal CodeDocument ParseString(TextBuffer buffer)
        {

            if (buffer.Status.Error != null) return null;

            List<Rule> procesRules = Rules.Select(r => r.CloneForParse(buffer) as Rule).ToList();
            if (!ParserFactory.InitializeGrammar(this, procesRules, buffer.Status))
                return null;
            var elements = new List<CodeElement>();
            bool ok;

            try
            {
                // skip preceding white spaces and comments
                buffer.FindNextWord(elements, 0);
                
                ok = procesRules[0].Load(elements, 0);
            }
            catch (Exception e)
            {
                buffer.Status.AddException(e, () => MessageRes.p02, e.Message);
                return null;
            }

            if (buffer.Status.Error != null) return null;

            if (!ok)
                buffer.Status.AddParseError(() => MessageRes.p02, procesRules[0].Name);
            else if (!buffer.IsEnd())
            {
                if (elements != null && elements.Count() > 0)
                {
                    CodeElement last = elements.Last();
                    string debug = last.ToMarkupProtected(string.Empty);
                    procesRules[0].ResolveErrorsLast(last);
                }
                buffer.Status.AddSyntaxErrorEof(() => MessageRes.p05);
            }
            else if (elements.Count == 1 && elements[0] is CodeDocument)
                return elements[0] as CodeDocument;
            else if (elements.Count == 1)
                // todo get the document name from procesRules[0]
                return new CodeDocument(elements[0].SubElements, procesRules[0]) { Name = elements[0].Name };
            else
                buffer.Status.AddParseError(() => MessageRes.p01, procesRules[0].Name);

            return null;
        }
    }
}
