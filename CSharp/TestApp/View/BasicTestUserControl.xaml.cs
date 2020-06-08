using IntoTheCodeExample;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace TestApp.View
{
    public partial class BasicTestUserControl : UserControl
    {

        private ExampleVievModelBase vm;

        public BasicTestUserControl()
        {
            InitializeComponent();
        }


        private void btnCodeOpen_Click(object sender, RoutedEventArgs e)
        {
            vm = DataContext as ExampleVievModelBase;
            OpenFileDialog(s => vm.Input = s);
        }

        private void btnGrammarOpen_Click(object sender, RoutedEventArgs e)
        {
            vm = DataContext as ExampleVievModelBase;
            OpenFileDialog(s => vm.Grammar = s);
        }

        /// <summary>Show FileDialog to open file.</summary>
        private void OpenFileDialog(Action<string> set) //object sender, RoutedEventArgs e)
        {
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

        #region properties for line and column

        public string CodeLine
        {
            get { return (string)GetValue(CodeLineProperty); }
            set { SetValue(CodeLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CodeLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeLineProperty =
            DependencyProperty.Register("CodeLine", typeof(string), typeof(BasicTestUserControl), new PropertyMetadata("0"));
        public string CodeColumn
        {
            get { return (string)GetValue(CodeColumnProperty); }
            set { SetValue(CodeColumnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CodeColumn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeColumnProperty =
            DependencyProperty.Register("CodeColumn", typeof(string), typeof(BasicTestUserControl), new PropertyMetadata("0"));

        public string GrammarLine
        {
            get { return (string)GetValue(GrammarLineProperty); }
            set { SetValue(GrammarLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GrammarLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GrammarLineProperty =
            DependencyProperty.Register("GrammarLine", typeof(string), typeof(BasicTestUserControl), new PropertyMetadata("0"));

        public string GrammarColumn
        {
            get { return (string)GetValue(GrammarColumnProperty); }
            set { SetValue(GrammarColumnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GrammarColumn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GrammarColumnProperty =
            DependencyProperty.Register("GrammarColumn", typeof(string), typeof(BasicTestUserControl), new PropertyMetadata("0"));

        private void GrammarPosChanged(object sender, RoutedEventArgs e)
        {
            int line;
            int column;
            if (!IsLoaded) return;
            GetLineAndColumn(sender, out line, out column);
            GrammarLine = line.ToString();
            GrammarColumn = column.ToString();
        }

        private void CodePosChanged(object sender, RoutedEventArgs e)
        {
            int line;
            int column;
            if (!IsLoaded) return;
            GetLineAndColumn(sender, out line, out column);
            CodeLine = line.ToString();
            CodeColumn = column.ToString();
        }
        private void GetLineAndColumn(object sender, out int line, out int column)

        {
            TextBox box = sender as TextBox;
            string text = box.Text;
            int pos = ((TextBox)sender).SelectionStart;
            //if (pos == NotValidPtr)
            //    pos = PointerNextChar;
            //int index = pos;
            string find = "\n";
            int nlPos = 0;
            line = 1;
            int findPos = text.IndexOf(find, nlPos, System.StringComparison.Ordinal);
            while (text.Length > nlPos && findPos > 0 && pos > findPos)
            {
                line++;
                nlPos = findPos + find.Length;
                findPos = text.IndexOf(find, nlPos, System.StringComparison.Ordinal);
            }
            // add 1; the line starts with column 1.
            column = pos - nlPos + 1;
        }

        #endregion properties for line and column
    }
}
