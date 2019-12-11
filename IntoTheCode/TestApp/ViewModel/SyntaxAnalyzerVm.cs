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

namespace TestCodeInternal.ViewModel
{
    public class SyntaxAnalyzerVm : ViewModelBase
    {
        private Parser _parser;
        private Parser _metaparser = new Parser(string.Empty);


        public SyntaxAnalyzerVm()
        {
            Reset(null);

            #region commands
            ResetInput = new DelegateCommand(Reset);
            SetSyntaxMoBNF = new DelegateCommand(setSyntaxHardcode);
            SetSyntaxEMoBNF = new DelegateCommand(i => SetSyntaxInp(MetaParser.SoftMetaSyntaxAndSettings));
            SetSyntaxCsv = new DelegateCommand(i => SetSyntaxInp(GetCSVSyntax()));

            LoadSyntax = new DelegateCommand(LoadS);
            CopySyntax = new DelegateCommand(CopyS);
            CompareMetaDoc = new DelegateCommand(CompareMetaDocument);
            LoadText = new DelegateCommand(LoadT);
            SetTextCsv = new DelegateCommand(i => SetCustomInp(GetCSVText()));
            OpenSyntaxLab = new DelegateCommand(i => SyntaxLabOpen());

            #endregion commands
        }


        #region text properties

        public string SyntaxInp { get; set; }
        public string SyntaxDoc { get; set; }
        public string SyntaxOut { get; set; }
        public string CustomInp { get; set; }
        public string CustomDoc { get; set; }
        public string CustomOut { get; set; }
        public Parser Parser { get { return _parser; } set { _parser = value; RaisePropertyChanged(nameof(Parser)); } }

        private void SetSyntaxInp(string s) { SyntaxInp = s; RaisePropertyChanged(() => SyntaxInp); }
        private void SetSyntaxDoc(string s) { SyntaxDoc = s; RaisePropertyChanged(() => SyntaxDoc); }
        private void SetSyntaxOut(string s) { SyntaxOut = s; RaisePropertyChanged(() => SyntaxOut); }
        private void SetCustomInp(string s) { CustomInp = s; RaisePropertyChanged(() => CustomInp); }
        private void SetCustomDoc(string s) { CustomDoc = s; RaisePropertyChanged(() => CustomDoc); }
        private void SetCustomOut(string s) { CustomOut = s; RaisePropertyChanged(() => CustomOut); }

        #endregion text properties

        #region command properties

        public DelegateCommand ResetInput { get; private set; }
        public DelegateCommand SetSyntaxMoBNF { get; private set; }
        public DelegateCommand SetSyntaxEMoBNF { get; private set; }
        public DelegateCommand SetSyntaxCsv { get; private set; }
        public DelegateCommand LoadSyntax { get; private set; }
        public DelegateCommand CopySyntax { get; private set; }
        public DelegateCommand CompareMetaDoc { get; private set; }
        public DelegateCommand LoadText { get; private set; }
        public DelegateCommand SetTextCsv { get; private set; }
        public DelegateCommand OpenSyntaxLab { get; private set; }

        #endregion command properties


        private void Reset(CommandInformation ci)
        {
            SetSyntaxInp(string.Empty);
            SetSyntaxDoc(string.Empty);
            SetSyntaxOut(string.Empty);
            SetCustomInp(string.Empty);
            SetCustomDoc(string.Empty);
            SetCustomOut(string.Empty);
        }

        private void SyntaxLabOpen()
        {

            //SyntaxEdit win2 = new SyntaxEdit();
            //win2.Show();
            //this.Close();
        }

        private void LoadS(CommandInformation ci)
        {
            try
            {
                SetSyntaxDoc("_metaparser.Syntax.doc.ToMarkup()");
                SetSyntaxOut(_metaparser.GetSyntax());

                Parser = new Parser(SyntaxInp);
                SetCustomDoc("Parser.Syntax.doc != null ? Parser.Syntax.doc.ToMarkup() : string.Empty");
                SetCustomOut(Parser.GetSyntax());
            }
            catch (System.Exception e)
            {
                var msg = new Msg(e.Message, "SyntaxAnalyzer", e);
                Post.Send(msg);
                //MessageBox.Show(e.Message, "SyntaxAnalyzer");
            }
        }

        private void CopyS(CommandInformation ci)
        {
            SetCustomInp(SyntaxInp);
        }

        private void LoadT(CommandInformation ci)
        {
            if (Parser == null)
            { SetCustomDoc("ColorText is null");
                return;
            }

            //CodeDocument doc = CodeDocument.Load(ColorText, CustomInp);
            LoadProces reading = new LoadProces(new FlatBuffer(CustomInp));
            CodeDocument doc = Parser.ParseString(reading);

            SetCustomDoc(doc != null ? doc.ToMarkup() : string.Empty);
            SetCustomOut(string.Empty);
        }

        private void setSyntaxHardcode(CommandInformation ci)
        {
            var hardcodeParser = new Parser();
            hardcodeParser = MetaParser.GetHardCodeParser();
            //hardcodeParser.Syntax.LinkSyntax();

            SetSyntaxInp(hardcodeParser.GetSyntax());
        }

        private void CompareMetaDocument(CommandInformation ci)
        {
            string filePath = @"D:\Temp";
            string actualFile = Path.Combine(filePath, "SyntaxDocFromMeta.txt");
            string expectFile = Path.Combine(filePath, "SyntaxDocReference.txt");
            string actualTags, expectTags, msg;

            // Get expected syntax document.
            //CodeDocument metaRef = MetaParserTest.TestMetaSyntaxDoc();
            CodeDocument metaRef = CodeDocument.Load(MetaParser.Instance, MetaParser.SoftMetaSyntaxAndSettings);
            expectTags = metaRef.ToMarkup();

            // Get actual meta syntax document.
            //if (Parser.MetaParser == null) _parser = new Parser("");
            LoadProces reading = new LoadProces(new FlatBuffer(MetaParser.SoftMetaSyntaxAndSettings));
            CodeDocument metaActual = CodeDocument.Load(_metaparser, reading);

            actualTags = metaActual.ToMarkup();
//            actualTags = _metaparser.Syntax.doc.ToMarkup();


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

        private string GetCSVSyntax()
        {
            return @"syntax     = headerline {valueline};
headerline = headerA ',' headerA ',' headerA;
valueline  = 'main' valueA ',' valueB ',' valueC {subLine};
subLine    = 'sub' value0 ',' value1 ',' value2;
headerA    = wordname;
headerB    = wordname;
headerC    = wordname;
valueA     = wordname;
valueB     = wordname;
valueC     = wordname;
value0     = wordname;
value1     = wordname;
value2     = '""' wordname '""';";
        }

    }
}
