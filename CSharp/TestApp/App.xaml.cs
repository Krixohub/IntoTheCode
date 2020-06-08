using System;
using System.Diagnostics;
using System.Windows;

namespace TestApp
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
        //DispatcherUnhandledException += 
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException; ;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string msg = ex != null ? ex.Message : string.Empty;
            MessageBox.Show(String.Format("Fejl :{0}, \r\n{1}", e.ExceptionObject.GetType().Name, msg, "Unhandled"));
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            string msg = ex != null ? ex.Message : string.Empty;
            MessageBox.Show(String.Format("Fejl :{0}, \r\n{1}", e.Exception.GetType().Name, msg, "Unhandled"));
        }
    }
}
