using System;
using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Message;
using IntoTheCode.Basic;
using IntoTheCode.Read.Element;
using System.Linq;

namespace IntoTheCode.Read
{
    /// <summary>Read text according to syntax definition.
    /// The parser holds the parser elements to read a code.
    /// Parser = a program that reads code, acording to a syntax.
    /// ParserElement = an element of a program that reads code elements, acording to a syntax element.</summary>
    public partial class Parser
    {
        internal int SymbolFixWidth;


        /// <summary>Create <see cref="Parser"/> parser.</summary>
        /// <param name="syntax">Create parser for proces syntax definition. If empty a meta syntax is created.</param>
        public Parser(string syntax)
        {
            DefinitionError = string.Empty;
            SetSyntax(string.IsNullOrWhiteSpace(syntax) ? MetaParser.SoftMetaSyntaxAndSettings : syntax);
        }

        /// <summary>Create <see cref="Parser"/>. Only for MetaParser and test.</summary>
        /// <exclude/>
        internal Parser()
        {
        }

        #region properties

        /// <summary>Syntax to read input.</summary>
        //internal Syntax Syntax;

        /// <summary>parser.Level == 1 : HardParser; parser.Level == 2: MetaParser; parser.Level == 3: SoftParser </summary>
        internal int Level = 3;
        public virtual string Name { get { return Rules == null || Rules.Count == 0 ? MetaParser.Syntax : Rules[0].Name; } }

        /// <summary>Property for parser elements.</summary>
        internal List<Rule> Rules { get; set; }

        internal string GetSyntax()
        {
            if (Rules == null || Rules.Count == 0) return MessageRes.p04;
            SymbolFixWidth = Rules.Max(eq => eq.Name.Length);
            string syntax = Rules.Aggregate(string.Empty, (ud, r) => (ud.Length == 0 ? ud : ud + "\r\n") + r.GetSyntax());
            return syntax;
        }

        /// <summary>Error message after parsing syntax definition.</summary>
        /// <exclude/>
        internal string DefinitionError { get; set; }

        #endregion properties

        /// <summary>Read a syntax definition from text. </summary>
        /// <param name="syntax">Syntax text.</param>
        /// <returns>Syntax is Ok.</returns>
        /// <exclude/>
        private void SetSyntax(string syntax)
        {
            CodeDocument syntaxDoc = null;

            ITextBuffer buffer = new FlatBuffer(syntax);
            syntaxDoc = MetaParser.Instance.ParseString(buffer);

            if (buffer.Status.Error != null || !ParserFactory.BuildRules(this, syntaxDoc, buffer.Status))
            {
                // only place to throw exception is CodeDocument.Load and Parser.SetSyntax (and MetaSyntax)
                var error = new ParserException(buffer.Status.Error.Message);
                error.AllErrors.AddRange(buffer.Status.AllErrors);
                throw error;
            }
        }

        /// <summary>Read from a text buffer and create a text document.</summary>
        /// <param name="buffer">The text buffer.</param>
        /// <returns>A text document, or null if error.</returns>
        /// <exclude/>
        internal CodeDocument ParseString(ITextBuffer buffer)
        {
            List<Rule> procesRules = Rules.Select(r => r.CloneForParse(buffer) as Rule).ToList();
            if (!ParserFactory.InitializeSyntax(this, procesRules, buffer.Status))
                return null;
            var elements = new List<TreeNode>();
            bool ok;

            try
            {
                ok = procesRules[0].Load(elements);

                // skip remaining white spaces
                procesRules[0].SkipWhiteSpace();
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
                buffer.Status.AddSyntaxErrorEof(() => MessageRes.p05);
            else if (elements.Count == 1 && elements[0] is CodeDocument)
                return elements[0] as CodeDocument;
            else if (elements.Count == 1)
                return new CodeDocument(elements[0].SubElements) { Name = elements[0].Name };
            else
                buffer.Status.AddParseError(() => MessageRes.p01, procesRules[0].Name);

            return null;
        }

        //#endregion Build syntax elements from dokument
    }
}
