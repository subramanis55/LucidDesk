using LucidDesk.Manager.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace LucidDesk.UserControls.Common
{
    public enum NotificationType
    {
        Information, Error, Invite, None
    }

    public class NotificationManager
    {
        public event EventHandler OnClickNotification;
        public event EventHandler<DeskConnectionInformation> OnClickInviteStatusGet;

        System.Timers.Timer arrangeTimer = new System.Timers.Timer();
        private int x = Screen.PrimaryScreen.Bounds.Width - 50;
        private int y = Screen.PrimaryScreen.Bounds.Height - 80;

        public int BorderRadius
        {
            set
            {
                NotificationControl.BorderRadius = value;
            }
            get
            {
                return NotificationControl.BorderRadius;
            }
        }
        List<NotificationControl> NotifiactionList = new List<NotificationControl>();
        public void ArrangeNotification()
        {
            for (int i = 0; i < NotifiactionList.Count; i++)
            {
                NotifiactionList[i].Left = x - NotifiactionList[i].ActualWidth;
                NotifiactionList[i].Top = y - (NotifiactionList[i].ActualHeight * (NotifiactionList.Count - i) - 10 * i);
            }
        }


        public void CreateNotification(string message, NotificationType notificationType)
        {
            NotificationControl obj = new NotificationControl(message, notificationType) { Left = x - 380 };
            obj.OnEnd += DisposeNotification;
            obj.Invoke();
          
            NotifiactionList.Add(obj);
            ArrangeNotification();
        }
        public void CreateInviteRequestNotification(DeskConnectionInformation deskConnectionInformation)
        {
            NotificationControl obj = new NotificationControl(deskConnectionInformation) { Left= x - 380 };
            obj.OnEnd += DisposeNotification;
            obj.OnClickInviteStatusGet+= OnClickInviteStatusGet;
            obj.Invoke();
            NotifiactionList.Add(obj);
            ArrangeNotification();
        }
        private void DisposeNotification(object sender, EventArgs args)
        {
            NotifiactionList.Remove(((NotificationControl)sender));
            ((Window)sender).Close();
            ArrangeNotification();
        }
        public void NotificationFormClickInvoke(object sender, EventArgs args)
        {
            OnClickNotification?.Invoke(this, args);

        }
    }
}
