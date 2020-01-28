using System.Diagnostics;
using System.IO;

using IntoTheCode.Basic.Layer;
using IntoTheCode.Basic.Util;
using IntoTheCode.Basic;
using IntoTheCode;
using IntoTheCode.Buffer;
using IntoTheCode.Read;
using System.Windows;
using IntoTheCode.Read;
using MoehlData.Basic.Layer;
using MoehlData.Basic.Message;
using System;

namespace TestCodeInternal.ViewModel
{
    public class GrammarAnalyzerVm : ViewModelBase
    {
        private Parser _parser;
        private Parser _metaparser = new Parser(string.Empty);


        public GrammarAnalyzerVm()
        {
            Reset(null);

            #region commands
            ResetInput = new DelegateCommand(Reset);
            SetGrammarMoBNF = new DelegateCommand(setGrammarHardcode);
            SetGrammarEMoBNF = new DelegateCommand(i => SetGrammarInp(MetaParser.SoftMetaGrammarAndSettings));
            SetGrammarCsv = new DelegateCommand(i => SetGrammarInp(GetCSVGrammar()));

            LoadGrammar = new DelegateCommand(LoadS);
            CopyGrammar = new DelegateCommand(CopyS);
            CompareMetaDoc = new DelegateCommand(CompareMetaDocument);
            LoadText = new DelegateCommand(LoadT);
            SetTextCsv = new DelegateCommand(i => SetCustomInp(GetCSVText()));
            OpenGrammarLab = new DelegateCommand(i => GrammarLabOpen());

            #endregion commands
        }

        /// <summary>Code For Documentation.</summary>
        public void CodeForDocumentation()
        {
        string Grammar = "Grammar";
        Parser parser = null;
        try
        {
            parser = new Parser(Grammar);
        }
        catch (Exception e)
        {
            // parser is null
            MessageBox.Show("The Grammar isn't working: " + e.Message);
        }

        }


        #region text properties

        public string GrammarInp { get; set; }
        public string GrammarDoc { get; set; }
        public string GrammarOut { get; set; }
        public string CustomInp { get; set; }
        public string CustomDoc { get; set; }
        public string CustomOut { get; set; }
        public Parser Parser { get { return _parser; } set { _parser = value; RaisePropertyChanged(nameof(Parser)); } }

        private void SetGrammarInp(string s) { GrammarInp = s; RaisePropertyChanged(() => GrammarInp); }
        private void SetGrammarDoc(string s) { GrammarDoc = s; RaisePropertyChanged(() => GrammarDoc); }
        private void SetGrammarOut(string s) { GrammarOut = s; RaisePropertyChanged(() => GrammarOut); }
        private void SetCustomInp(string s) { CustomInp = s; RaisePropertyChanged(() => CustomInp); }
        private void SetCustomDoc(string s) { CustomDoc = s; RaisePropertyChanged(() => CustomDoc); }
        private void SetCustomOut(string s) { CustomOut = s; RaisePropertyChanged(() => CustomOut); }

        #endregion text properties

        #region command properties

        public DelegateCommand ResetInput { get; private set; }
        public DelegateCommand SetGrammarMoBNF { get; private set; }
        public DelegateCommand SetGrammarEMoBNF { get; private set; }
        public DelegateCommand SetGrammarCsv { get; private set; }
        public DelegateCommand LoadGrammar { get; private set; }
        public DelegateCommand CopyGrammar { get; private set; }
        public DelegateCommand CompareMetaDoc { get; private set; }
        public DelegateCommand LoadText { get; private set; }
        public DelegateCommand SetTextCsv { get; private set; }
        public DelegateCommand OpenGrammarLab { get; private set; }

        #endregion command properties


        private void Reset(CommandInformation ci)
        {
            SetGrammarInp(string.Empty);
            SetGrammarDoc(string.Empty);
            SetGrammarOut(string.Empty);
            SetCustomInp(string.Empty);
            SetCustomDoc(string.Empty);
            SetCustomOut(string.Empty);
        }

