using System.IO;
using System.Windows;

using MoehlData.Basic.Layer;
using IntoTheCode;
using IntoTheCode.Read;
using System;
using System.Linq;

namespace ViewModel
{
    public class SyntaxEditVm : ViewModelBase
    {
        private string _codeTemp = string.Empty;
        private bool _meta = false;
        //private string _grammar;

        private Parser _metaParser;
        private Parser _codeParser;

        public SyntaxEditVm()
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

        #region command properties

        
        public DelegateCommand GrammarLoadCom { get; private set; }
        public DelegateCommand GrammarLoadFileCom { get; private set; }
        public DelegateCommand CodeLoadCom { get; private set; }
        public DelegateCommand CodeLoadFileCom { get; private set; }

        #endregion command properties

        #endregion properties

        /// <summary>Parser syntax.</summary>
        /// <param name="ci"></param>
        private void GrammarLoad(CommandInformation ci)
        {
            if (Meta) return;
            try
            {
                _codeParser = new Parser(Grammar);
                GrammarOk = true;
                CodeLoad(null);
                            }
            catch (Exception e)
            {

                GrammarOk = false;
            }
        }

        /// <summary>Load the syntax file. "../TestFiles/SyntaxMeta.txt".</summary>
        /// <param name="ci"></param>
        private void GrammarLoadFile(CommandInformation ci)
        {
            //string path = Application.Current.StartupUri.AbsolutePath;
            //string path1 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //DirectoryInfo dir = new DirectoryInfo(path1).Parent.Parent.Parent.Parent;
            ////string filename = Path.Combine(dir.FullName, "TestFiles", "SyntaxTest.txt");
            //string filename = Path.Combine(dir.FullName, "TestFiles", "SyntaxMeta.txt");
            //BnfPresenter.LoadFile(filename);
        }

        /// <summary>Parser syntax.</summary>
        /// <param name="ci"></param>
        private void CodeLoad(CommandInformation ci)
        {
            try
            {
                CodeDocument doc = CodeDocument.Load(_codeParser, Code);
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

        /// <summary>Load the syntax file. "../TestFiles/SyntaxMeta.txt".</summary>
        /// <param name="ci"></param>
        private void CodeLoadFile(CommandInformation ci)
        {
            //string path = Application.Current.StartupUri.AbsolutePath;
            //string path1 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //DirectoryInfo dir = new DirectoryInfo(path1).Parent.Parent.Parent.Parent;
            ////string filename = Path.Combine(dir.FullName, "TestFiles", "SyntaxTest.txt");
            //string filename = Path.Combine(dir.FullName, "TestFiles", "SyntaxMeta.txt");
            //BnfPresenter.LoadFile(filename);
        }

        /// <summary>Parser syntax.</summary>
        /// <param name="ci"></param>
        private void SetMeta(bool value)
        {
            if (value == _meta) return;
            _meta = value;
            if (_meta)
            {
                _codeParser = _metaParser;
                _codeTemp = Code;
                Code = Grammar;
                CodeLoad(null);
            }
            else
            {
                Grammar = Code;
                Code = _codeTemp;
                GrammarLoad(null);
            }
            RaisePropertyChanged(nameof(MetaLabel));
            RaisePropertyChanged(nameof(Meta)); 
        }

    }
}
