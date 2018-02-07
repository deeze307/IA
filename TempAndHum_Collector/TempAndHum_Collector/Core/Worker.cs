using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace TempAndHum_Collector.Core
{
    public class Worker
    {
        public Controller _controller;
        public bool Detener = false;
        public Timer timer;
        public int timerInterval = 1;
        public ProgressBar progressBar;
        public BackgroundWorker bgWorker = new BackgroundWorker();

        public DoWorkEventHandler WorkerStart;

        public Worker(Controller _cont)
        {
            _controller = _cont;
        }

        #region Timer
        /// <summary>
        /// Inicializa el timer para ejecutar las operaciones del worker
        /// </summary>
        public void StartTimer()
        {
            int milisegundos = (int)TimeSpan.FromSeconds(timerInterval).TotalMilliseconds;

            timer = new Timer();
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = milisegundos;
            timer.Enabled = true;
            timer.Start();

            _controller.controllerLog.info("Timer iniciado en: " + _controller.controllerConfig.intervalo + " seg");
            Log.system.info("Timer de Sensores fue iniciado. Intervalo: " + _controller.controllerConfig.intervalo + " seg ");
        }

        public void StopTimer()
        {
            Detener = true;
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
                Log.system.warning("Se detuvo la ejecucion del timer de los sensores");
                _controller.controllerLog.info("Timer de ejecucion detenido con exito. ");
            }
            else
            {
                _controller.controllerLog.verbose("El timer no se encuentra en ejecucion. ");
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            StartOperation();
        }
        #endregion

        #region Worker
        public void StartOperation(bool forceStart = false)
        {
            if (forceStart) { Detener = false; }
            if (!Detener)
            {
                if (timer == null)
                {
                    StartTimer();
                }

                if (!bgWorker.IsBusy)
                {
                    ExecuteWorker();
                }
                else
                {
                    _controller.controllerLog.debug("*** Espere, el proceso se encuentra aun en ejecucion");
                }
            }
        }

        public BackgroundWorker ExecuteWorker()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += new DoWorkEventHandler(WorkerStart);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(WorkerProgressChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerRunCompleted);
            bgWorker.RunWorkerAsync();

            return bgWorker;
        }

        private void WorkerRunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Value = 0;
            _controller.controllerLog.verbose(string.Format("*** Operacion completa ***"));
            _controller.controllerLog.verbose("---------------------------------------------");
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        public void SetProgressTotal(int count)
        {
            progressBar.Invoke((MethodInvoker)(() => progressBar.Maximum = count));
        }

        public void SetProgressWorking(int count)
        {
            bgWorker.ReportProgress(count);
        }
        #endregion
    }
}
