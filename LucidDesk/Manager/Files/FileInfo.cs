using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucidDesk.Files
{
    public class FileInfo
    {
        public FileInfo(string path)
        {
            path = path.Replace("/", "\\");
            FullName = path;
            FileName = Path.GetFileName(path);
            DirectoryPath = Path.GetDirectoryName(path);
        }
        public string FileName { set; get; }
        public string DirectoryPath { set; get; }

        public string FullName { set; get; }
    }
}
