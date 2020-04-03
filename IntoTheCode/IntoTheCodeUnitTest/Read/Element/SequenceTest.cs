using System;
using System.Collections.Generic;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Read.Element
{
    [TestClass]
    public class SequenceTest
    {
        [TestMethod]
        public void ITC16Load()
        {
            string markup;
            List<ParserElementBase> elements;


            // sytax: "'Abcde' | string | identifier"
            elements = new List<ParserElementBase>() { new Sequence(new WordSymbol("a1"),
                    new WordIdent(),
                    new WordString()) };

            // Read identifier
            markup = @"<identifier>a</identifier>
<string>a2</string>
<identifier>b</identifier>
<string>b2</string>
";
            Util.ParserElementLoad("  a1 a 'a2' a1 b 'b2'  ", markup, elements);
        }
    }
}
