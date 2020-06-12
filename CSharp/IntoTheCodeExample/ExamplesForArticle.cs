using IntoTheCode;

namespace IntoTheCodeExample
{
    class ExamplesForArticle
    {
        protected void TheVeryBasics()
        {
string grammar = ""; // Set the grammar in this string
string input = ""; // Set the input in this string
var parser = new Parser(grammar); // instantiate the parser
// parse the input and get output tree
TextDocument doc = TextDocument.Load(parser, input);

            var d = doc;
        }

        protected void ExamplesOfExpressions()
        {
            string grammar = @"
name = firstname lastname;
transport = train | bus;
customer   = customerId (person | company); 
post = letter | (packagetype package); 

";
        }

        protected void ExamplesOfSettings()
        {
            string grammar = @"
list      = {name};
name      = firstname lastname;
firstname = identifier;
lastname  = identifier;
settings
name      collapse;";
        }

    }
}
