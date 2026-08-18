using LucidDesk.Manager.Enum;
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
        public ApplicationMode ApplicationMode { get; set; } = ApplicationMode.Local;
        public string ApplicationName { get; set; } = "LucidDesk";
        public int FrameSharePerSecond { get; set; } = 20;
        public int MouseInputSharePerSecond { get; set; } = 20;
        public int ServerPort { set; get; } = 3306;
        public string ServerAddress { set; get; } = "localhost";
 

    }
}
