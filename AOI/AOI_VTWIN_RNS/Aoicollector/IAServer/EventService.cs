using CollectorPackage.Aoicollector.IAServer.Mapper;
using CollectorPackage.Aoicollector.Inspection;
using Newtonsoft.Json;
using System;

namespace CollectorPackage.Aoicollector.IAServer
{
    public class EventService : Api
    {
        public PanelInfoMapper result;

        public PanelInfoMapper SendEventInspection(Panel panel)
        {
            result = new PanelInfoMapper();
            hasResponse = false;

            //try
            //{
            //    string path = string.Format("{0}/api/aoicollector/event/inspection/{1}", apiUrl, panel.barcode);               

            //    string jsonData = Consume(path);
            //    hasResponse = true;
            //    string[] data = jsonData.Split(':');
            //    string serial = data[1].Substring(1, 10);
            //    result = JsonConvert.DeserializeObject<PanelInfoMapper>(jsonData);
            //}
            //catch (Exception ex)
            //{
            //    error = ex;
            //}

            return result;
        }
    }
}
