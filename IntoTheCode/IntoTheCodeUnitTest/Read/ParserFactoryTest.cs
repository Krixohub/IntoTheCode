using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using IntoTheCode;
using IntoTheCode.Basic;
using IntoTheCode.Buffer;
using IntoTheCode.Read;
using IntoTheCode.Read.Structure;
using IntoTheCode.Read.Words;
using IntoTheCode.Message;
using System.Linq.Expressions;
using IntoTheCode.Basic.Util;
using IntoTheCodeUnitTest.Read;

namespace Read
{
    [TestClass]
    public class ParserFactoryTest
    {

        /// <summary>Test different types of grammar build error.</summary>
        [TestMethod]
        public void ITC21BuildError()
        {
            string grammar;
            // Can't reproduce pb01 - pb04. Problems in the CodeDocument is catched when parsing (before build)

            // pb05: Link grammar stx. Identifier stx is defined twice Line 1, colomn 1
            Util.ParserLoad("stx = ':'; stx = identifier;", string.Empty, string.Empty,
                Util.BuildMsg(1, 12, () => MessageRes.pb05, "stx", "stx"),
                Util.BuildMsg(1, 1, () => MessageRes.pb05, "stx", "stx"));

            // pb06: Identifier 'hans' not found in Grammar Line 1, colomn 7
            Util.ParserLoad("stx = hans ;", string.Empty, string.Empty,
                Util.BuildMsg(1, 7, () => MessageRes.pb06, "hans"));

            // pb07: Settings: Identifier 'hans' cant be resolved Line 1, colomn 21
            //                    "123456789012345678901
            Util.ParserLoad("stx = ':'; settings hans collapse;", string.Empty, string.Empty,
                Util.BuildMsg(1, 21, () => MessageRes.pb07, "hans"));

            // pb08: Settings: Identifier 'stx' Property 'flip' cant be resolved Line 1, colomn 25
            //                    "1234567890123456789012345
            Util.ParserLoad("stx = ':'; settings stx flip;", string.Empty, string.Empty,
                Util.BuildMsg(1, 25, () => MessageRes.pb08, "stx", "flip"));

            // pb09: First rule 'stx' must have Collapse=false Line 1, colomn 1
            Util.ParserLoad("stx = ':'; \r\n settings stx collapse; ", string.Empty, string.Empty,
                Util.BuildMsg(1, 1, () => MessageRes.pb09, "stx"));

            // itc10: Syntax error (value). Expecting ', found f. Line 2, colomn 26
            //                    "12345678901234\12345678901234567890123456
            Util.ParserLoad("stx = ':'; \r\n settings stx collapse = false; ", string.Empty, string.Empty,
                Util.BuildMsg(2, 26, () => MessageRes.itc10, "value", "'", "f"));

            // pb11: The rule 'sty' must have a non recursive path. Line 1, colomn 12
            Util.ParserLoad("stx = sty; sty = stx;", string.Empty, string.Empty,
                Util.BuildMsg(1, 12, () => MessageRes.pb11, "sty"));

            // no pb11 error
            Util.ParserLoad("aa = ab; ab = aa | identifier;", string.Empty, string.Empty);
            Util.ParserLoad("aa = ab; ab = identifier | aa;", string.Empty, string.Empty);

            // no error
            grammar = @"
aa = ab;
ab = aa | ba;
ba = ba | ca;
ca = cb;
cb = ca | identifier ;";
            Util.ParserLoad(grammar, string.Empty, string.Empty);

            // pb11: The rule 'cb' must have a non recursive path. Line 6, colomn 1
            grammar = @"
aa = ab;
ab = aa | ba;
ba = ba | ca;
ca = cb;
cb = ca ;";
            Util.ParserLoad(grammar, string.Empty, string.Empty,
                Util.BuildMsg(6, 1, () => MessageRes.pb11, "cb"));


        }

        #region utillity functions

        #endregion utillity functions
    }
}
