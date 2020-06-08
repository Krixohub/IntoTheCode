using System;
using System.Diagnostics;
using System.Windows;

using MoehlData.Basic.Message;
using MoehlData.ClientWpf.Util;
using MoehlData.Basic.Util;

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
            InitializeBasic.SetStuf(typeof(App).Assembly, new BasicResources());
            //Post.AddResource(typeof(Basic.Message.Basic));
            GlobalsBasic.Instance.ChannelError = new MessageChannelMessageBox();
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException; ;

            //// Hvis der ikke er trust til certifikat
            //// (Fejl: The remote certificate is invalid according to the validation procedure.)
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string msg = ex != null ? ex.Message : string.Empty;
            MessageBox.Show(String.Format("Fejl :{0}, \r\n{1}", e.ExceptionObject.GetType().Name, msg, "Unhandled"));
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Post.Send(() => Res1.SomeError, e.Exception, EventLogEntryType.Error, "Dispacher");
        }
    }
}
