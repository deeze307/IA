using System;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Data;
using System.Collections.Generic;

using TempAndHum_Collector.Core;
using TempAndHum_Collector.Util.Files;
using TempAndHum_Collector.Src.Database.Model;
using System.Threading.Tasks;

namespace TempAndHum_Collector.Collector
{
    public class CollectorController : Controller
    {
        public CollectorController(RichTextBox log, ProgressBar progress)
        {
            Prepare(log, progress, WorkerStart);
        }

        private void WorkerStart(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                if (!sensorReady)
                {
                    StartInspection();
                }
                else
                {
                    controllerLog.warning("WorkerStart() => CheckPcbFiles() => no finalizo correctamente");
                }
            }
            catch (Exception ex)
            {
                controllerLog.stack("WorkerStart()", this, ex);
            }
        }

        private void StartInspection()
        {
            //Actualizo los datos de los sensores
            Sensores.Actives();

            controllerLog.verbose("Localizando CSV de Mediciones");
            
            // Obtengo archivos CSV en carpeta de Mediciones para cada Sensor Activo
            foreach (Sensores sensor in Sensores.list)
            {
                controllerLog.info("");
                controllerLog.info("Revisando mediciones de sensor " + sensor.nombre);
                string anioMes = DateTime.Today.ToString("yyyy-MM");
                string fecha = DateTime.Today.ToString("yyyy-MM-dd");
                IOrderedEnumerable<FileInfo> csv = FilesHandler.GetFiles(sensor.nombre+"_"+fecha+".csv", controllerConfig.inspectionCsvPath+"\\"+ sensor.nombre+"\\"+anioMes);
                int totalCsv = csv.Count();

                worker.SetProgressTotal(totalCsv);

                if (totalCsv > 0)
                {
                    int file_count = 0;

                    foreach (FileInfo file in csv)
                    {
                        file_count++;

                        //RedisRuntime(file_count.ToString(), totalCsv.ToString());

                        controllerLog.info("---------------------------------------------");
                        controllerLog.info(" Procesando " + file_count + " / " + totalCsv);
                        controllerLog.info("---------------------------------------------");

                        HandleInspection(file,sensor);

                        worker.SetProgressWorking(file_count);

                    }
                    

                    controllerLog.info("No hay más datos a revisar");
                    controllerLog.info("++++++++++++++++++++++++++");
                }
                else
                {
                    //RedisRuntime();

                    controllerLog.info("No se encontraron Mediciones de el día de la fecha para el sensor "+sensor.nombre);
                }

                
            }

        }

        private void HandleInspection(FileInfo file, Sensores sensor)
        {
            DateTime fecha = DateTime.Parse(sensor.last_date_saved);

            #region Lectura de las filas del archivo
            DataTable csv = null;
            try
            {
                controllerLog.debug("Leyendo: " + file.FullName);
                csv = FilesHandler.FileToTable(file.FullName, ',');
            }
            catch(Exception ex)
            {
                controllerLog.stack("No fue posible leer: " + file.FullName, this, ex);
            }
            
            #endregion
            if (csv != null)
            {
                if(csv.Rows.Count > 0)
                {
                    int filas = 0;
                    bool creado = true;
                    foreach (DataRow Fila in csv.Rows)
                    {
                        humedad = Fila[0].ToString();
                        temperatura = Fila[1].ToString();
                        fecha_medicion = Convert.ToDateTime(Fila[4].ToString());
                        if (fecha_medicion > fecha)
                        {
                            filas++;
                            creado = Mediciones.createMeasure(sensor.id_sensor, temperatura, humedad, fecha_medicion);
                            if(!creado)
                            {
                                controllerLog.warning("La Fila con fecha " + fecha_medicion + " para sensor "+sensor.nombre+" NO se agregó a la BD");
                            }
                        }
                        else
                        {
                            //controllerLog.warning(file.FullName+"| la fila con fecha " + fecha_medicion + " NO se agregará a la BD");
                        }
                    }
                    if(filas > 0)
                    {
                        controllerLog.success("Registros Agregados: " + filas);
                    }
                    else
                    {
                        controllerLog.success("No se han registrado Nuevas Mediciones");
                    }
                    
                    //Actualizo la fecha de la última lectura de las mediciones
                    controllerLog.info("---------------------------------------------");
                    controllerLog.info("Actualizando Ping de Sensor " + sensor.nombre);
                    Sensores.UpdatePing(sensor.nombre, fecha_medicion);
                    controllerLog.info("Ping Actualizado.");
                    controllerLog.info("---------------------------------------------");
                }
            }

        }
    }
}
