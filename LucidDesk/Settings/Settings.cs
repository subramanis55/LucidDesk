using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucidDesk.Settings
{
    [Serializable]
    public class Settings
    {
        public string ServerHostName { set; get; } = "";
        public string ServerPort { set; get; } = "3306";
        public string ServerIpAddress { set; get; } = "localhost";
        public string ServerPassword { set; get; } = "";
        public string ServerUserName { set; get; } = "root";
        public string ServerDatabase { set; get; } = "luciddesk";

    }
}
