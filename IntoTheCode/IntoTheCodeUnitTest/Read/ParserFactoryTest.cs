using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using IntoTheCode;
using IntoTheCode.Basic;
using IntoTheCode.Buffer;
using IntoTheCode.Read;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
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
        public void BuildError()
        {

            //// pb05: Link grammar stx. Identifier stx is defined twice Line 1, colomn 1
            //Util.ParserBuildError("stx = ':'; stx = identifier;",
            //    Util.BuildMsg(1, 1, () => MessageRes.pb05, "stx", "stx"),
            //    Util.BuildMsg(1, 12, () => MessageRes.pb05, "stx", "stx"));


            // pb11: The rule 'stx' must have a non recursive path. Line 1, colomn 1
            Util.ParserBuildError("stx = sty; sty = stx;",
                Util.BuildMsg(1, 1, () => MessageRes.pb11, "stx"));

        }

        /// <summary>Test different types of grammar build error.</summary>
        [TestMethod]
        public void SyntaxErrorBuild()
        {
            string errMsg1, errMsg2;

            // Can't reproduce pb01 - pb04. Problems in the CodeDocument is catched when parsing (before build)

            // Double rule
            errMsg1 = null;
            errMsg2 = null;
            try { var parser = new Parser("stx = ':'; stx = identifier;"); }
            catch (ParserException e) { errMsg1 = e.AllErrors[0].Message; errMsg2 = e.Message; }
            Util.ParseErrorResPos("Double rule, build error", 1, 1, errMsg1, () => MessageRes.pb05, "stx", "stx");
            Util.ParseErrorResPos("Double rule, build error", 1, 12, errMsg2, () => MessageRes.pb05, "stx", "stx");

            // identifier not found
            errMsg1 = null;
            try { var parser = new Parser("stx = hans ;"); }
            catch (ParserException e) { errMsg1 = e.Message; }
            Util.ParseErrorResPos("identifier not found, build error", 1, 7, errMsg1, () => MessageRes.pb06, "hans");

            // settings identifier not found
            errMsg1 = null;
            try { var parser = new Parser("stx = ':'; settings hans collapse;"); }
            //                            "123456789012345678901
            catch (ParserException e) { errMsg1 = e.Message; }
            Util.ParseErrorResPos("settings identifier not found, build error", 1, 21, errMsg1, () => MessageRes.pb07, "hans");

            // settings prop not found
            errMsg1 = null;
            try { var parser = new Parser("stx = ':'; settings stx flip;"); }
            //                            "1234567890123456789012345
            catch (ParserException e) { errMsg1 = e.Message; }
            Util.ParseErrorResPos("settings prop not found, build error", 1, 25, errMsg1, () => MessageRes.pb08, "stx", "flip");

            // First rule cant have tag=false
            errMsg1 = null;
            try { var parser = new Parser("stx = ':'; \r\n settings stx collapse; "); }
            catch (ParserException e) { errMsg1 = e.Message; }
            Util.ParseErrorResPos("First rule cant have tag=false, build error", 1, 1, errMsg1, () => MessageRes.pb09, "stx");

            // todo find this error
            // grammar error expecting string ( 'false' )
            errMsg1 = null;
            try { var parser = new Parser("stx = ':'; \r\n settings stx collapse = false; "); }
            //                             12345678901234\12345678901234567890123456
            catch (ParserException e) { errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos(" grammar error", 2, 26, errMsg1, () => MessageRes.pe10, "value", "'", "f");
        }

        #region utillity functions

        #endregion utillity functions
    }
}
