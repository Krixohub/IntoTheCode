using IntoTheCode;
using IntoTheCode.Basic.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCodeExample.Blank
{
    public class BlankVievModel : ExampleVievModelBase
    {
        private string _initialInput = @"a";

        private string _initialGrammar = @"a = 'a';";

        public BlankVievModel()
        {
            Grammar = _initialGrammar;
            Input = _initialInput;
        }

        protected override void ProcessOutput(CodeDocument doc)
        {
            // Compile expression
            string result;
            try
            {
                result = doc.Name + " is ok\r\n";
                var names = new List<string>();
                int pad = doc.ChildNodes.Count() > 0 ? doc.ChildNodes.Max(n => n.Name.Length) + 2 : 0;
                IEnumerable<CodeElement> elements = doc.ChildNodes.OfType<CodeElement>();
                foreach (CodeElement elem in elements)
                    if (!names.Contains(elem.Name))
                    {
                        result += "Number of " + (elem.Name + ":").PadRight(pad) + elements.Count(e => e.Name == elem.Name) + "\r\n";
                        names.Add(elem.Name);
                    }

                int i = doc.ChildNodes.OfType<CommentElement>().Count();
                if (i > 0)
                    result += "Number of " + "comment:".PadRight(pad) + i + "\r\n";
            }
            catch (Exception e)
            {
                result = "Output cant be read.\r\n" + e.Message + "\r\n";
            }

            Output = result;
        }

    }
}
