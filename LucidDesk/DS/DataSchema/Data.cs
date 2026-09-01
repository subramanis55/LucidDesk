using LucidDesk.DS.Classes;
using LucidDesk.DS.Enum;
using Newtonsoft.Json;

namespace LucidDesK.DS.DataSchema
{
    public class Data
    {
        public Data(ReponseAndReqType reponseAndReqType, object data)
        {
            ReponseAndReqType = reponseAndReqType;
            DataObject = data;
        }
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
