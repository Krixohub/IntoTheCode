using System.Collections.Generic;

using IntoTheCode.Read.Element;
using IntoTheCode.Message;
using System;
using IntoTheCode.Read;
using IntoTheCode.Buffer;
using IntoTheCode.Read.Element.Words;

namespace IntoTheCode.Read
{
    /// <summary>The meta-syntax for proces syntax definitions.
    /// The Instance property is a singleton parser of syntaxes.</summary>
    internal class MetaParser //: Syntax
    {
        // Make static meta parser and syntax thread safe.
        private static object _syncMetaParser = new object();

        private static Parser _instance;

        /// <summary>A linked syntax for MoBNF.</summary>
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
        internal const string Syntax = "Syntax";           // Grammer
        internal const string MetaSyntax_ = "MetaSyntax";  // Grammer
        internal const string HardSyntax_ = "HardSyntax";  // Grammer
        internal const string Rule_______ = "Rule";        // Equation , Statement, Action, Definition, Assignment,
        internal const string Expression_ = "expression";  // 
        internal const string Element____ = "element";     // 
        internal const string Block______ = "block";       // 
        internal const string Sequence___ = "sequence";    // repetition, sequence
        internal const string Optional___ = "optional";    // 
        internal const string Parentheses = "parentheses"; // 
        internal const string Operator___ = "operator";    // 
        internal const string Or_________ = "or";          // 
        internal const string Equal______ = "equal";       // 
        internal const string Symbol_____ = "symbol";      // ruleId, identifier(ebnf), Word, Symbol(bnf), Term, literal,
        internal const string Quote______ = "quote";       // wordsymbol
        internal const string WordName___ = "wordname";    // word
        internal const string WordString_ = "wordstring";  // , Token, symbol, id
        internal const string Settings___ = "settings";    // 
        internal const string Setter_____ = "setter";      // 
        internal const string Assignment_ = "assignment";  // 
        internal const string Property___ = "property";    // 
        internal const string Value______ = "value";       // 
        //internal const string HardCode = "hc_";          // 
        //internal const string Tag________ = "tag";         // 
        internal const string Collapse___ = "collapse";    // 
        internal const string Div________ = "div";   // 
        internal const string Comment____ = "comment";     // 
        internal const string Whitespace_ = "ws";          // 
        internal const string WsDef______ = "wsdef";       // 

        #endregion element const names

        /// <summary>The meta syntax for proces other syntaxes.</summary>
        internal static string SoftMetaSyntaxAndSettings { get { return MetaSyntax + MetaSettings; } }

        /// <summary>The meta syntax for proces other syntaxes.</summary>
        internal static string MetaSyntax
        {
            get
            {
                string syntax = @"MetaSyntax  = {Rule} [settings];
Rule        = symbol '=' expression ';';
expression  = element {[or] element};
element     = symbol | quote | block;
block       = sequence | optional | parentheses;
sequence    = '{' expression '}';
optional    = '[' expression ']';
parentheses = '(' expression ')';
or          = '|';
symbol      = wordname;
quote       = wordstring;
settings    = 'settings' {setter};
setter      = wordname assignment {',' assignment} ';';
assignment  = property ['=' value];
property    = wordname;
value       = wordstring;";
                return syntax;
            } 
        }
/*
property    = 'collapse' | 'div' | 'ws' | 'wsdef';
property    = wordname;
 */ 
        /// <summary>The shapin syntax.</summary>
        internal static string MetaSettings
        {
            get
            {
                //    return "";
                string syntax = @"
settings
expression collapse;
element    collapse;
block      collapse;
settings   collapse;";
                return syntax;
            }
        }

