using LucidDesk.Log;
using LucidDesk.Manager;
using LucidDesk.Manager.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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
            this.DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
            SetUpCheck();
            MainWindow window = new MainWindow();
            window.Show();
            window.Load();
        }

        private void SetUpCheck()
        {

            LocalDatabaseManager.SetUp();
            ServerDatabaseManager.Setup();
            string deskMacAddress = SystemInformationManager.GetMacAddress();
            if (deskMacAddress == null)
            {
                LucidDesk.UserControls.MessageBox messagebox = new UserControls.MessageBox();
                messagebox.ShowMessageBox("Somthing Wrong in server connection ", "Error", UserControls.MessageBoxType.Ok);
                Thread.Sleep(5000);
                LogManager.LogException("Somthing Wrong in server connection ");
                ShutdownApp();
            }
            else
            {
                if (!ServerDatabaseManager.IsDeskExits(deskMacAddress))
                {
                    loginWindow loginWindow = new loginWindow();
                    loginWindow.OnClickNext += LoginWindowOnClickNext;
                    loginWindow.ShowDialog();

                    Dictionary<string, Desk> DeskProfilesServerDictionary = ServerDatabaseManager.GetDeskProfiles();
                    DeskProfileManager.DeskProfilesDictionary = DeskProfileManager.GetDeskProfilesData();
                    List<Desk> deskProfiles = DeskProfilesServerDictionary.Values.ToList();
                    for (int i = 0; i < deskProfiles.Count; i++)
                    {
                        if (DeskProfileManager.DeskProfilesDictionary.ContainsKey("" + deskProfiles[i].Id))
                        {
                            deskProfiles[i].IsFavorite = DeskProfileManager.DeskProfilesDictionary["" + deskProfiles[i].Id].IsFavorite;

                        }
                        DeskProfileManager.CreateDeskProfiledata(deskProfiles[i]);
                    }
                }
                else
                {
                    DeskProfileManager.DeskProfilesDictionary = ServerDatabaseManager.GetDeskProfiles();
                    List<Desk> deskProfiles = DeskProfileManager.DeskProfilesDictionary.Values.ToList();
                    for (int i = 0; i < deskProfiles.Count; i++)
                    {
                        DeskProfileManager.UpdateDeskProfiledata(deskProfiles[i]);
                    }
                }
                DeskProfileManager.DeskProfilesDictionary = DeskProfileManager.GetDeskProfilesData();
                DeskProfileManager.DeskProfiles = DeskProfileManager.DeskProfilesDictionary.Values.ToList();
                StartServerConnection();
            }

        }

        private void LoginWindowOnClickNext(object sender, Desk desk)
        {
            ServerDatabaseManager.CreateDeskProfile(desk);
            ((Window)sender).Close();
        }

        private void StartServerConnection()
        {

            if (LucidDesk.MainWindow.ServerNetworkManager.isStarted) return;
            //Visibility = Visibility.Hidden;
            //ShowInTaskbar = false;
            //ServerNetworkManager.StartServer;
            LucidDesk.MainWindow.ServerNetworkManager.StartServer();
            // ServerNetworkManager.isStarted = true;


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
