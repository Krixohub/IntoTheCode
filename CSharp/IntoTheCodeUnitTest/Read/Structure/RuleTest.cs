using System;
using System.Collections.Generic;
using IntoTheCode.Grammar;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Read.Structure
{
    [TestClass]
    public class RuleTest
    {
        [TestMethod]
        public void ITC18Load()
        {
            string markup;
            List<Rule> rules;

            //  Read a varname
            // TestIdentifier = identifier;
            rules = new List<Rule>() { new Rule("TestIdentifier", new WordIdent()) };
            markup = "<TestIdentifier>Bname</TestIdentifier>\r\n";
            Util.ParserLoadRule(rules, "  Bname  ", markup);

            // Read a TestSeries
            // TestSeries       = 'TestSeries' { identifier };
            rules = new List<Rule>() { new Rule("TestSeries", new WordSymbol("TestSeries"), new Sequence(new WordIdent())) };
            markup = @"<TestSeries>
  <identifier>jan</identifier>
  <identifier>ole</identifier>
  <identifier>Mat</identifier>
</TestSeries>
";
            Util.ParserLoadRule(rules, "  TestSeries jan ole Mat  ", markup);

            // alt            = TestIdentifier | TestString | TestSymbol;
            // TestIdentifier = identifier;
            // TestSymbol     = 'Abcde';
            // TestString     = string;
            rules = GetHardCodeRuleAlternatives();

            // Read a 'or grammar = identifier'
            markup = "<alt>\r\n  <TestIdentifier>Bcccc</TestIdentifier>\r\n</alt>\r\n";
            Util.ParserLoadRule(rules, "  Bcccc  ", markup);

            // Read a 'or TestString'
            markup = "<alt>\r\n  <string>Ccccc</string>\r\n</alt>\r\n";
            Util.ParserLoadRule(rules, " 'Ccccc'  ", markup);

            // Read a TestOption
            rules = GetHardCodeRuleTestOption();
            markup = "<TestOption>\r\n  <TestQuote2>qwerty</TestQuote2>\r\n</TestOption>\r\n";
            Util.ParserLoadRule(rules, "  TestOption 'qwerty'  ", markup);

            markup = "<TestOption/>\r\n";
            Util.ParserLoadRule(rules, "  TestOption   ", markup);

            markup = "<TestOption>\r\n  <TestIdentifier>wer</TestIdentifier>\r\n</TestOption>\r\n";
            Util.ParserLoadRule(rules, "  TestOption wer  ", markup);


            // Read: TestLines       = 'TestLines' { identifier '=' string ';' };
            rules = new List<Rule>() { new Rule("TestLines",
                new WordSymbol("TestLines"),
                new Sequence(new WordIdent(), new WordSymbol("="), new WordString(), new WordSymbol(";"))) };
            markup = @"<TestLines>
  <identifier>name</identifier>
  <string>Oscar</string>
  <identifier>addr</identifier>
  <string>GoRoad</string>
  <identifier>mobile</identifier>
  <string>555 55</string>
</TestLines>
";
            Util.ParserLoadRule(rules, "  TestLines name = 'Oscar'; addr = 'GoRoad'; \r\n mobile = '555 55'; ", markup);
        }

        #region utillity functions

        /// <summary>A grammar for test. Hard coded.</summary>
        private List<Rule> GetHardCodeRuleAlternatives()
        {
            //List<RuleLink> symbolsToResolve = new List<RuleLink>();
            List<Rule> list = new List<Rule>();
            list.Add(new Rule("alt",
                    new Or(new RuleLink("TestIdentifier"),
                    new Or(new RuleLink("TestString"),
                    new RuleLink("TestSymbol")))) /*{ Tag = true }*/);
            // TestIdentifier     = varName;
            list.Add(new Rule("TestIdentifier",
                new WordIdent()) /*{ Tag = true }*/);
            // TestSymbol       = 'Abcde';
            list.Add(new Rule("TestSymbol",
                new WordSymbol("Abcde"))
            { Collapse = true });

            // TestString       = Quote;
            list.Add(new Rule("TestString",
                new WordString())
            { Collapse = true });

            return list;
        }

        /// <summary>A grammar for test. Hard coded.</summary>
        private List<Rule> GetHardCodeRuleTestOption()
        {
            //List<RuleLink> symbolsToResolve = new List<RuleLink>();
            List<Rule> rules = new List<Rule>();
            rules.Add(new Rule("TestOption", new WordSymbol("TestOption"),
                new Optional(new RuleLink("TestIdentifier")),
                new Optional(new RuleLink("TestQuote2"))));

            // TestIdentifier     = identifier;
            rules.Add(new Rule("TestIdentifier", new WordIdent()) /*{ Tag = true }*/);
            // TestQuote2      = string;
            rules.Add(new Rule("TestQuote2", new WordString()));

            return rules;
        }

        #endregion utillity functions
    }
}
