using System;
using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Message;
using IntoTheCode.Read;
using IntoTheCode.Read.Structure;
using System.Linq;
using IntoTheCode.Basic.Util;

namespace IntoTheCode
{
    /// <summary>Read text according to a grammar.
    /// The parser holds the parser elements to read a code.
    /// Parser = a program that reads code, acording to a grammar.
    /// ParserElement = an element of a program that reads code elements, acording to a syntax element.</summary>
    public class Parser
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

        /// <summary>parser.Level == 1 : HardParser; parser.Level == 2: MetaParser; parser.Level == 3: SoftParser </summary>
        internal int Level = 3;

        /// <summary>Name of grammar; The name of the first rule.</summary>
        public virtual string Name { get { return Rules == null || Rules.Count == 0 ? MetaParser.Grammar : Rules[0].Name; } }

        /// <summary>Property for parser elements.</summary>
        internal List<Rule> Rules { get; set; }

        private string GetSyntax()
        {
            if (Rules == null || Rules.Count == 0) return MessageRes.itc22;
            SymbolFixWidth = Rules.Max(eq => eq.Name.Length);
            return Rules.Aggregate(string.Empty, (ud, r) => (ud.Length == 0 ? ud : ud + "\r\n") + r.GetGrammar());
        }

        /// <summary>Get formatted grammar for this parser.</summary>
        /// <returns>A grammar.</returns>
        public string GetGrammar()
        {
            string Grammar = GetSyntax();

            var settings = new List<Tuple<string, string>>();
            foreach (var rule in Rules)
                rule.GetSettings(settings);

            if (settings.Count > 0)
            {
                Grammar = Grammar.NL() + MetaParser.Settings___;
                List<String> done = new List<string>();

                foreach (var set in settings)
                {
                    if (!done.Contains(set.Item1))
                    {
                        done.Add(set.Item1);
                        Grammar = Grammar.NL() + set.Item1.PadRight(SymbolFixWidth) + " ";
                        Grammar += string.Join(" , ", settings.Where(s => s.Item1 == set.Item1).Select(s => s.Item2));
                        Grammar += ";";
                    }
                }

            }
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

            if (buffer.Status.Error != null || !ParserBuilder.BuildRules(this, grammarDoc, buffer.Status))
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
            if (!ParserBuilder.InitializeGrammar(this, procesRules, buffer.Status))
                return null;
            var elements = new List<TextElement>();
            bool ok;

            var resultDoc = new CodeDocument(elements, procesRules[0].Name);

            try
            {
                // skip preceding white spaces and comments
                buffer.FindNextWord(resultDoc.ChildNodes, false);
                //buffer.InsertComments(elements);
                
                ok = procesRules[0].Load(elements, 0);

                buffer.FindNextWord(elements, false);
                //buffer.InsertComments(elements);
            }
            catch (Exception e)
            {
                buffer.Status.AddException(e, () => MessageRes.itc12, e.Message);
                return null;
            }

            if (buffer.Status.Error != null) return null;

            if (!ok)
                buffer.Status.AddParseError(() => MessageRes.itc12, procesRules[0].Name);
            else if (!buffer.IsEnd())
            {
                if (elements != null && elements.Count() > 0)
                {
                    CodeElement last = elements.OfType<CodeElement>().Last();
                    string debug = last.ToMarkupProtected(string.Empty);
                    buffer.GetLoopLast(null);
                    procesRules[0].ResolveErrorsLast(last, 0);
                }
                buffer.Status.AddSyntaxErrorEof(() => MessageRes.itc13);
            }
            else if (elements.OfType<CodeElement>().Count() == 1)
            {
                resultDoc.AddRange(elements.OfType<CodeElement>().First().ChildNodes);
                resultDoc.AddRange(elements.OfType<CommentElement>());
                return resultDoc;
            }
            else
                buffer.Status.AddParseError(() => MessageRes.itc20, procesRules[0].Name);

            return null;
        }
    }
}
