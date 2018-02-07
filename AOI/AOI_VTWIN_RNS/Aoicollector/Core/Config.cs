using System;
using System.Collections.Generic;
using System.Linq;
using CollectorPackage.Aoicollector.Inspection.Model;
using CollectorPackage.Src.Config;

namespace CollectorPackage.Aoicollector.Core
{
    public class Config
    {
        public static bool downloading = false;
        public static bool debugMode = false;

        public string machineType       { get; set; }
        public string machineNameKey    { get; set; }

        public int intervalo            { get; set; }
        public string xmlExportPath     { get; set; }
        public string dataProgPath      { get; set; }
        public string inspectionCsvPath { get; set; }
        public string inspectionCsvPathPTH { get; set; }

        public void setConfig(string _machineType, string _machineNameKey)
        {
            machineType = _machineType;
            machineNameKey = _machineNameKey;
            inspectionCsvPath = AppConfig.Read(machineType, "csvPath");
            xmlExportPath = AppConfig.Read(machineType, "xmlExport");
            dataProgPath = AppConfig.Read(machineType, "dataProg");
            intervalo = int.Parse(AppConfig.Read(machineType, "intervalo"));
        }

        #region ESTATICAS
        public static bool dbDownloadComplete { get; set; }
        public static List<Machine> byPassLine = new List<Machine>();
        public static List<Machine> toEndInspect = new List<Machine>();

        public static bool isByPassMode(Machine aoi)
        {
            int exist = byPassLine.Where(o => o.maquina.ToString() == aoi.maquina.ToString()).Count();
            if (exist > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isEndInspect(Machine aoi)
        {
            int exist = toEndInspect.Where(o => o.maquina.ToString() == aoi.maquina.ToString()).Count();
            if (exist > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isAutoStart()
        {
            return AppConfig.Read("APP", "autostart").ToString().Equals("true");
        }

        public static bool dbDownload()
        {
            #region DESCARGA INFORMACION DE MYSQL
            Log.system.verbose("Iniciando descarga de datos MySql");

            try
            {            
                Faultcode.Download();
                Machine.Download();
                PcbInfo.Download();

                List<Machine> lista = Machine.list;

                Log.system.notify("Faultcodes: " + Faultcode.Total());
                Log.system.notify("Maquinas: " + Machine.Total());
                Log.system.notify("PcbInfo: " + PcbInfo.Total());

                dbDownloadComplete = true;
            }
            catch (Exception ex)
            {
                dbDownloadComplete = false;
                Log.system.error(ex.Message);
            }
            #endregion

            return dbDownloadComplete;
        }
        #endregion
    }
}
