using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using TempAndHum_Collector.Core;

namespace TempAndHum_Collector.Src.Database.Model
{
    public class Mediciones : Controller
    {

        public static bool createMeasure(int id_sensor, string temperatura, string humedad, DateTime fecha_medicion)
        {
            bool creado = true;
            string fecha = fecha_medicion.ToString("yyyy-MM-dd H:mm:ss");
            string query = "CALL temp_hum.sp_createMeasurePoint('" + id_sensor + "','" + humedad + "','"+ temperatura +"','"+ fecha + "')";
            MySqlConnector mysql = new MySqlConnector();
            mysql.LoadConfig("IASERVER");
            mysql.Query(query);
            if(mysql.rows)
            {
                creado = true;
            }
            else
            {
                creado = false;
            }
            return creado;
        }
    }
}
