using IntoTheCode;
using IntoTheCodeExample.Expression.Executers;
using System;
using System.Linq;

namespace IntoTheCodeExample.CsvData
{
    public class CsvDataVievModel : ExampleVievModelBase
    {
        private string _initialInput = @"col1,col2,col3
main d45,hej,d4
main d22,dav,d7
sub rt,d4,'hg'
sub er,d5,'hj'";

        private string _initialGrammar = @"syntax     = headerline {valueline};
headerline = headerA ',' headerA ',' headerA;
valueline  = 'main' valueA ',' valueB ',' valueC {subLine};
subLine    = 'sub' value0 ',' value1 ',' value2;
headerA    = identifier;
headerB    = identifier;
headerC    = identifier;
valueA     = identifier;
valueB     = identifier;
valueC     = identifier;
value0     = identifier;
value1     = identifier;
value2     = string;";

        public CsvDataVievModel()
        {
            Grammar = _initialGrammar;
            Input = _initialInput;
        }

        protected override void ProcessOutput(TextDocument doc)
        {
            // Compile expression
            string result;
            try
            {
                result = "fine"; // doc.ChildNodes.OfType<CodeElement>().FirstOrDefault();
            }
            catch (Exception e)
            {
                Output = "Output cant be read.\r\n" + e.Message;
                return;
            }

          
            Output = "Csv data: " + result;
        }

    }
}
