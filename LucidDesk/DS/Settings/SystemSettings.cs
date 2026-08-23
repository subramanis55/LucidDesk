using LucidDesk.DS.Enum;
using System;

namespace LucidDesk.DS.Settings
{
    [Serializable]
    public class SystemSettings
    {
        public ApplicationMode ApplicationMode { get; set; } = ApplicationMode.Local;
        public string ApplicationName { get; set; } = "LucidDesk";
        public int FrameSharePerSecond { get; set; } = 20;
        public int MouseInputSharePerSecond { get; set; } = 20;
        public int ApplicationDefaultPort { get; set; } = 9000;
        public int ServerPort { set; get; } = 3306;
        public string ServerAddress { set; get; } = "localhost";
        public string ServerBaseURL { set; get; } = "";
        public string ConnectionMode { set; get; } = "TCP";
    }
}