        private void GrammarLabOpen()
        {

            //GrammarEdit win2 = new GrammarEdit();
            //win2.Show();
            //this.Close();
        }

        private void LoadS(CommandInformation ci)
        {
            try
            {
                SetGrammarDoc("_metaparser.Grammar.doc.ToMarkup()");
                SetGrammarOut(_metaparser.GetGrammar());

                Parser = new Parser(GrammarInp);
                SetCustomDoc("Parser.Grammar.doc != null ? Parser.Grammar.doc.ToMarkup() : string.Empty");
                SetCustomOut(Parser.GetGrammar());
            }
            catch (System.Exception e)
            {
                var msg = new Msg(e.Message, "GrammarAnalyzer", e);
                Post.Send(msg);
                //MessageBox.Show(e.Message, "GrammarAnalyzer");
            }
        }

        private void CopyS(CommandInformation ci)
        {
            SetCustomInp(GrammarInp);
        }

        private void LoadT(CommandInformation ci)
        {
            if (Parser == null)
            { SetCustomDoc("ColorText is null");
                return;
            }

            //CodeDocument doc = CodeDocument.Load(ColorText, CustomInp);
            TextBuffer buffer = new FlatBuffer(CustomInp);
            CodeDocument doc = Parser.ParseString(buffer);

            SetCustomDoc(doc != null ? doc.ToMarkup() : string.Empty);
            SetCustomOut(string.Empty);
        }

        private void setGrammarHardcode(CommandInformation ci)
        {
            var hardcodeParser = new Parser();

            // Status er kun til opsamling af fejl
            ParserStatus status = new ParserStatus(null);
            hardcodeParser = MetaParser.GetHardCodeParser(status);

            SetGrammarInp(hardcodeParser.GetGrammar());
        }

        private void CompareMetaDocument(CommandInformation ci)
        {
            string filePath = @"D:\Temp";
            string actualFile = Path.Combine(filePath, "GrammarDocFromMeta.txt");
            string expectFile = Path.Combine(filePath, "GrammarDocReference.txt");
            string actualTags, expectTags, msg;

            // Get expected Grammar document.
            //CodeDocument metaRef = MetaParserTest.TestMetaGrammarDoc();
            CodeDocument metaRef = CodeDocument.Load(MetaParser.Instance, MetaParser.SoftMetaGrammarAndSettings);
            expectTags = metaRef.ToMarkup();

            // Get actual meta Grammar document.
            //if (Parser.MetaParser == null) _parser = new Parser("");
            TextBuffer buffer = new FlatBuffer(MetaParser.SoftMetaGrammarAndSettings);
            CodeDocument metaActual = _metaparser.ParseString(buffer);

            actualTags = metaActual.ToMarkup();
//            actualTags = _metaparser.Grammar.doc.ToMarkup();


            // Write compare result to output.
            msg = CodeDocument.CompareCode(metaActual, metaRef);
            msg = (string.IsNullOrEmpty(msg) ? "All expected tags are contained in actual document" : msg) + "\r\n";
            File.WriteAllText(expectFile, msg + expectTags);
            File.WriteAllText(actualFile, msg + actualTags);

            // Start WinMerge to compare actual and expected.
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Program Files (x86)\WinMerge\WinMergeU.exe";
            startInfo.Arguments = expectFile + " " + actualFile;
            Process.Start(startInfo);
        }

        private string GetCSVText()
        {
            return @"col1,col2,col3
main d45,hej,d4
main d22,dav,d7
sub rt,d4,""hg""
sub er,d5,""hj""";
        }

        private string GetCSVGrammar()
        {
            return @"Grammar     = headerline {valueline};
headerline = headerA ',' headerA ',' headerA;
valueline  = 'main' valueA ',' valueB ',' valueC {subLine};
subLine    = 'sub' value0 ',' value1 ',' value2;
headerA    = name;
headerB    = name;
headerC    = name;
valueA     = name;
valueB     = name;
valueC     = name;
value0     = name;
value1     = name;
value2     = '""' name '""';";
        }

    }
}
