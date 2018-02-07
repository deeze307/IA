using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cogiscan_Utilities.WebServices;
using Cogiscan_Utilities.Entitys;

namespace Cogiscan_Utilities
{
    public partial class rawMaterialWatcher : Form
    {
        public rawMaterialWatcher()
        {
            InitializeComponent();
        }

        private void rawMaterialWatcher_Load(object sender, EventArgs e)
        {
            setLocation();//posiciono el formulario
            comboBox1.DataSource = DB2.ConexionDB2.AllLocations();
            comboBox1.SelectedIndex = 0;
            comboBox3.DataSource = Clases.containerLocation.locationType.Distinct().ToList();
            comboBox3.SelectedIndex = 0;
            cbxRMWModels.DataSource = Conexion.queryRMWModels();
            btnMostrarUbicacion.BackColor = Color.GreenYellow;
            cbxRMWModels.Enabled = false;
            comboBox1.Enabled = true;
            getShift();
            timerMaterialsOnLocation.Tick += new EventHandler(timerMaterialsOnLocation_Tick);
            timerShift.Tick +=new EventHandler(timerShift_Tick);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.Text == "TODOS" || comboBox3.Text =="Todos")
            {
                comboBox1.DataSource = DB2.ConexionDB2.AllLocations();
            }
            else
            {
                comboBox1.DataSource = DB2.ConexionDB2.FilteredLocations(comboBox3.Text);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboBox1.Text != "-- Seleccione Contenedor --") && (btnMostrarUbicacion.BackColor == Color.GreenYellow))
            {
                timerMaterialsOnLocation.Enabled = true;
                timerMaterialStatus.Enabled = false;
            }
            else if ((comboBox1.Text != "-- Seleccione Contenedor --") && (btnMostrarStatus.BackColor == Color.GreenYellow))
            {
                timerMaterialStatus.Enabled = true;
                timerMaterialsOnLocation.Enabled = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Form rmw = this.Owner;
            MenuStrip menuStrip = (MenuStrip)rmw.Controls["menuStrip1"];
            menuStrip.Items["estadoDeMaterialesToolStripMenuItem"].Text = "Estado de Materiales";
            this.Hide();
        }

        private void setLocation()
        {
            if (Screen.AllScreens.Length > 1)
            {
                int YbtnClose = Screen.AllScreens[1].WorkingArea.Height - btnClose.Top;
                btnClose.Location = new Point()
                {
                    X = Screen.AllScreens[1].WorkingArea.Location.X - (Screen.AllScreens[1].WorkingArea.Location.X - (Screen.AllScreens[1].WorkingArea.Location.X - 50)),
                    Y = 12
                };
                this.Width = Screen.AllScreens[1].WorkingArea.Width;
                this.Height = Screen.AllScreens[1].WorkingArea.Height;
                dataGridView1.Width = Screen.AllScreens[1].WorkingArea.Width - 300;
                dataGridView1.Height = Screen.AllScreens[1].WorkingArea.Height - 75;
                dataGridView2.Width = Screen.AllScreens[1].WorkingArea.Width - 25;
                dataGridView2.Height = Screen.AllScreens[1].WorkingArea.Height - 75;
                this.Location = new Point();
                this.Location = new Point()
                {
                    X = Math.Max(Screen.AllScreens[1].WorkingArea.Location.X, Screen.AllScreens[1].WorkingArea.Location.X + (Screen.AllScreens[1].WorkingArea.Width - this.Width)),
                    Y = Math.Max(Screen.AllScreens[1].WorkingArea.Location.Y, Screen.AllScreens[1].WorkingArea.Location.Y + (Screen.AllScreens[1].WorkingArea.Height - this.Width))
                };
            }
            else
            {
                this.Width = Screen.AllScreens[0].WorkingArea.Width;
                this.Height = Screen.AllScreens[0].WorkingArea.Height;
                dataGridView1.Width = Screen.AllScreens[0].WorkingArea.Width - 300;
                dataGridView1.Height = Screen.AllScreens[0].WorkingArea.Height - 75;
                dataGridView2.Width = Screen.AllScreens[0].WorkingArea.Width - 25;
                dataGridView2.Height = Screen.AllScreens[0].WorkingArea.Height - 75;
                this.Location = new Point();
                this.Location = new Point()
                {
                    X = Math.Max(Screen.AllScreens[0].WorkingArea.Location.X, Screen.AllScreens[0].WorkingArea.Location.X + (Screen.AllScreens[0].WorkingArea.Width - this.Width)),
                    Y = Math.Max(Screen.AllScreens[0].WorkingArea.Location.Y, Screen.AllScreens[0].WorkingArea.Location.Y + (Screen.AllScreens[0].WorkingArea.Height - this.Width))
                };
 
            }
        }

