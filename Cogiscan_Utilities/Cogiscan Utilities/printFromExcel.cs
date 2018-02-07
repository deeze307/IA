using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Cogiscan_Utilities.Clases;
using Cogiscan_Utilities.WebServices;

namespace Cogiscan_Utilities
{
    public partial class printFromExcel : Form
    {
        public printFromExcel()
        {
            InitializeComponent();
        }
        private string Excel13ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
        private void button1_Click(object sender, EventArgs e)
        {
            importador.ShowDialog();
        }

        private void importador_FileOk(object sender, CancelEventArgs e)
        {
            string ruta = importador.FileName;
            string extension = Path.GetExtension(ruta);
            string conStr, sheetName;
            conStr = string.Empty;
            conStr = string.Format(Excel13ConString,ruta,"NO");
            //Get the name of the First Sheet.
            using (OleDbConnection con = new OleDbConnection(conStr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.Connection = con;
                    con.Open();
                    DataTable dtExcelSchema = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    //sheetName = dtExcelSchema.Rows[0]["prueba$"].ToString();
                    con.Close();
                }
            }
            //Read Data from the First Sheet.
            using (OleDbConnection con = new OleDbConnection(conStr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    using (OleDbDataAdapter oda = new OleDbDataAdapter())
                    {
                        DataTable dt = new DataTable();
                        cmd.CommandText = "SELECT * From [Hoja1$]";
                        cmd.Connection = con;
                        con.Open();
                        oda.SelectCommand = cmd;
                        oda.Fill(dt);
                        con.Close();

                        //Populate DataGridView.
                        dataGridView1.DataSource = dt;
                        label1.Text = dt.Rows.Count.ToString() +" Items";
                        progressBar1.Step = Convert.ToInt32(dt.Rows.Count);
                        progressBar1.Maximum = Convert.ToInt32(dt.Rows.Count);
                        progressBar1.Value = 0;
                    }
                }
            }

        }

        private void printFromExcel_Load(object sender, EventArgs e)
        {
            label1.Text = "";
            lblStat.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string id = "";
            string partNumber = "";
            int qty = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (chkID.Checked)
                {

                }
                else
                {
                    id = row.Cells[0].Value.ToString();
                    partNumber = row.Cells[1].Value.ToString();
                    qty = Convert.ToInt32(row.Cells[2].Value.ToString());
                    WS.initRawMaterial(partNumber, id, qty, "REEL", "INVENTARIO", "2015");
                }
                progressBar1.Value = progressBar1.Value + 1;
            }
            lblStat.Text = "Completado";
        }
    }
}
