using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TestApp.View;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void btnExpression_Click(object sender, RoutedEventArgs e)
        {
            ShowTab("Expression", typeof(ExpressionTab));
        }

        private void btnCsvData_Click(object sender, RoutedEventArgs e)
        {
            ShowTab("CsvData", typeof(CsvDataTab));
        }


        private void ShowTab(string name, Type controlType)
        {
            TabItem tab = Tabs.Items.OfType<TabItem>().SingleOrDefault(n => n.Header.ToString() == name);
            if (tab != null)
                tab.Focus();
            else
            {
                UserControl item = Activator.CreateInstance(controlType, new object[0]) as UserControl;
                tab = new TabItem
                {
                    Content = item,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    Header = name,
                    //Style = Application.Current.Resources["FaneStil"] as Style
                };

                Tabs.Items.Add(tab);
                Tabs.SelectedIndex = Tabs.Items.Count - 1;
            }
        }
    }
}
