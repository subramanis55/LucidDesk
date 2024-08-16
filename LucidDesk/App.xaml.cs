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

namespace LucidDesk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
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
            if(deskMacAddress==null){
                LucidDesk.MainWindow.NotificationManager.CreateNotification("Somthing Wrong",UserControls.Common.NotificationType.Information);
                Thread.Sleep(5000);
                Environment.Exit(0);
            }
            else{
                if (!ServerDatabaseManager.IsDeskExits(deskMacAddress))
                {
                    loginWindow loginWindow = new loginWindow();
                    loginWindow.OnClickNext += LoginWindowOnClickNext;
                    loginWindow.ShowDialog();

                    Dictionary<string,Desk> DeskProfilesServerDictionary = ServerDatabaseManager.GetDeskProfiles();
                    DeskProfileManager.DeskProfilesDictionary = DeskProfileManager.GetDeskProfilesData();
                    List<Desk> deskProfiles = DeskProfilesServerDictionary.Values.ToList();
                    for (int i = 0; i < deskProfiles.Count; i++)
                    {
                    if(DeskProfileManager.DeskProfilesDictionary.ContainsKey(""+ deskProfiles[i].Id)){
                            deskProfiles[i].IsFavorite = DeskProfileManager.DeskProfilesDictionary["" + deskProfiles[i].Id].IsFavorite;

                    }
                        DeskProfileManager.CreateDeskProfiledata(deskProfiles[i]);
                    }
                }
                else
                {
                    //    DeskProfileManager.DeskProfilesDictionary = ServerDatabaseManager.GetDeskProfiles();
                    //    List<Desk> deskProfiles = DeskProfileManager.DeskProfilesDictionary.Values.ToList();
                    //    for (int i = 0; i < deskProfiles.Count; i++)
                    //    {
                    //        DeskProfileManager.UpdateDeskProfiledata(deskProfiles[i]);
                    //    }
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
    }
}
