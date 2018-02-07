using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cogiscan_Utilities
{
    public partial class validar : Form
    {
        public validar()
        {
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (WebServices.WS.queryItem(textBox1.Text))
                {
                    lblStatus.Text = (DB2.ConexionDB2.UnlockQuarantine(textBox1.Text))? "Desbloqueo Exitoso" : "Ocurrió un Error";
                    textBox1.Text = "";
                }
                else
                {
                    lblStatus.ForeColor = Color.Coral;
                    lblStatus.Text = "Item "+textBox1.Text+" Desconocido";
                }
            }
        }

        private void validar_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "";
        }
    }
}
