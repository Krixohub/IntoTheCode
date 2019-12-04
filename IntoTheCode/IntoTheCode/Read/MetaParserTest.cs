namespace IntoTheCode.Read
{
    /// <summary>This class is for double checking hardcoded- and meta- syntax.</summary>
    internal class MetaParserTest : MetaParser
    {
        /// <summary>Never instantiate.</summary>
        private MetaParserTest()
        { }

        /// <summary>A hard coded text syntax.</summary>
        internal static string TestHardCodeSyntax =
         @"
HardSyntax  = {Rule} [settings];
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
//property    = wordname;
//property    = 'collapse' | 'milestone' | 'ws' | 'wsdef';

        /// <summary>To test changes to the meta syntax.</summary>
        internal static CodeDocument TestMetaSyntaxDoc()
        {
            CodeDocument syntax = new CodeDocument { Name = MetaSyntax_ };

            // syntax   = {rule}
            syntax.AddElement(new HardElement(Rule_______, string.Empty,
                new HardElement(Symbol_____, MetaSyntax_),
                new HardElement(Sequence___, string.Empty,
                    new HardElement(Symbol_____, Rule_______)),
                new HardElement(Optional___, string.Empty,
                    new HardElement(Symbol_____, Settings___))));

            // rule = symbol '=' expression ';'
            syntax.AddElement(new HardElement(Rule_______, string.Empty,
                new HardElement(Symbol_____, Rule_______),
                new HardElement(Symbol_____, Symbol_____),
                new HardElement(Quote______, "="),
                new HardElement(Symbol_____, Expression_),
                new HardElement(Quote______, ";")));

            // expression = element {[or] element};
            syntax.AddElement(new HardElement(Rule_______, string.Empty,
                new HardElement(Symbol_____, Expression_),
                new HardElement(Symbol_____, Element____),
                new HardElement(Sequence___, string.Empty,
                    new HardElement(Optional___, string.Empty,
                        new HardElement(Symbol_____, Or_________)),
                    new HardElement(Symbol_____, Element____))));

            // element    = symbol | quote | block; Husk ny block
            syntax.AddElement(new HardElement(Rule_______, string.Empty,
                new HardElement(Symbol_____, Element____),
                new HardElement(Symbol_____, Symbol_____),
                new HardElement(Or_________, string.Empty),
                new HardElement(Symbol_____, Quote______),
                new HardElement(Or_________, string.Empty),
                new HardElement(Symbol_____, Block______)));

            // block      = sequence | optional | parentheses);
            syntax.AddElement(new HardElement(Rule_______, string.Empty,
                new HardElement(Symbol_____, Block______),
                new HardElement(Symbol_____, Sequence___),
                new HardElement(Or_________, string.Empty),
                new HardElement(Symbol_____, Optional___),
                new HardElement(Or_________, string.Empty),
                new HardElement(Symbol_____, Parentheses)));

            // sequence      = '{' expression '}';
            syntax.AddElement(new HardElement(Rule_______, string.Empty,
                new HardElement(Symbol_____, Sequence___),
                new HardElement(Quote______, "{"),
                new HardElement(Symbol_____, Expression_),
                new HardElement(Quote______, "}")));

            // optional     = '[' expression ']';
            syntax.AddElement(new HardElement(Rule_______, string.Empty,
                new HardElement(Symbol_____, Optional___),
                new HardElement(Quote______, "["),
                new HardElement(Symbol_____, Expression_),
                new HardElement(Quote______, "]")));

            // parentheses      = '(' expression ')';
            syntax.AddElement(new HardElement(Rule_______, string.Empty,
                new HardElement(Symbol_____, Parentheses),
                new HardElement(Quote______, "("),
                new HardElement(Symbol_____, Expression_),
                new HardElement(Quote______, ")")));

            // or         = '|';
            syntax.AddElement(new HardElement(Rule_______, string.Empty,
                new HardElement(Symbol_____, Or_________),
                new HardElement(Quote______, "|")));

            // symbol      > wordname;
            syntax.AddElement(new HardElement(Rule_______, string.Empty,
                new HardElement(Symbol_____, Symbol_____),
                new HardElement(Symbol_____, WordName___)));

            // quote     = wordstring;
            syntax.AddElement(new HardElement(Rule_______, string.Empty,
                new HardElement(Symbol_____, Quote______),
                new HardElement(Symbol_____, WordString_)));

            return syntax;
        }
    }
}
