using System;
using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Read;
using IntoTheCode.Basic.Layer;
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
            SetSyntax(string.IsNullOrWhiteSpace(syntax) ? MetaParser.Instance.GetSyntax() : syntax);
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
            if (Rules == null || Rules.Count == 0) return "No syntax rules";
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

            LoadProces loadProces = new LoadProces(new FlatBuffer(syntax));
            syntaxDoc = CodeDocument.Load(MetaParser.Instance, loadProces);
            if (!string.IsNullOrEmpty(loadProces.LoadError))
                throw new SyntaxErrorException(loadProces.LoadError);

            if (syntaxDoc == null)
                throw new SyntaxErrorException("Can't read syntax.");
            ParserFactory.BuildRules(this, syntaxDoc);
            if (!string.IsNullOrEmpty(DefinitionError))
                throw new SyntaxErrorException(DefinitionError);

            //return true;
        }

        /// <summary>Read from a text buffer and create a text document.</summary>
        /// <param name="buf">The text buffer.</param>
        /// <returns>A text document, or null if error.</returns>
        /// <exclude/>
        internal CodeDocument ParseString(LoadProces proces)
        {
           
            try
            {
                var elements = new List<TreeNode>();
                if (!Rules[0].Load(proces, elements))
                    throw new SyntaxErrorException(string.Format("Syntax error proces '{0}'", Rules[0].Name));

                if (!proces.TextBuffer.IsEnd())
                    proces.LoadError = "End of input not reached";
                //throw new SyntaxErrorException("End of input not reached");

                if (elements.Count == 1 && elements[0] is CodeDocument)
                    return elements[0] as CodeDocument;

                if (elements.Count == 1)
                    return new CodeDocument(elements[0].SubElements) { Name = elements[0].Name };

                throw new Exception(string.Format("First rule '{0} must represent all document and have Tag=true", Rules[0].Name));
            }
            catch (Exception e)
            {
                proces.LoadError = DefinitionError == null ? e.Message: "DefinitionError: " + DefinitionError;
                return null;
            }
        }

        //#endregion Build syntax elements from dokument
    }
}
