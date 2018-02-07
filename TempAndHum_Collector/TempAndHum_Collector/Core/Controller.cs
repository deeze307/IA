using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using TempAndHum_Collector.Core;
using TempAndHum_Collector.Collector;

namespace TempAndHum_Collector.Core
{
    public class Controller : CSVModel
    {
        public bool sensorReady = false;
        public MySqlConnection mysql = new MySqlConnection();

        public Worker worker { get; set; }
        public RichLog controllerLog { get; set; }
        public Config controllerConfig { get; set; }

        public Controller()
        {
            controllerConfig = new Config();
            worker = new Worker(this);
        }

        #region Inicializadores
        public void Prepare(RichTextBox logRichTextBox, ProgressBar progress, DoWorkEventHandler WorkerStart)
        {
            controllerLog = new RichLog(logRichTextBox);
            LoadConfig();
            LoadWorker(progress, WorkerStart);
        }

        public void LoadConfig()
        {
            controllerConfig.setConfig();
        }
        public void ReLoadConfig()
        {
            LoadConfig();
        }
        private void LoadWorker(ProgressBar progress, DoWorkEventHandler WorkerStart)
        {
            worker.timerInterval = controllerConfig.intervalo;
            worker.progressBar = progress;
            worker.WorkerStart = new DoWorkEventHandler(WorkerStart);
        }
        #endregion

        public async void Start(bool forceStart = false)
        {
            if (Config.dbDownloadComplete)
            {
                controllerLog.verbose("Iniciando operaciones");
                worker.StartOperation(forceStart);
            }
            else
            {
                controllerLog.warning("No se pudo descargar informacion del servidor.");
                controllerLog.verbose("Re intentando conexion...");

                bool downloaded = await Task.Run(() =>
                    Config.dbDownload()
                );

                if (downloaded)
                {
                    controllerLog.verbose("Iniciando operaciones");
                    worker.StartOperation(forceStart);
                }
            }
        }

        public void Stop()
        {
            controllerLog.verbose("Deteniendo operaciones");
            worker.StopTimer();
        }
    }
}
