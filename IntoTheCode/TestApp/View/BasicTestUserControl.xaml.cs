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


        private void btnCodeLoad_Click(object sender, RoutedEventArgs e)
        {
            vm = DataContext as ExampleVievModelBase;
            OpenFileDialog(s => vm.Input = s);
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
    }
}
