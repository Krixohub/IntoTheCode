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
        internal const string RuleId_____ = "ruleId";      // ruleId, Word, Symbol(bnf), Term, literal,
        internal const string Expression_ = "expression";  // 
        internal const string Element____ = "element";     // 
        internal const string Block______ = "block";       // 
        internal const string Sequence___ = "sequence";    // repetition, sequence
        internal const string Optional___ = "optional";    // 
        internal const string Parentheses = "parentheses"; // 
        internal const string Or_________ = "or";          // 
        internal const string WordName___ = "name";        // name, name, word, ident, didentifier(ebnf)
        internal const string WordString_ = "string";      // string, Token, symbol, name
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
Rule        = ruleId '=' expression ';';
expression  = element {[or] element};
element     = ruleId | symbol | block;
block       = sequence | optional | parentheses;
sequence    = '{' expression '}';
optional    = '[' expression ']';
parentheses = '(' expression ')';
or          = '|';
ruleId      = name;
symbol      = string;
settings    = 'settings' {setter};
setter      = name assignment {',' assignment} ';';
assignment  = property ['=' value];
property    = name;
value       = string;";
                return syntax;
            } 
        }
/*
property    = 'collapse' | 'div' | 'ws' | 'wsdef';
property    = name;
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
                new Sequence(new RuleLink(Rule_______)),
                new Optional(new RuleLink(Settings___))));

            // rule = ruleId '=' expression ';'
            list.Add(new Rule(Rule_______,
                new RuleLink(RuleId_____),
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

            // element    = ruleId | symbol | block; Husk ny block
            list.Add(new Rule(Element____,
                new Or(new RuleLink(RuleId_____),
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
            // ruleId     = name;
            // todo eliminate ruleIds with name or string
            list.Add(new Rule(RuleId_____,
                new WordName(WordName___)));
            // symbol       = string;
            list.Add(new Rule(WordSymbol_,
                new WordString()));

            // settings   > 'settings' {setter};
            list.Add(new Rule(Settings___,
                new WordSymbol(Settings___),
                new Sequence(
                    new RuleLink(Setter_____)))
            { Collapse = true });


            // setter     > name assignment {',' assignment} ';';
            list.Add(new Rule("setter",
                new WordName(WordName___),
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


            // property   > name;
            list.Add(new Rule(Property___,
                new WordName(WordName___)));

            // value      > string;";
            list.Add(new Rule(Value______,
                new WordString()));

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
