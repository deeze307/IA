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
    public partial class asignTag : Form
    {
        public asignTag()
        {
            InitializeComponent();
        }

        private bool chkifNotEmpty(string texto)
        {
            if (texto != "") { return true; }
            else {return false;}
        }

        private void asignTag_Load(object sender, EventArgs e)
        {
            if (Global.caller == "split")
            { txtBarcode.Visible = false; }
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (chkifNotEmpty(txtBarcode.Text)))
            {
                Global.barcode = txtBarcode.Text;
                if (Global.barcode.StartsWith(")"))
                { Global.barcode = Global.barcode.Replace(")", "("); }
                this.Hide();
            }
            else if ((e.KeyCode == Keys.Enter) && (!chkifNotEmpty(txtBarcode.Text)))
            {
                MessageBox.Show("Debe asignar un código para hacer referencia al material", "Error de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void txtContenedor_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (chkifNotEmpty(txtContenedor.Text))&&(txtBarcode.Visible))
            {
                lastPrinted.Contenedor = txtContenedor.Text;
                txtBarcode.Enabled = true;
                txtBarcode.Focus();
            }
            else if ((e.KeyCode == Keys.Enter) && (!chkifNotEmpty(txtContenedor.Text))&&(txtBarcode.Visible))
            {
                MessageBox.Show("Debe asignar un Contenedor para hacer referencia al material", "Error de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (!txtBarcode.Visible)
            {
                Global.barcode = txtContenedor.Text;
                if (Global.barcode.StartsWith(")"))
                { Global.barcode = Global.barcode.Replace(")", "("); }
                this.Hide();
            }
        }
    }
}
