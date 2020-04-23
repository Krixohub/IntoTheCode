using IntoTheCode;
using IntoTheCode.Basic.Layer;
using IntoTheCode.Read;
using IntoTheCodeExample.Expression.Executers;
using System;
using System.Linq;

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

        private string _grammar = @"input = [exp];
exp   = mul | sum | div | sub | par | number;
mul    = exp '*' exp;
div    = exp '/' exp;
sum    = exp '+' exp;
sub    = exp '-' exp;
number = int;
par    = '(' exp ')';

settings
mul Precedence = '2';
div Precedence = '2';
sum Precedence = '1';
sub Precedence = '1';
exp collapse;
par collapse;";

        public ExpressionVievModel ()
        {
            BuildParser();
            Input = "1 + 2 * (4 - 1)";
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






        public string Grammar
        {
            get { return _grammar; }
            set
            {
                if (_grammar == value) return;
                _grammar = value;
                BuildParser();
                RaisePropertyChanged(nameof(Grammar));
            }
        }
        //private string _grammar;

        public string GrammarOutput
        {
            get { return _grammarOutput; }
            private set
            {
                if (_grammarOutput == value) return;
                _grammarOutput = value;
                RaisePropertyChanged(nameof(GrammarOutput));
            }
        }
        private string _grammarOutput;

        public string GrammarMarkup
        {
            get { return _grammarMarkup; }
            private set
            {
                if (_grammarMarkup == value) return;
                _grammarMarkup = value;
                RaisePropertyChanged(nameof(GrammarMarkup));
            }
        }
        private string _grammarMarkup;




        #endregion properties

        private void BuildParser()
        {
            try
            {
                _parser = new Parser(_grammar);
                GrammarOutput = "Grammar is ok" + "\r\n\r\n" + _parser.GetGrammar();
                BuildExpression();
            }
            catch (ParserException e)
            {
                GrammarOutput = e.Message;
            }

            try
            {
                TextDocument doc = TextDocument.Load(new Parser(string.Empty) , _grammar);
                GrammarMarkup = doc.ToMarkup();
            }
            catch (ParserException e)
            {
                GrammarMarkup = e.Message;
            }
        }

        private void BuildExpression()
        {
            if (_parser == null)
            {
                Markup = "Grammar is not valid";
                //Output = "Grammar is not valid";
                return;
            }

            // Load input
            TextDocument doc = null;
            try
            {
                doc = TextDocument.Load(_parser, _input) ;
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
                expression = ExpressionBase.Factory(doc.SubElements.OfType<CodeElement>().FirstOrDefault());
            }
            catch (Exception e)
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
