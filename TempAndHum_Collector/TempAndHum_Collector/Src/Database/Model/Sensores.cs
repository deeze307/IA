using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using TempAndHum_Collector.Core;

namespace TempAndHum_Collector.Src.Database.Model
{
    class Sensores : Controller
    {
        public static List<Sensores> list = new List<Sensores>();

        public int id_sensor;
        public string nombre;
        public string ip_sensor;
        public string last_date_saved;
        
        /// <summary>
        /// Descarga los sensores Activos
        /// </summary>
        public static void Actives()
        {
            list = new List<Sensores>();
            string query = "CALL temp_hum.getActiveSensors()";
            MySqlConnector mysql = new MySqlConnector();
            mysql.LoadConfig("IASERVER");

            DataTable dt = mysql.Query(query);
            if(mysql.rows)
            {
                foreach (DataRow r in dt.Rows)
                {
                    Sensores sensor = new Sensores();
                    sensor.id_sensor = int.Parse(r["id_sensor"].ToString());
                    sensor.nombre = r["nombre"].ToString();
                    sensor.ip_sensor = r["ip_sensor"].ToString();
                    sensor.last_date_saved = r["last_date_saved"].ToString();
                    list.Add(sensor);
                }
            }
        }

        public static int Total()
        {
            return list.Count();
        }

        public static bool UpdatePing(string nombreSensor,DateTime fecha_medicion)
        {
            try
            {
                string fecha = fecha_medicion.ToString("yyyy-MM-dd H:mm:ss");
                string query = "CALL temp_hum.sp_updatePing('" + nombreSensor + "','"+ fecha + "')";
                MySqlConnector mysql = new MySqlConnector();
                mysql.LoadConfig("IASERVER");
                mysql.Query(query);
                
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
