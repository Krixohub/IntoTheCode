using System.Collections.Generic;

using IntoTheCode.Read.Structure;
using System;
using IntoTheCode.Buffer;
using IntoTheCode.Read.Words;

namespace IntoTheCode.Read
{
    /// <summary>The meta-grammar for proces grammar definitions.
    /// The Instance property is a singleton parser of grammares.
    /// This class contains a hard coded parser and a string MetaGrammer, 
    /// to build rules of the meta parser.</summary>
    internal class MetaParser //: Grammar
    {
        // Make static meta parser and grammar thread safe.
        private static object _syncMetaParser = new object();

        private static Parser _instance;

        /// <summary>A linked grammar for EBNF.</summary>
        internal MetaParser()
        {
        }

        internal static Parser Instance
        {
            get
            {
                lock (_syncMetaParser)
                    if (_instance == null) CreateMetaParser();
                return _instance;
            }
        }

        #region element const names

        /// <summary>Tokens.</summary>
        internal const string Grammar      = "Grammar";      // Grammer
        internal const string MetaGrammar_ = "MetaGrammar";  // Grammer
        internal const string HardGrammar_ = "HardGrammar";  // Grammer
        internal const string Rule_______ = "rule";        // Equation , Statement, Action, Definition, Assignment,
        //internal const string RuleId_____ = "ruleId";      // ruleId, Word, Symbol(bnf), Term, literal,
        internal const string Expression_ = "expression";  // 
        internal const string Element____ = "element";     // 
        internal const string Block______ = "block";       // 
        internal const string Sequence___ = "sequence";    // repetition, sequence
        internal const string Optional___ = "optional";    // 
        internal const string Parentheses = "parentheses"; // 
        internal const string Or_________ = "or";          // 
        internal const string WordIdent__ = "identifier";  // id, name, word, ident, identifier(ebnf)
        internal const string WordString_ = "string";      // string, Token, symbol, name
        internal const string WordInt____ = "int";         // string, Token, symbol, name
        internal const string WordSymbol_ = "symbol";      // wordsymbol, symbol
        //internal const string Operator___ = "operator";    // 
        //internal const string Equal______ = "equal";       // 

        // Settings
        internal const string Settings___ = "settings";    // 
        internal const string Setter_____ = "setter";      // 
        internal const string Assignment_ = "assignment";  // 
        internal const string Property___ = "property";    // 
        internal const string Value______ = "value";       // 
        //internal const string HardCode = "hc_";          // 
        //internal const string Tag________ = "tag";         // 
        internal const string Collapse___ = "collapse";    // 
        internal const string Trust______ = "trust";   // 
        internal const string Comment____ = "comment";     // 
        //internal const string Whitespace_ = "ws";          // 
        //internal const string WsDef______ = "wsdef";       // 

        #endregion element const names

        /// <summary>The meta grammar for proces other grammares.</summary>
        internal static string SoftMetaGrammarAndSettings { get { return MetaGrammar + MetaSettings; } }

        /// <summary>The meta grammar for proces other grammares.</summary>
        internal static string MetaGrammar
        {
            get
            {
                string grammar = @"MetaGrammar = {rule} [settings];
rule        = identifier '=' expression ';';
expression  = element {[or] element};
element     = identifier | symbol | sequence | optional | parentheses;
sequence    = '{' expression '}';
optional    = '[' expression ']';
parentheses = '(' expression ')';
or          = '|';
symbol      = string;
settings    = 'settings' {setter};
setter      = identifier assignment {',' assignment} ';';
assignment  = property ['=' value];
property    = identifier;
value       = string;";
                return grammar;
            }
        }

        // property    = 'collapse' | 'trust';

        /// <summary>The shapin grammar.</summary>
        internal static string MetaSettings
        {
            get
            {
                string grammar = @"
settings
rule        comment;
expression  collapse;
element     collapse;
settings    collapse;";
                //    return "";
                //                string grammar = @"
                //settings
                //expression collapse;
                //element    collapse;
                //block      collapse;
                //settings   collapse;";
                return grammar;
            }
        }

