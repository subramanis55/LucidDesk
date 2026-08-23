using System;

namespace LucidDesk.Log
{
    public static class LogManager
    {
        public static void Intialize()
        {

        }
        public static void LogException(string error)
        {
            System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: {error}\n\n");
        }
    }
}
