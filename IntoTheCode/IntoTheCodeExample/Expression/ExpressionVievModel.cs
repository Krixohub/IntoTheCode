using IntoTheCode;
using IntoTheCode.Basic.Layer;
using IntoTheCode.Read;
using IntoTheCodeExample.Expression.Executers;

namespace IntoTheCodeExample.Expression
{
    public class ExpressionVievModel : NotifyChanges
    {
        private Parser _parser;
        /*

        private string _grammar = @"expr   = mul | sum | div | sub | int;
mul    = exp '*' exp;
div    = exp '/' exp;
sum    = exp '+' exp;
sub    = exp '-' exp;";*/

        private string _grammar = @"
exp   = mul | sum | div | sub | number;
mul    = exp '*' exp;
div    = exp '/' exp;
sum    = exp '+' exp;
sub    = exp '-' exp;
number = int;

settings
mul Precedence = '2';
div Precedence = '2';
sum Precedence = '1';
sub Precedence = '1';";


        public ExpressionVievModel ()
        {
            try
            {
                _parser = new Parser(_grammar);
            }
            catch (ParserException e)
            {
                Output = e.Message;
            }
            Input = "1 + 2";
        }

        #region properties

        public string Input
        {
            get { return _input; }
            set
            {
                if (_input == value) return;
                _input = value;
                BuildExpression();
                RaisePropertyChanged(nameof(Input));
            }
        }
        private string _input;

        public string Output
        {
            get { return _output; }
            private set
            {
                if (_output == value) return;
                _output = value;
                RaisePropertyChanged(nameof(Output));
            }
        }
        private string _output;

        public string Markup
        {
            get { return _markup; }
            private set
            {
                if (_markup == value) return;
                _markup = value;
                RaisePropertyChanged(nameof(Markup));
            }
        }
        private string _markup;

        #endregion properties

        private void BuildExpression()
        {
            if (_parser == null)
            {
                Markup = "Grammar is not valid";
                //Output = "Grammar is not valid";
                return;
            }

            // Load input
            CodeDocument doc = null;
            try
            {
                doc = CodeDocument.Load(_parser, _input) ;
            }
            catch (ParserException e)
            {
                Output = e.Message;
                Markup = "Input is not valid";
                return;
            }

            // Convert to markup
            Markup = doc.ToMarkup();

            // Compile expression
            ExpressionBase expression;
            try
            {
                expression = ExpressionBase.CreateExpression(doc.SubElements[0]);
            }
            catch (ParserException e)
            {
                Output = "Expression does not compile. \r\n" + e.Message;
                return;
            }

            // execute expression
            float result;
            try
            {
                result = expression.execute();
            }
            catch (ParserException e)
            {
                Output = "Expression cant execute.\r\n" + e.Message;
                return;
            }

            Output = "Expression result: " + result;
        }
    }
}
