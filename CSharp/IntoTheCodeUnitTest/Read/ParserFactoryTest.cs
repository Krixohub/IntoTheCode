
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntoTheCode.Message;
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
            // Can't reproduce pb01 - itc31. Problems in the CodeDocument is catched when parsing (before build)

            // itc23: The rule stx is defined twice in grammar stx. Line 1, colomn 1
            Util.ParserLoad("stx = ':'; stx = identifier;", string.Empty, string.Empty,
                Util.BuildMsg(1, 12, () => MessageRes.itc23, "stx", "stx"),
                Util.BuildMsg(1, 1, () => MessageRes.itc23, "stx", "stx"));

            // itc24: Identifier 'hans' not found in Grammar Line 1, colomn 7
            Util.ParserLoad("stx = hans ;", string.Empty, string.Empty,
                Util.BuildMsg(1, 7, () => MessageRes.itc24, "hans"));

            // itc26: Settings: Identifier 'hans' cant be resolved Line 1, colomn 21
            //              "123456789012345678901
            Util.ParserLoad("stx = ':'; settings hans collapse;", string.Empty, string.Empty,
                Util.BuildMsg(1, 21, () => MessageRes.itc26, "hans"));

            // itc27: Settings: Identifier 'stx' Property 'flip' cant be resolved Line 1, colomn 25
            //              "1234567890123456789012345
            Util.ParserLoad("stx = ':'; settings stx flip;", string.Empty, string.Empty,
                Util.BuildMsg(1, 25, () => MessageRes.itc27, "stx", "flip"));

            // itc21: First rule 'stx' must have Collapse=false Line 1, colomn 1
            Util.ParserLoad("stx = ':'; \r\n settings stx collapse; ", string.Empty, string.Empty,
                Util.BuildMsg(1, 1, () => MessageRes.itc21, "stx"));

            // itc10: Syntax error (value). Expecting ', found f. Line 2, colomn 26
            //                    "12345678901234\12345678901234567890123456
            Util.ParserLoad("stx = ':'; \r\n settings stx collapse = false; ", string.Empty, string.Empty,
                Util.BuildMsg(2, 26, () => MessageRes.itc10, "value", "'", "f"));

            // itc25: The rule 'sty' must have a non recursive path. Line 1, colomn 12
            Util.ParserLoad("stx = sty; sty = stx;", string.Empty, string.Empty,
                Util.BuildMsg(1, 12, () => MessageRes.itc25, "sty"));

            // no itc25 error
            Util.ParserLoad("aa = ab; ab = int aa | identifier;", string.Empty, string.Empty);
            Util.ParserLoad("aa = ab; ab = identifier | int aa;", string.Empty, string.Empty);

            // no error
            grammar = @"
aa = ab;
ab = int aa | ba;
ba = int ba | ca;
ca = cb;
cb = int ca | identifier ;";
            Util.ParserLoad(grammar, string.Empty, string.Empty);

            // itc25: The rule 'cb' must have a non recursive path. Line 6, colomn 1
            grammar = @"
aa = ab;
ab = int aa | ba;
ba = int ba | ca;
ca = cb;
cb = int ca ;";
            Util.ParserLoad(grammar, string.Empty, string.Empty,
                Util.BuildMsg(6, 1, () => MessageRes.itc25, "cb"));

            // itc28: The rule 'a' must have a mandatory word in recursive path Line 1, colomn 1
            Util.ParserLoad("a = b; b = a | identifier;", string.Empty, string.Empty,
                Util.BuildMsg(1, 1, () => MessageRes.itc28, "a"));

            // itc28: The rule 'a' must have a mandatory word in recursive path Line 1, colomn 1
            Util.ParserLoad("a = b; b = int | a;", string.Empty, string.Empty,
                Util.BuildMsg(1, 1, () => MessageRes.itc28, "a"));

            // itc28: The rule 'a' must have a mandatory word in recursive path Line 1, colomn 1
            Util.ParserLoad("a = b | int; b = [int] c; c= {int} a;", string.Empty, string.Empty,
                Util.BuildMsg(1, 1, () => MessageRes.itc28, "a"));
        }

        #region utillity functions

        #endregion utillity functions
    }
}
