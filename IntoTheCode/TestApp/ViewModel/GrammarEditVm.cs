using System.IO;
using System.Windows;

using MoehlData.Basic.Layer;
using IntoTheCode;
using IntoTheCode.Read;
using System;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace ViewModel
{
    public class GrammarEditVm : ViewModelBase
    {
        private string _codeTemp = string.Empty;
        private bool _meta = false;
        //private string _grammar;

        private Parser _metaParser;
        private Parser _codeParser;

        public GrammarEditVm()
        {
            //BnfPresenter = new TextParserPresenter();
            //TextPresenter = new TextParserPresenter();

            //Grammar = string.Empty;
            //Code = string.Empty;
            //Tree { get; set; }
            _metaParser = new Parser(string.Empty);

            #region commands
            GrammarLoadCom = new DelegateCommand(GrammarLoad);
            GrammarLoadFileCom = new DelegateCommand(GrammarLoadFile);
            CodeLoadCom = new DelegateCommand(CodeLoad);
            CodeLoadFileCom = new DelegateCommand(CodeLoadFile);

            #endregion commands

            //if (XamlUtil.IsInDesignTool()) return;
            //IntoTheCodeParser parser = new IntoTheCodeParser(string.Empty);
            //Parser = IntoTheCodeParser.BaseParser;
        }

        #region properties

        //public TextParserPresenter BnfPresenter { get; private set; }
        //public TextParserPresenter TextPresenter { get; private set; }

        public string Grammar
        {
            get { return _grammar; }
            set
            {
                if (_grammar == value) return;
                _grammar = value;
                RaisePropertyChanged(nameof(Grammar));
            }
        }
        private string _grammar;

        public string Code
        {
            get { return _code; }
            set
            {
                if (_code == value) return;
                _code = value;
                RaisePropertyChanged(nameof(Code));
            }
        }
        private string _code;

        public string Tree
        {
            get { return _tree; }
            set
            {
                if (_tree == value) return;
                _tree = value;
                RaisePropertyChanged(nameof(Tree));
            }
        }
        private string _tree;

        public bool GrammarOk
        {
            get { return _grammarOk; }
            private set
            {
                if (_grammarOk == value) return;
                _grammarOk = value;
                RaisePropertyChanged(nameof(GrammarOk));
            }
        }
        private bool _grammarOk;

        public bool CodeOk
        {
            get { return _codeOk; }
            private set
            {
                if (_codeOk == value) return;
                _codeOk = value;
                RaisePropertyChanged(nameof(CodeOk));
            }
        }
        private bool _codeOk;


        public string MetaLabel { get { return _meta ? "Code" : "Meta"; } }
        public bool Meta {
            get { return _meta ; }
            set { SetMeta(value); }
        }
        public bool NonMeta { get { return !_meta; } }

        #region command properties


        public DelegateCommand GrammarLoadCom { get; private set; }
        public DelegateCommand GrammarLoadFileCom { get; private set; }
        public DelegateCommand CodeLoadCom { get; private set; }
        public DelegateCommand CodeLoadFileCom { get; private set; }

        #endregion command properties

        #endregion properties

        /// <summary>Parser grammar.</summary>
        /// <param name="ci"></param>
        private void GrammarLoad(CommandInformation ci)
        {
            if (Meta)
            {
                try
                {
                    TextDocument doc = TextDocument.Load(_metaParser, Grammar);
                    _codeParser = new Parser(Grammar);
                    GrammarOk = true;
                    Code = _codeParser.GetGrammar();
                    Tree = doc.ToMarkup();
                }
                catch (ParserException e)
                {

                    GrammarOk = false;
                    Tree = e.Message + "\r\n\r\n" + string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
                }
                catch (Exception e)
                {

                    GrammarOk = false;
                    Tree = e.Message + "\r\n\r\n" + e.StackTrace;
                }
            return;
        }
            try
            {
                _codeParser = new Parser(Grammar);
                GrammarOk = true;
                //CodeLoad(null);
                            }
            catch (Exception e)
            {

                GrammarOk = false;
            }
        }

        /// <summary>Load the grammar file. "../TestFiles/GrammarMeta.txt".</summary>
        /// <param name="ci"></param>
        private void GrammarLoadFile(CommandInformation ci)
        {
            OpenFileDialog(s => { Grammar = s; });
        }

        /// <summary>Parser grammar.</summary>
        /// <param name="ci"></param>
        private void CodeLoad(CommandInformation ci)
        {
            try
            {
                TextDocument doc = TextDocument.Load(_codeParser, Code);
                CodeOk = true;
                Tree = doc.ToMarkup();
            }
            catch (ParserException e)
            {

                CodeOk = false;
                Tree = e.Message + "\r\n\r\n" + string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            catch (Exception e)
            {

                CodeOk = false;
                Tree = e.Message + "\r\n\r\n" + e.StackTrace;
            }
        }

        /// <summary>Load the grammar file. "../TestFiles/GrammarMeta.txt".</summary>
        /// <param name="ci"></param>
        private void CodeLoadFile(CommandInformation ci)
        {
            OpenFileDialog(s => { Code = s; });
        }

        /// <summary>Parser grammar.</summary>
        /// <param name="ci"></param>
        private void SetMeta(bool value)
        {
            if (value == _meta) return;
            _meta = value;
            Tree = string.Empty;

            if (_meta)
            {
               //_codeParser = _metaParser;
                _codeTemp = Code;
                //Code = ;
                GrammarLoad(null);
            }
            else
            {
                //Grammar = Code;
                Code = _codeTemp;
                CodeLoad(null);
            }
            RaisePropertyChanged(nameof(MetaLabel));
            RaisePropertyChanged(nameof(Meta));
            RaisePropertyChanged(nameof(NonMeta));
        }

        /// <summary>Show FileDialog to open file.</summary>
        private void OpenFileDialog(Action<string> set) //object sender, RoutedEventArgs e)
        {
            //// Set filter for file extension and default file extension 
            //dlg.DefaultExt = ".png";
            //dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                LoadFile(openFileDialog.FileName, set);
        }

        public void LoadFile(string fileName, Action<string> set)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    string filetext;
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                    using (StreamReader sr = new StreamReader(fileStream, Encoding.Default))
                        filetext = sr.ReadToEnd();

                    set(filetext);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
                MessageBox.Show("The file dosn't exists");
        }
    }
}
