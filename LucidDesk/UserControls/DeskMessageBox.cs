using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace LucidDesk.UserControls
{
    public class DeskMessageBox
    {
        public static LucidDesk.UserControls.MessageBoxScreen messagebox = new UserControls.MessageBoxScreen() { IsHideOnly = true, WindowStartupLocation = WindowStartupLocation.CenterOwner };
        public static DialogResult ShowMessageBox(string message, string heading, MessageBoxType messageBoxType, Window parent=null)
        {
            messagebox.Owner = parent;
          return  messagebox.ShowMessageBox(message, heading, messageBoxType);
        }
    }
}
