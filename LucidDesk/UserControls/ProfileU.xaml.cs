using LucidDesk.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LucidDesk.UserControls
{
    /// <summary>
    /// Interaction logic for ProfileU.xaml
    /// </summary>
    public partial class ProfileU : UserControl
    {
        private Desk desk;

        public string DeskUserName
        {
            get { return (string)GetValue(DeskUserNameProperty); }
            set { SetValue(DeskUserNameProperty, value); }
        }

        public static readonly DependencyProperty DeskUserNameProperty =
            DependencyProperty.Register("DeskUserName", typeof(string), typeof(ProfileU), new PropertyMetadata("Name"));


        public string PCName
        {
            get { return (string)GetValue(PCNameProperty); }
            set { SetValue(PCNameProperty, value); }
        }

        public static readonly DependencyProperty PCNameProperty =
            DependencyProperty.Register("PCName", typeof(string), typeof(ProfileU), new PropertyMetadata("PcName"));



        public string DeskOSName
        {
            get { return (string)GetValue(DeskOSNameProperty); }
            set { SetValue(DeskOSNameProperty, value); }
        }

        public static readonly DependencyProperty DeskOSNameProperty =
            DependencyProperty.Register("DeskOSName", typeof(string), typeof(ProfileU), new PropertyMetadata("Windows"));



        public string DeskId
        {
            get { return (string)GetValue(DeskIdProperty); }
            set { SetValue(DeskIdProperty, value); }
        }

        public static readonly DependencyProperty DeskIdProperty =
            DependencyProperty.Register("DeskId", typeof(string), typeof(ProfileU), new PropertyMetadata(""));

        public Desk Desk
        {
            set
            {
                desk = value;
                DeskUserName = desk.DeskUserName;
                PCName = Desk.HostName;
                DeskOSName = Desk.OsName;
                DeskId =""+Desk.Id;
                DeskProfileImageComponent.Image = Desk.ProfileImage;
                DesktopImageComponent.Source = Desk.DesktopImage;
            }
            get
            {
                return desk;
            }
        }





        public ProfileU()
        {
            InitializeComponent();
        }
    }
}
