using System;
using System.Windows.Forms;
using TempAndHum_Collector.Collector;

using TempAndHum_Collector.Core;
using TempAndHum_Collector.Src.Config;
using TempAndHum_Collector.Src.Database;

using System.Threading.Tasks;


namespace TempAndHum_Collector
{
    public partial class Form1 : Form
    {
        CollectorController _collector;
        Controller _controller = new Controller();
        MySqlConnector iaserver = new MySqlConnector();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log.system = new RichLog(rtbLog);
            
            _collector = new CollectorController(rtbLog,progressBar1);
            loadConfDB();

            if(Config.isAutoStart())
            {
                _collector.Start(true);
            }
        }

        private void loadConfDB()
        {
            
            _controller.LoadConfig();
           
            iaserver.LoadConfig("IASERVER");

            txtServer.Text = iaserver.host;
            txtUser.Text = iaserver.user;
            txtPass.Text = iaserver.pass;
            txtDB.Text = iaserver.database;
            txtCSV.Text = _controller.controllerConfig.inspectionCsvPath;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            bool res = true;
            res = ValidarMysql();
            iaserver.LoadConfig("IASERVER");
            iaserver.LoadConfig("APP");
            if(res)
            { MessageBox.Show("Configuración Guardada!!!"); }
        }

        private bool ValidarMysql()
        {
            bool valid = true;
            if (!txtUser.Text.Trim().Equals(""))
            {
                AppConfig.Save("IASERVER", "db_user", txtUser.Text.Trim());
            }
            else
            {
                MessageBox.Show("Complete el campo Usuario");
                valid = false;
            }

            if (!txtPass.Text.Trim().Equals(""))
            {
                AppConfig.Save("IASERVER", "db_pass", txtPass.Text.Trim());
            }
            else
            {
                MessageBox.Show("Complete el campo Clave");
                valid = false;
            }

            if (!txtServer.Text.Trim().Equals(""))
            {
                AppConfig.Save("IASERVER", "db_host", txtServer.Text.Trim());
            }
            else
            {
                MessageBox.Show("Complete el campo Servidor");
                valid = false;
            }

            if (!txtDB.Text.Trim().Equals(""))
            {
                AppConfig.Save("IASERVER", "db_database", txtDB.Text.Trim());
            }
            else
            {
                MessageBox.Show("Complete el campo Database");
                valid = false;
            }

            if (!txtCSV.Text.Trim().Equals(""))
            {
                AppConfig.Save("APP", "csvPath", txtCSV.Text.Trim());
            }
            else
            {
                MessageBox.Show("Complete el campo Ruta de CSV");
                valid = false;
            }
            return valid;
        }
    }
}
