using DeskUI.Manager.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskUI.Manager.Classes.DataSchema
{
    public class Data
    {
        public ReponseAndReqType ReponseAndReqType { set; get; }
        public object DataObject { set; get; }

        public DeskImageData GetDeserializeDeskImageData()
        {
            return JsonConvert.DeserializeObject<DeskImageData>(DataObject.ToString());
        }
        public DeskControlData GetDeserializeDeskControlData()
        {
            return JsonConvert.DeserializeObject<DeskControlData>(DataObject.ToString());
        }
        public DeskConnectionInformation GetDeserializeDeskConnectionInformation()
        {
            return JsonConvert.DeserializeObject<DeskConnectionInformation>(DataObject.ToString());
        }

    }
}
