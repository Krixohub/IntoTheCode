﻿using System;
using System.Collections.Generic;
using IntoTheCode.Read;
using IntoTheCode.Read.Structure;
using IntoTheCode.Read.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Read.Structure
{
    [TestClass]
    public class OrTest
    {
        [TestMethod]
        public void ITC16Load()
        {
            string markup;
            List<ParserElementBase> elements;

            // sytax: "'Abcde' | string | identifier"
            elements = new List<ParserElementBase>() { new Or(new WordSymbol("Abcde"),
                    new Or(new WordString(),
                    new WordIdent())) };

            // Read identifier
            markup = "<identifier>Bcccc</identifier>\r\n";
            Util.ParserLoadElement(elements, "  Bcccc  ", markup);

            // Read string
            markup = "<string>Bcccc</string>\r\n";
            Util.ParserLoadElement(elements, "  'Bcccc'  ", markup);

            // Read symbol 'Abcde'
            markup = "";
            Util.ParserLoadElement(elements, "  Abcde  ", markup);
        }
    }
}
