using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempAndHum_Collector.Src.Config;
using TempAndHum_Collector.Src.Database.Model;

namespace TempAndHum_Collector.Core
{
    public class Config
    {
        public int intervalo { get; set; }
        public string inspectionCsvPath { get; set; }

        public void setConfig()
        {
            intervalo = int.Parse(AppConfig.Read("APP", "intervalo"));
            inspectionCsvPath = AppConfig.Read("APP", "csvPath");
        }


        public static bool isAutoStart()
        {
            return AppConfig.Read("APP", "autostart").ToString().Equals("true");
        }

        public static bool dbDownloadComplete { get; set; }
        public static bool dbDownload()
        {
            Log.system.verbose("Iniciando descarga de datos MySql");
            try
            {
                Sensores.Actives();

                Log.system.verbose("Sensores Activos: " + Sensores.Total());

                dbDownloadComplete = true;
            }
            catch(Exception ex)
            {
                dbDownloadComplete = false;
                Log.system.error(ex.Message);
            }
            return dbDownloadComplete;
        }
    }
}