        internal static Parser GetHardCodeParser()
        { 
            List<Rule> list = new List<Rule>();

            // Build Mo Backus Naur Form in code.
            // syntax   = {rule} [settings];
            list.Add(new Rule(HardSyntax_,
                new Sequence(new Symbol(Rule_______)),
                new Optional(new Symbol(Settings___))));

            // rule = symbol '=' expression ';'
            list.Add(new Rule(Rule_______,
                new Symbol(Symbol_____),
                new Quote("="),
                new Symbol(Expression_),
                new Quote(";")));

            // expression = element {[or] element};
            list.Add(new Rule(Expression_,
                new Symbol(Element____),
                new Sequence(new Optional(new Symbol(Or_________)), new Symbol(Element____)))
            { Collapse = true });

            // element    = symbol | quote | block; Husk ny block
            list.Add(new Rule(Element____,
                new Or(new Symbol(Symbol_____),
                new Or(new Symbol(Quote______),
                new Symbol(Block______))))
            { Collapse = true });

            // block      = sequence | optional | parentheses;
            list.Add(new Rule(Block______,
                new Or(new Symbol(Sequence___),
                new Or(new Symbol(Optional___),
                new Symbol(Parentheses))))
            { Collapse = true });

            // sequence     = '{' expression '}';
            list.Add(new Rule(Sequence___,
                new Quote("{"),
                new Symbol(Expression_),
                new Quote("}")));
            // optional     = '[' expression ']';
            list.Add(new Rule(Optional___,
                new Quote("["),
                new Symbol(Expression_),
                new Quote("]")));
            // parentheses      = '(' expression ')';
            list.Add(new Rule(Parentheses,
                new Quote("("),
                new Symbol(Expression_),
                new Quote(")")));
            // or         = '|';
            list.Add(new Rule(Or_________,
                new Quote("|")));
            // symbol     = wordname;
            // todo eliminate symbols with wordname or wordstring
            list.Add(new Rule(Symbol_____,
                new Symbol(WordName___) { SymbolElement = new WordName(WordName___) }));
            // quote       = wordstring;
            list.Add(new Rule(Quote______,
                new Symbol(WordString_) { SymbolElement = new WordString() }));

            // settings   > 'settings' {setter};
            list.Add(new Rule(Settings___,
                new Quote(Settings___),
                new Sequence(
                    new Symbol(Setter_____)))
            { Collapse = true });


            // setter     > wordname assignment {',' assignment} ';';
            list.Add(new Rule("setter",
                new Symbol(WordName___) { SymbolElement = new WordName(WordName___) },
                new Symbol(Assignment_),
                new Sequence(
                    new Quote(","),
                    new Symbol(Assignment_)),
                new Quote(";")));

            // assignment > property '=' value;
            list.Add(new Rule(Assignment_,
                new Symbol(Property___),
                new Optional(
                    new Quote("="),
                    new Symbol(Value______))));


            // property   > wordname;
            list.Add(new Rule(Property___,
                new Symbol(WordName___) { SymbolElement = new WordName(WordName___) }));

            // value      > string;";
            list.Add(new Rule(Value______,
                new Symbol(WordString_) { SymbolElement = new WordString() }));

            Parser parser = new Parser() { Level = 1 }; // { Name = HardSyntax_ };
            parser.Rules = list;
            foreach (var eq in list) eq.Parser = parser;
            ParserFactory.InitializeSyntax(parser);

            return parser;
        }

        /// <summary>Create the parser to read syntax definition</summary>
        /// <exclude/>
        private static void CreateMetaParser()
        {
            if (_instance != null) return;

            try
            {
                // Hard coded parser
                var hardcodeParser = GetHardCodeParser();

                // Use hard coded syntax to read meta syntax.
                LoadProces proces = new LoadProces(new FlatBuffer(MetaParser.SoftMetaSyntaxAndSettings));
                CodeDocument metaSyntaxDoc = CodeDocument.Load(hardcodeParser, proces);
                if (metaSyntaxDoc == null)
                    throw new Exception(MessageRes.DevelOnly + hardcodeParser.Name + ". " + proces.LoadError);

                string gg = metaSyntaxDoc.ToMarkup();

                _instance = new Parser() { Level = 2 };
                try
                {
                    ParserFactory.BuildRules(_instance, metaSyntaxDoc);
                }
                catch (Exception e)
                {
                    _instance.DefinitionError = MessageRes.DevelOnly + metaSyntaxDoc.Name + ". " + e.Message;
                    //_metaParser.DefinitionError = MessageRes.DevelOnly + _metaParser.Syntax.Name + ". " + e.Message;
                    //throw new Exception(_metaSyntax.DefinitionError);
                }
            }
            catch (Exception e)
            {
                //DefinitionError = e.Message;
                _instance.DefinitionError = e.Message;
                throw new Exception(MessageRes.DevelOnly + e.Message);
            }
        }

    }
}
