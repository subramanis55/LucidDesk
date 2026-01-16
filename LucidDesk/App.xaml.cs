using DeskBackend;
using DeskBackend.Database;
using DeskUI.DeskStructure;
using DeskUI.Log;
using DeskUI.Manager;
using DeskUI.UserControls;
using Settings;
using Settings.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DeskUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            SettingsManager.Initialize();
            SetUpCheck();
            MainWindow window = new MainWindow();
            window.Show();
           // ShutdownApp();
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

                if (SettingsManager.Settings.ApplicationMode == ApplicationMode.Online)
                {
                    DeskMessageBox.ShowMessageBox("Something Wrong Online SetUp ", "Error", UserControls.MessageBoxType.Ok);
                    LogManager.LogException("Something Wrong  Online SetUp ");
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

            if (DeskUI.MainWindow.ServerNetworkManager.isStarted) return;
            DeskUI.MainWindow.ServerNetworkManager.StartServer();

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
