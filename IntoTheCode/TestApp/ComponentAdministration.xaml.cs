//using ParserTest.Component;

using System.Windows;

using TestApp.Grammar;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for CompAdm.xaml
    /// </summary>
    public partial class ComponentAdministration : Window
    {

        GrammarEdit _grammarEdit;
        GrammarAnalyser _grammarAnalyser;
       // CompAdmin _compAdmin;
 
        public ComponentAdministration()
        {
            InitializeComponent();
        }

        private void BtnGrammarEditClick(object sender, RoutedEventArgs e)
        {
            //if (_grammarEdit == null)
            //{
            //    _grammarEdit = new GrammarEdit();
            //    _grammarEdit.Show();
            //}
            //else
            //    _grammarEdit.Activate();


        }
        private void BtnGrammarAnalClick(object sender, RoutedEventArgs e)
        {
            if (_grammarAnalyser == null)
            {
                _grammarAnalyser = new GrammarAnalyser();
                _grammarAnalyser.Show();
            }
            else
                _grammarAnalyser.Activate();
        }

        private void BtnCompSvcClick(object sender, RoutedEventArgs e)
        {
            //if (_compAdmin == null)
            //{
            //    _compAdmin = new CompAdmin();
            //    _compAdmin.Show();
            //}
            //else
            //    _compAdmin.Activate();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (_grammarEdit != null) _grammarEdit.Close();
            if (_grammarAnalyser != null) _grammarAnalyser.Close();
            //if (_compAdmin != null) _compAdmin.Close();
        }
    }
}
