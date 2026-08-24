using LucidDesk.DS.Classes;
using LucidDesk.Log;
using LucidDesk.Manager;
using LucidDesk.Manager.Connection;
using LucidDesk.Manager.Database;
using LucidDesk.UserControls;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LucidDesk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CloseOtherProcess();
            this.DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
            SetUpCheck();
            MainWindow window = new MainWindow();
            window.Show();

        }

        private void CloseOtherProcess()
        {
            var current = Process.GetCurrentProcess();

            var others = Process.GetProcessesByName(current.ProcessName)
                                .Where(p => p.Id != current.Id);
            foreach (var process in others)
            {
                try
                {
                    process.CloseMainWindow();
                    if (!process.WaitForExit(3000))
                    {
                        process.Kill();
                    }
                }
                catch
                {
                }
            }
        }

        private void SetUpCheck()
        {
            LocalDatabaseManager.SetUp();
            DeskProfileManager.Intialize();
            string deskMacAddress = SystemInformationManager.GetMacAddress();
            if (deskMacAddress == null)
            {
                DeskMessageBox.ShowMessageBox("Something Wrong SetUp ", "Error", UserControls.MessageBoxType.Ok);
                LogManager.LogException("Something Wrong SetUp ");
                ShutdownApp();
            }
            else
            {
                if (!DeskProfileManager.DeskExits(SystemInformationManager.MacAddress))
                {
                    loginWindow loginWindow = new loginWindow();
                    loginWindow.OnClickNext += LoginWindowOnClickNext;
                    loginWindow.ShowDialog();
                }
                StartNetWorkServerConnection();
            }
        }
        private void LoginWindowOnClickNext(object sender, Desk desk)
        {
            DeskProfileManager.CreateDeskProfiledata(desk);
            ((Window)sender).Hide();
        }

        private void StartNetWorkServerConnection()
        {

            if (ConnectionManager.IsSeverStarted) return;
            ConnectionManager.StartConnectionServer();

        }
        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogManager.LogException(e.Exception.ToString());
            e.Handled = true;     // prevent crash
            ShutdownApp();
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogManager.LogException(e.ExceptionObject.ToString());
            ShutdownApp();
        }

        private void TaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogManager.LogException(e.Exception.ToString());
            e.SetObserved();
            ShutdownApp();
        }

        private void ShutdownApp()
        {
            // Close windows gracefully
            Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });
        }

    }
}
