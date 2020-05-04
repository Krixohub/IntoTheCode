using IntoTheCode;
using IntoTheCode.Basic.Layer;
using IntoTheCode.Read;

namespace IntoTheCodeExample
{
    public abstract class ExampleVievModelBase : NotifyChanges
    {
        private Parser _parser;

        #region properties

        public string Input
        {
            get { return _input; }
            set
            {
                if (_input == value) return;
                _input = value;
                ParseInput();
                RaisePropertyChanged(nameof(Input));
            }
        }
        private string _input;

        public string Output
        {
            get { return _output; }
            protected set
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
        private string _grammar;

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

        protected abstract void ProcessOutput(TextDocument doc);

        private void ParseInput()
        {
            if (_parser == null)
            {
                Markup = "Grammar is not valid";
                Output = "";
                return;
            }

            // Load input
            TextDocument doc = null;
            try
            {
                doc = TextDocument.Load(_parser, _input);
            }
            catch (ParserException e)
            {
                Output = e.Message;
                Markup = "Input is not valid";
                return;
            }

            // Convert to markup
            Markup = doc.ToMarkup();
            ProcessOutput(doc);
        }

        private void BuildParser()
        {
            _parser = null;
            try
            {
                _parser = new Parser(_grammar);
                GrammarOutput = "Grammar is ok" + "\r\n\r\n" + _parser.GetGrammar();
                ParseInput();
            }
            catch (ParserException e)
            {
                GrammarOutput = e.Message;
            }

            try
            {
                TextDocument doc = TextDocument.Load(new Parser(string.Empty), _grammar);
                GrammarMarkup = doc.ToMarkup();
            }
            catch (ParserException e)
            {
                GrammarMarkup = e.Message;
            }
        }
    }
}
