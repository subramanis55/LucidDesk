using LucidDesk.Manager.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Classes.DataSchema
{
    public class DeskControlData
    {
        public ControlKeyType ControlDataType { set; get; }

        public string ControlData {  set; get; }
    }
}