        private void btnMostrarUbicacion_Click(object sender, EventArgs e)
        {
            dataGridView2.Visible = false;
            dataGridView1.Visible = true;
            btnMostrarUbicacion.BackColor = Color.GreenYellow;
            btnMostrarStatus.BackColor = Color.Ivory;
            timerMaterialStatus.Enabled = false;
            timerMaterialsOnLocation.Enabled = true;
            cbxRMWModels.Enabled = false;
            comboBox1.Enabled = true;
        }

        private void btnMostrarStatus_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataGridView2.Visible = true;
            btnMostrarUbicacion.BackColor = Color.Ivory;
            btnMostrarStatus.BackColor = Color.GreenYellow;
            timerMaterialsOnLocation.Enabled = false;
            timerMaterialStatus.Enabled = true;
            cbxRMWModels.Enabled = true;
            comboBox1.Enabled = false;
        }

        private void fillDGV(Array materiales)
        {
            BindingSource source = new BindingSource();
            source.DataSource = materiales;
            if (btnMostrarUbicacion.BackColor == Color.GreenYellow)
            {
                dataGridView1.DataSource = source;
                dataGridView2.DataSource = null;
            }
            else
            {
                dataGridView2.DataSource = source;
                dataGridView1.DataSource = null;
            }
            dataGridView1.ClearSelection();
            dataGridView2.ClearSelection();
        }

        private void timerMaterialStatus_Tick(object sender, EventArgs e)
        {
            
            //Array materiales = WS.getRawMaterialStatus(comboBox1.Text);
            //Array status = Conexion
        }

        private void timerMaterialsOnLocation_Tick(object sender, EventArgs e)
        {
            Array materiales = WS.getContentsAndReturnValues(comboBox1.Text);
            
            //fillDGV(materiales);
        }
        string selectedModel;
        private void cbxRMWModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRMWModels.Text != "-- Seleccione Modelo --")
            {
                comboBox1.Enabled = true;
                selectedModel = cbxRMWModels.Text;
                Array materiales = Conexion.queryRMWBOM(selectedModel);

                fillDGV(materiales);
                timerMaterialStatus.Tick += new EventHandler(timerMaterialStatus_Tick);
            }
                
        }
        private void fillStatus(DataGridView dgv,DateTime horaInicio, DateTime horaFin)
        {
            foreach ( DataGridViewRow row in dgv.Rows)
            {
                Conexion.getRawMaterialStatus(row.ToString(),horaInicio,horaFin);
            }
        }

        private void timerShift_Tick(object sender, EventArgs e)
        {
            DateTime _horaInicio = new DateTime();
            DateTime _horaFin = new DateTime();
            shiftEntity hora = new shiftEntity();
            string Turno = getShift();
            if (Turno == "TM")
            {
                _horaInicio = hora.TMini;
                _horaFin = hora.TMfin;
            }
            else if (Turno =="TT")
            {
                _horaInicio = hora.TTini;
                _horaFin = hora.TTfin;
            }
            fillStatus(dataGridView2,_horaInicio,_horaFin);
        }

        public static string getShift()
        {
            ///Configuraciones de Horarios de turnos
            #region Hours Settings
            shiftEntity hora = new shiftEntity();
            hora.horaActual = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));
            hora.TMini = Convert.ToDateTime(DateTime.Now.ToString("05:30:00"));
            hora.TMfin = Convert.ToDateTime(DateTime.Now.ToString("14:59:59"));
            hora.TTini = Convert.ToDateTime(DateTime.Now.ToString("15:00:00"));
            hora.TTfin = Convert.ToDateTime(DateTime.Now.ToString("23:59:59")); 
            #endregion
            if ((hora.horaActual >= hora.TMini)&&(hora.horaActual <=hora.TMfin))//si es turno mañana
            {
                return "TM";
            }
            else if ((hora.horaActual >= hora.TTini)&&(hora.horaActual <=hora.TTfin))//si es turno tarde
            {
                return "TT";
            }
            return "";
        }
    }
}