        internal static Parser GetHardCodeParser(ParserStatus status)
        {
            List<Rule> list = new List<Rule>();

            // Build Mo Backus Naur Form in code.
            // grammar   = {rule} [settings];
            list.Add(new Rule(HardGrammar_,
                new Sequence(new RuleLink(Rule_______)),
                new Optional(new RuleLink(Settings___)))
                { Comment = true });

            // rule = identifier '=' expression ';'
            list.Add(new Rule(Rule_______,
                new WordIdent(),
                new WordSymbol("="),
                new RuleLink(Expression_),
                new WordSymbol(";")));

            // expression = element {[or] element};
            list.Add(new Rule(Expression_,
                new RuleLink(Element____),
                new Sequence(
                    new Optional(
                        new RuleLink(Or_________)),
                    new RuleLink(Element____)))
            { Collapse = true });

            // element    = identifier | symbol | block;
            list.Add(new Rule(Element____,
                new Or(new WordIdent(),
                    new Or(new RuleLink(WordSymbol_),
                        new RuleLink(Block______))))
            { Collapse = true });

            // block      = sequence | optional | parentheses;
            list.Add(new Rule(Block______,
                new Or(new RuleLink(Sequence___),
                    new Or(new RuleLink(Optional___),
                        new RuleLink(Parentheses))))
            { Collapse = true });

            // sequence     = '{' expression '}';
            list.Add(new Rule(Sequence___,
                new WordSymbol("{"),
                new RuleLink(Expression_),
                new WordSymbol("}")));
            // optional     = '[' expression ']';
            list.Add(new Rule(Optional___,
                new WordSymbol("["),
                new RuleLink(Expression_),
                new WordSymbol("]")));
            // parentheses      = '(' expression ')';
            list.Add(new Rule(Parentheses,
                new WordSymbol("("),
                new RuleLink(Expression_),
                new WordSymbol(")")));
            // or         = '|';
            list.Add(new Rule(Or_________,
                new WordSymbol("|")));

            //// ruleId     = identifier;
            //// todo eliminate ruleIds with identifier or string
            //list.Add(new Rule(RuleId_____,
            //    new WordName(WordName___)));

            // symbol       = string;
            list.Add(new Rule(WordSymbol_,
                new WordString()));

            // settings   > 'settings' {setter};
            list.Add(new Rule(Settings___,
                new WordSymbol(Settings___),
                new Sequence(
                    new RuleLink(Setter_____)))
            { Collapse = true });


            // setter     > identifier assignment {',' assignment} ';';
            list.Add(new Rule("setter",
                new WordIdent(),
                new RuleLink(Assignment_),
                new Sequence(
                    new WordSymbol(","),
                    new RuleLink(Assignment_)),
                new WordSymbol(";")));

            // assignment > property '=' value;
            list.Add(new Rule(Assignment_,
                new RuleLink(Property___),
                new Optional(
                    new WordSymbol("="),
                    new RuleLink(Value______))));


            // property   > identifier;
            list.Add(new Rule(Property___,
                new WordIdent()));

            // value      > string;";
            list.Add(new Rule(Value______,
                new WordString()));

            Parser parser = new Parser() { Level = 1 }; // { Name = HardGrammar_ };
            parser.Rules = list;
            //foreach (var eq in list) eq.Parser = parser;
            ParserFactory.InitializeGrammar(parser, parser.Rules, status);

            return parser;
        }

        /// <summary>Create the parser to read grammar definition</summary>
        /// <exclude/>
        private static void CreateMetaParser()
        {
            if (_instance != null) return;
            int step = 1;
            TextBuffer buffer = new FlatBuffer(MetaParser.SoftMetaGrammarAndSettings);
            TextDocument metaGrammarDoc = null;
            try
            {
                // Hard coded parser
                Parser hardcodeParser = GetHardCodeParser(buffer.Status);

                if (buffer.Status.Error == null)
                {
                    step = 2;
                    // Use hard coded grammar to read meta grammar.
                    metaGrammarDoc = hardcodeParser.ParseString(buffer);

                    string gg = metaGrammarDoc.ToMarkup();
                    step = 3;
                }
                if (buffer.Status.Error == null)
                {
                    _instance = new Parser() { Level = 2 };
                    ParserFactory.BuildRules(_instance, metaGrammarDoc, buffer.Status);
                }
            }
            catch (Exception e)
            {
                // if an exception occurs under parsing MetaGrammar it is an HardGrammar error
                var e2 = new ParserException((step <= 2 ? HardGrammar_ : MetaGrammar_) + " " + e.Message);
                if (e is ParserException) e2.AllErrors = ((ParserException)e).AllErrors;
                throw e2;
            }
            if (buffer.Status.Error != null)
            {
                // if an proces-error occurs under parsing MetaGrammar it is an MetaGrammar error
                var e2 = new ParserException((step == 1 ? HardGrammar_ : MetaGrammar_) + " " + buffer.Status.Error.Message);
                e2.AllErrors = buffer.Status.AllErrors;
                throw e2;
            }
        }

    }
}
