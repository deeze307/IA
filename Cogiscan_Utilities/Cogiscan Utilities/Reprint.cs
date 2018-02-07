using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cogiscan_Utilities.WebServices;

namespace Cogiscan_Utilities
{
    public partial class reprint : Form
    {
        public reprint()
        {
            InitializeComponent();
        }

        private void reprint_Load(object sender, EventArgs e)
        {
            lblstatus.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Reprint();
        }

        private void txtReprint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Reprint();
            }
        }

        private void Reprint()
        {
            string[] containerInfo = WS.getContainerInfo(txtReprint.Text);
            if ((containerInfo != null)&&(containerInfo.Count() != 0))
            {
                string partNumber = containerInfo[0];
                int qty = Convert.ToInt32(containerInfo[1].Replace(".0", ""));
                string container = txtReprint.Text;
                Impresion imp = new Impresion();
                lastPrinted.chkReimp.Checked = false;
                Conexion.insertInContainerDataBase(partNumber, container, qty,"REEL", lastPrinted.cbModeloSM.Text, lastPrinted.cbLoteSM.Text,'Y');
                imp.print(partNumber, container, qty, lastPrinted.cbModeloSM.Text, lastPrinted.cbLoteSM.Text, lastPrinted.boton, lastPrinted.Printer, lastPrinted.rbtContg, lastPrinted.rbtSplitg, lastPrinted.rbtCont200dpi, lastPrinted.rbtSplit200dpi, 0, container, lastPrinted.chkReimp);
            }
            else
            {
                lblstatus.ForeColor = Color.Red;
                lblstatus.Text = "El Elemento NO EXISTE";
            }
        }
    }
}
