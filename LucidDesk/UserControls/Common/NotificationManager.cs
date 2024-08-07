using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LucidDesk.UserControls.Common
{
    public enum NotificationType
    {
        Information, Error,Invite, None
    }
   
        public class NotificationThrowManager
        {
            public event EventHandler OnClickNotification;

        System.Timers.Timer arrangeTimer = new System.Timers. Timer();
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
                    NotifiactionList[i].Margin = new System.Windows.Thickness(x - NotifiactionList[i].ActualWidth, y - (NotifiactionList[i].ActualHeight * (NotifiactionList.Count - i) - 10 * i),0,0);
                }
            }

     
            public void CreateNotification(string message, NotificationType notificationType)
            {
            NotificationControl obj = new NotificationControl(message, notificationType);
                obj.OnEnd += DisposeNotification;
                obj.Show();
                NotifiactionList.Add(obj);
                ArrangeNotification();
            }
            private void DisposeNotification(object sender, EventArgs args)
            {
                NotifiactionList.Remove(((NotificationControl)sender));
                ((Control)sender).Dispose();
                ArrangeNotification();
            }
            public void NotificationFormClickInvoke(object sender, EventArgs args)
            {
                OnClickNotification?.Invoke(this, args);

            }
        
    }
}
