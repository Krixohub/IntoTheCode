//using ParserTest.Component;

using System.Windows;

using TestApp.Syntax;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for CompAdm.xaml
    /// </summary>
    public partial class ComponentAdministration : Window
    {

        SyntaxEdit _syntaxEdit;
        SyntaxAnalyser _syntaxAnalyser;
       // CompAdmin _compAdmin;
 
        public ComponentAdministration()
        {
            InitializeComponent();
        }

        private void BtnSyntaxEditClick(object sender, RoutedEventArgs e)
        {
            if (_syntaxEdit == null)
            {
                _syntaxEdit = new SyntaxEdit();
                _syntaxEdit.Show();
            }
            else
                _syntaxEdit.Activate();


        }
        private void BtnSyntaxAnalClick(object sender, RoutedEventArgs e)
        {
            if (_syntaxAnalyser == null)
            {
                _syntaxAnalyser = new SyntaxAnalyser();
                _syntaxAnalyser.Show();
            }
            else
                _syntaxAnalyser.Activate();
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
            //if (_syntaxEdit != null) _syntaxEdit.Close();
            if (_syntaxAnalyser != null) _syntaxAnalyser.Close();
            //if (_compAdmin != null) _compAdmin.Close();
        }
    }
}
