using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.CsvData
{
    public class CsvDataVievModel : ExampleVievModelBase
    {
        private string _initialInput = @"animal,gestation,longevity
Ass,365,19
Bear,220,22
Cat,61,11
Chicken,22,7
Cow,280,11
Deer,249,13
Dog,63,11
Elephant,624,35
Fox,57,9
Goat,151,12
Groundhog,31,7
Hippopotamus,240,30
Horse,336,23
Kangaroo,35,5
Lion,108,10
Monkey,205,14
Pig,115,10
Rabbit,31,7
Sheep,151,12
Squirrel,44,8
Wolf,62,11";

        private string _initialGrammar = @"syntax     = headerline {valueline};
headerline = headerA ',' headerB ',' headerC;
headerA    = identifier;
headerB    = identifier;
headerC    = identifier;

valueline  = valueA ',' valueB ',' valueC;
valueA     = identifier;
valueB     = int;
valueC     = int;
";

        public CsvDataVievModel()
        {
            Grammar = _initialGrammar;
            Input = _initialInput;
        }

        protected override void ProcessOutput(CodeDocument doc)
        {
            // Compile expression
            string result;
            int animalCount = 0;
            try
            {
                result = "ok\r\n"; // doc.ChildNodes.OfType<CodeElement>().FirstOrDefault();
                animalCount = doc.Nodes("valueline").Count();
                result += "Number of animals: " + animalCount + "\r\n";
                
            }
            catch (Exception e)
            {
                Output = "Output cant be read.\r\n" + e.Message + "\r\n";
                return;
            }

            try
            {
                float longlivitySum = doc.Nodes("valueline").Sum(node => int.Parse(node.Nodes("valueC").FirstOrDefault()?.Value ?? "0"));
                if (animalCount > 0)
                    result += "Average longevity: " + longlivitySum / animalCount;
            }
            catch (Exception e)
            {
                result += "longevity cant be read.\r\n" + e.Message + "\r\n";
            }


            Output = "Csv data: " + result;
        }

    }
}
