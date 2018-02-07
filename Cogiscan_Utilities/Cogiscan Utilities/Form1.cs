using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using Cogiscan_Utilities.WebServices;
using Cogiscan_Utilities.DB2;
using Cogiscan_Utilities.Clases;
using Cogiscan_Utilities.Entitys;
using Microsoft.VisualBasic;
using System.Threading;

namespace Cogiscan_Utilities
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //Chequeo la version de la aplicación
            this.Text = AssemblyInfo.appInfo();
            //Chequeo si el cliente ya se encuentra en la tabla HeartBreath
            Conexion.checkClientOnHeartBreathTable();
            //Obtengo la impresora predeterminada
            PrinterSettings defaultPrinter = new PrinterSettings();
            txtPrinterContainer.Text = defaultPrinter.PrinterName;
            txtPrinterSplit.Text = defaultPrinter.PrinterName;
            lastPrinted.Printer = txtPrinterSplit.Text;
            lblContenedor.Text = Conexion.queryLastInsert();
            lblNuevoContenedor.Text = "";
            ultimaInsercion.Tick+= new EventHandler(getLastInsert);
            ultimaInsercion.Enabled = true;
            txtInitContainer.Select();

        }

        private void getLastInsert(object source, EventArgs e)
        {
            lblContenedor.Text = Conexion.queryLastInsert();
            Conexion.insertHearthBreath();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HostAndUser.host();
            Global.chkUserLogged();//chequeo si hay algun usuario logueado
            lblresultado.Text = "";
            lblResultSplit.Text = "";
            lblPNSplit.Text = "";
            lblQtySplit.Text = "";
            //WS.queryTooling();
            List<string> dataSource = new List<string>();
            dataSource = Conexion.queryModelos();
            cbModeloGC.DataSource = dataSource;
            cbModeloSM.DataSource = dataSource;
            cbTipoCG.SelectedIndex = 0;// por defecto selecciono el tipo REEL
            //****LOAD TAB****//
            lblSelectedContainer.Text = "";
            lblSelectedLocation.Text = "";
            lblStatusLoad.Text = "";
            btnDelete.Enabled = false;
            btnDeleteAll.Enabled = false;
            ////////////////////

            string partNumber = "T2SC3332-S--";
            string partNumberToRead = "";
            if (partNumber.Contains("-"))
            {
                string[] splitted = partNumber.Split('-');
                for (int i=0; i < splitted.Length;i++)
                {
                    if(i < (splitted.Length -1))
                    {
                        if (splitted[i] =="")
                        {
                            if (splitted[i-1] =="")
                            {
                                partNumberToRead = partNumberToRead + "-";
                            }
                            else
                            {
                                partNumberToRead = partNumberToRead + "-";
                            }
                        }
                        else
                        {
                            if (Char.IsLetter(splitted[i], splitted[i].Length - 1))
                            {
                                partNumberToRead = partNumberToRead + splitted[i] + "->5";
                            }
                            else if (Char.IsNumber(splitted[i], splitted[i].Length - 1))
                            {
                                partNumberToRead = partNumberToRead + splitted[i] + ">6->5";
                            }
                        }
                    }
                    else
                    {
                        partNumberToRead = partNumberToRead + splitted[i];
                    }
                }
                partNumberToRead = "";
                //Este es para el caso "Letra - Nro - Letra/Nro"
                string pattern = @"(?'letra'[A-Z]\-)(?'centro'\w+)(?'digito'\d)(\-)";
                string replacement = "${letra}>5${centro}${digito}>6->5";
                Regex rgx = new Regex(pattern);
                partNumberToRead = rgx.Replace(partNumberToRead, replacement);
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            if (Global.isLogged)
            {
                Imprimir(lblContenedor.Text, txtPN.Text, txtQTY.Text, cbModeloGC.Text, cbLoteGC.Text, btnGenerar.Name,cbTipoCG.Text,txtLotCode.Text,txtDateCode.Text);
                clearRes.Tick += new EventHandler(clearResult);
                clearRes.Enabled = true;
            }
            else { MessageBox.Show("Debe loguearse antes de imprimir", "No se detecto usuario logueado"); }
        }

        public void Imprimir(string Contenedorinit,string partNumber,string qty, string modelo,string lote, string boton, string tipoContenedor, string LotCode, string DateCode)
        {
            try
            {   
                if (chkTxtBox())
                {
                    if (WS.chkPN(txtPN.Text))
                    {
                        Thread.Sleep(1000);
                        Conexion.queryDesc(partNumber, modelo, lote);
                        if (lastPrinted.Descripcion != "")
                        {
                            btnReCont.Visible = true;
                            lastPrinted.Contenedor = incrementarContenedorIRM(Contenedorinit);
                            lastPrinted.Cantidad = Convert.ToInt32(qty);
                            lastPrinted.PartNumber = partNumber;
                            lastPrinted.modelo = modelo;
                            lastPrinted.lote = lote;
                            lastPrinted.tipo = tipoContenedor;
                            lastPrinted.boton = boton;
                            if (lastPrinted.tipo == "TRAY")
                            {
                                asignTag at = new asignTag();
                                at.ShowDialog();
                            }
                            lblresultado.Text = WS.initRawMaterial(lastPrinted.PartNumber, lastPrinted.Contenedor, lastPrinted.Cantidad, lastPrinted.tipo,lastPrinted.modelo,lastPrinted.lote);
                            ConexionDB2.SetQuarantine(lastPrinted.Contenedor,"Inicialización de Material");
                            //Para imprimir
                            Impresion imp = new Impresion();
                            if (lastPrinted.tipo == "REEL")
                            { imp.print(partNumber, lastPrinted.Contenedor, lastPrinted.Cantidad, lastPrinted.modelo, lastPrinted.lote, lastPrinted.boton, txtPrinterContainer.Text, rbtContg, rbtSplitg, rbtCont200dpi, rbtSplit200dpi, 0, "", chkReimp); }
                            txtPN.Text = "";
                            txtQTY.Text = "";
                            txtDateCode.Text = "";
                            txtLotCode.Text = "";
                            txtPN.Focus();
                        }
                        else
                        {
                            lblresultado.ForeColor = Color.DarkRed;
                            lblresultado.Text = "el PartNumber "+partNumber+" no corresponde al Modelo: '"+modelo+"' - Lote: '"+lote+"'"; 
                        }
                    }
                    else
                    {
                        MessageBox.Show("El PartNumber " + partNumber + " no existe", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Debe completar todos los campos", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            { Conexion.insertErrorInDB(ex.ToString()); }
        }

        private void btnDividir_Click(object sender, EventArgs e)
        {
            if (Global.isLogged)
            {
                btnReSplit.Visible = true;
                lastPrinted.PartNumber = lblPNSplit.Text;
                lastPrinted.Cantidad = Convert.ToInt32(txtQTYsplit.Text);
                lastPrinted.ContenedorInicial = txtInitContainer.Text;
                string lblInicialFormateado = lblQtySplit.Text.Replace(".0", "");
                lastPrinted.CantidadInicial = Convert.ToInt32(lblInicialFormateado);
                try
                { lastPrinted.Descripcion = Conexion.queryDesc(lastPrinted.PartNumber, cbModeloSM.Text, cbLoteSM.Text); }
                catch (Exception ex)
                {
                    Conexion.insertErrorInDB(ex.ToString());
                }
                if (chkTxtBox())
                {
                    if (chkQtyLoop(lastPrinted.Cantidad, lastPrinted.CantidadInicial, Convert.ToInt32(numericUpDown1.Value.ToString())))//chequeo que la cantidad de repeticiones de divisiones no supere la cantidad del contenedor
                    {
                        if (containerId != "" && locationInContainer != "")
                        { WS.unLoadContentId(txtInitContainer.Text, containerId, locationInContainer); }
                        if (Global.tipoContenedor == "REEL")
                        { lastPrinted.Contenedor = incrementarContenedorSRM(); }
                        else if (Global.tipoContenedor == "TRAY")
                        {
                            Global.caller = "split";
                            asignTag at = new asignTag();
                            at.ShowDialog();
                        }
                        utils.Enabled = false;
                        if (Convert.ToInt32(numericUpDown1.Value.ToString()) > 1)
                        { chkReimp.Checked = false; }
                        for (int i = 1; i <= Convert.ToInt32(numericUpDown1.Value.ToString());i++)//dependiendo de la cantidad de repeticiones hago las impresiones
                        {
                            lblResultSplit.Text = WS.splitRawMaterial(txtInitContainer.Text, lastPrinted.CantidadInicial, lastPrinted.Cantidad, lastPrinted.Contenedor, cbModeloSM.Text, cbLoteSM.Text);
                            if (lblResultSplit.Text == "Operación Exitosa")
                            {
                                lblNuevoContenedor.Text = lastPrinted.Contenedor;
                                if (Global.caller != "split")//Para imprimir
                                {
                                    ConexionDB2.SetQuarantine(lastPrinted.Contenedor, "Division de Material");
                                    Impresion imp = new Impresion();
                                    imp.print(lastPrinted.PartNumber, lastPrinted.Contenedor, lastPrinted.Cantidad, cbModeloSM.Text, cbLoteSM.Text, btnDividir.Name, txtPrinterSplit.Text, rbtContg, rbtSplitg, rbtCont200dpi, rbtSplit200dpi, lastPrinted.CantidadInicial, lastPrinted.ContenedorInicial, chkReimp);
                                    lastPrinted.Contenedor = incrementarContenedorSRM();
                                }
                                lastPrinted.CantidadInicial++;
                            }
                        }
                        if (lblResultSplit.Text == "Operación Exitosa")
                        { WS.LoadContent(txtInitContainer.Text, containerId, locationInContainer); }//Luego de dividir el material, lo vuelvo a cargar en la ubicacion donde se encontraba.
                        utils.Enabled = true;
                        txtQTYsplit.Text = "";
                        lblPNSplit.Text = "";
                        lblQtySplit.Text = "";
                        txtInitContainer.Text = "";
                        txtInitContainer.Focus();
                        numericUpDown1.Value = 1;
                        clearRes.Tick += new EventHandler(clearResult);
                        clearRes.Enabled = true;
                    }
                    else
                    { MessageBox.Show(
                        "La cantidad de materiales divididos por cantidad de impresiones supera el total de materiales disponibles" +
                        "\n" +
                        "Materiales requeridos para dividir: " + lastPrinted.Cantidad * Convert.ToInt32(numericUpDown1.Value.ToString())+
                        "\n" +
                        "Materiales disponibles para dividir: " + lastPrinted.CantidadInicial +
                        "\n" +
                        "Diferencia: " + (lastPrinted.CantidadInicial - (lastPrinted.Cantidad * Convert.ToInt32(numericUpDown1.Value.ToString()))),
                        "Error al intentar dividir",
                        MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
                else
                {
                    MessageBox.Show("Debe completar todos los campos", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else { MessageBox.Show("Debe Iniciar su Sesion antes de imprimir","No se detecto usuario logueado",MessageBoxButtons.OK,MessageBoxIcon.Exclamation); }
        }

        private bool chkQtyLoop(int cantidadSplit, int cantidadInicial, int cantidadLoop)
        {
            if (cantidadInicial >= (cantidadSplit * cantidadLoop))
            { return true; }
            else
            { return false; }
        }
        private void clearResult(object source, EventArgs e)
        {
            lblresultado.Text = "";
            lblResultSplit.Text = "";
            lblNuevoContenedor.Text = "";
            clearRes.Enabled = false;
        }

        private bool chkTxtBox()
        {
            //Valido que los campos no esten vacios según el Tab en el cual se esté posicionado
            if ((utils.SelectedTab == utils.TabPages["split"]) && (txtInitContainer.Text == "" || txtQTYsplit.Text == "")) return false;
            else if ((utils.SelectedTab == utils.TabPages["codeGenerator"]) && (txtPN.Text == "" || txtQTY.Text == "" || cbModeloGC.Text == "-Seleccione-" || cbLoteGC.Text == "-Seleccione-")) return false;
            return true;
        }

        private void txtQTY_KeyPress(object sender, KeyPressEventArgs e)
        {
            //para validar que solo se ingresen números
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private string incrementarContenedorIRM(string contenedor)
        {
            string[] splitted = Regex.Split(contenedor, @"(?<=\p{L})(?=\p{N})")
            .Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            string letras = splitted[0];
            int numeros = Convert.ToInt32(splitted[1]) + 1;
            string contenedorFormateado="";
            if (numeros < 10)
            {
                contenedorFormateado = letras+"0000" + numeros;
            }
            else if (numeros >= 10 && numeros < 100)
            {
                contenedorFormateado = letras + "000" + numeros;
            }
            else if (numeros >= 100 && numeros < 1000)
            {
                contenedorFormateado = letras + "00" + numeros;
            }
            else if (numeros >= 1000 && numeros < 10000)
            {
                contenedorFormateado = letras + "0" + numeros;
            }
            else if (numeros >= 10000)
            {
                contenedorFormateado = letras + numeros;
            }
            
            return contenedorFormateado;
        }

        private string incrementarContenedorSRM()
        {
            string letras;
            int numeros;
            string lastContainer;
            lastContainer = Conexion.queryLastInsertSRM();
            if (lastContainer.Contains("CGSS."))
            {
                //utilizo expresiones regulares para separar el texto de los numeros
                string[] splitted = Regex.Split(lastContainer, @"(?<=\p{L}.)(?=\p{N})")
                .Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                letras = splitted[0];
                numeros = Convert.ToInt32(splitted[1]) + 1;
            }
            else
            {
                letras = "CGSS.";
                numeros= 1;
            }

            string contenedorFormateado = "";
            if (numeros < 10)
            {
                contenedorFormateado = letras + "0000" + numeros;
            }
            else if (numeros >= 10 && numeros < 100)
            {
                contenedorFormateado = letras + "000" + numeros;
            }
            else if (numeros >= 100 && numeros < 1000)
            {
                contenedorFormateado = letras + "00" + numeros;
            }
            else if (numeros >= 1000 && numeros < 10000)
            {
                contenedorFormateado = letras + "0" + numeros;
            }
            else if (numeros >= 10000)
            {
                contenedorFormateado = letras + numeros;
            }

            return contenedorFormateado;

        }

        private void lblresultado_TextChanged(object sender, EventArgs e)
        {
            if (lblresultado.Text == "Operación Exitosa") lblresultado.ForeColor = Color.Green;
            else lblresultado.ForeColor = Color.Red;
        }

        private void btnSelectPrinterContainer_Click_1(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == DialogResult.OK)
            {
                txtPrinterContainer.Text = pd.PrinterSettings.PrinterName;
                txtPrinterSplit.Text = pd.PrinterSettings.PrinterName;
                lastPrinted.Printer = pd.PrinterSettings.PrinterName;
            }
        }

        private void txtPrinterContainer_TextChanged_1(object sender, EventArgs e)
        {

            if (txtPrinterContainer.Text == "")
            {
                txtPN.Enabled = false;
                txtQTY.Enabled = false;
            }
            else
            {
                txtPN.Enabled = true;
                txtQTY.Enabled = true;
            }
        }

        private void btnSelectPrinterSplit_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == DialogResult.OK)
            {
                lastPrinted.Printer = pd.PrinterSettings.PrinterName;
                txtPrinterContainer.Text = lastPrinted.Printer;
                txtPrinterSplit.Text = lastPrinted.Printer;

            }
        }

        private void lblResultSplit_TextChanged(object sender, EventArgs e)
        {
            if (lblResultSplit.Text == "Operación Exitosa") lblResultSplit.ForeColor = Color.Green;
            else lblResultSplit.ForeColor = Color.Red;
        }

        public string containerId;
        public string locationInContainer;

        private void txtInitContainer_Leave(object sender, EventArgs e)
        {   
            if (txtInitContainer.Text != "")
            {
                lblResultSplit.ForeColor = Color.DarkGray;
                lblResultSplit.Text = "Espere Por Favor...";
                findContainer.Tick+=new EventHandler(findContainer_Tick);
                findContainer.Enabled = true;
            }           
        }

        private void utils_Selected(object sender, TabControlEventArgs e)
        {
            if (utils.SelectedTab == utils.TabPages["split"])
            { txtInitContainer.Select(); }
            else { txtPN.Select(); }
        }

        private void cbModelo_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbLoteGC.DataSource = Conexion.queryLotes(cbModeloGC.Text);
            cbLoteGC.Enabled = true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            btnRefreshGC.Enabled = false;
            cbModeloGC.DataSource = Conexion.queryModelos();
            btnRefreshGC.Enabled = true;
        }

        private void cbModeloSM_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbLoteSM.DataSource = Conexion.queryLotes(cbModeloSM.Text);
            cbLoteSM.Enabled = true;
        }

        private void printExcel_Click(object sender, EventArgs e)
        {
            printFromExcel pfe = new printFromExcel();
            pfe.ShowDialog();
        }

        private void cbLoteGC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLoteGC.Text != "-Seleccione-")
            {
                printExcel.Enabled = true;
            }
        }

        public string changeUserLogged
        {
            get
            {
                return this.login.Text;
            }
            set
            {
                this.login.Text = value;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult res = MessageBox.Show("¿Está seguro que desea salir de la aplicación?", "Saliendo...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.No) e.Cancel = true;
        }

        private void btnReSplit_Click_1(object sender, EventArgs e)
        {    
            DialogResult res = MessageBox.Show("¿Está seguro que desea reimprimir la etiqueta " + lastPrinted.Contenedor + "?", "Confirmación de Reimpresion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Impresion imp = new Impresion();
                imp.print(lastPrinted.PartNumber, lastPrinted.Contenedor, lastPrinted.Cantidad, cbModeloSM.Text, cbLoteSM.Text, btnDividir.Name, txtPrinterSplit.Text, rbtContg, rbtSplitg, rbtCont200dpi, rbtSplit200dpi, lastPrinted.CantidadInicial, lastPrinted.ContenedorInicial, chkReimp);
            }
        }

        private void txtInitContainer_TextChanged(object sender, EventArgs e)
        {
            if (txtInitContainer.Text != "")
            { btnReSplit.Visible = false; }
        }

        private void txtPN_TextChanged(object sender, EventArgs e)
        {
            if (txtPN.Text != "")
            { btnReCont.Visible = false; }
        }

        private void btnReCont_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("¿Está seguro que desea reimprimir la etiqueta " + lastPrinted.Contenedor + "?", "Confirmación de Reimpresion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Impresion imp = new Impresion();
                imp.print(lastPrinted.PartNumber, lastPrinted.Contenedor, lastPrinted.Cantidad, lastPrinted.modelo, lastPrinted.lote, lastPrinted.boton, txtPrinterContainer.Text, rbtContg, rbtSplitg, rbtCont200dpi, rbtSplit200dpi, 0, "", chkReimp);
            }
        }

        private void findContainer_Tick(object sender, EventArgs e)
        {
            string[] resultInfo;
            resultInfo = WS.getContainerInfo(txtInitContainer.Text);
            if (resultInfo != null)
            {
                btnDividir.Enabled = true;
                lblPNSplit.Text = resultInfo[0];
                lblQtySplit.Text = resultInfo[1];
                if (resultInfo.Count() == 4)
                {
                    containerId = resultInfo[2];
                    locationInContainer = resultInfo[3];
                }
                else if (resultInfo.Count() == 3)
                {
                    containerId = resultInfo[2];
                }
                lblResultSplit.Text = "";
            }
            else
            {
                lblResultSplit.ForeColor = Color.Red;
                lblResultSplit.Text = "El PartNumber NO EXISTE o no corresponde al Modelo-Lote";
                btnDividir.Enabled = false;
                lblPNSplit.Text = "";
                lblQtySplit.Text = "";
            }
            findContainer.Enabled = false;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Global.loginType = "globalUser";
            Login login = new Login();
            login.Show(this);
        }

        private void reimprimirManualmenteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lastPrinted.boton = btnDividir.Name;
            lastPrinted.rbtCont200dpi = rbtCont200dpi;
            lastPrinted.rbtSplit200dpi = rbtSplit200dpi;
            lastPrinted.rbtSplitg = rbtSplitg;
            lastPrinted.rbtContg = rbtContg;
            lastPrinted.chkReimp = chkReimp;
            lastPrinted.cbModeloSM = cbModeloSM;
            lastPrinted.cbLoteSM = cbLoteSM;
            reprint rp = new reprint();
            rp.ShowDialog();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            containerLocation.db_accepted_cntr = Conexion.queryAcceptedContainer();
            containerLocation.db_accepted_raw_material = Conexion.queryAcceptedRawMaterial();
            if (e.KeyCode == Keys.Enter)
            {
                if (containerLocation.flag && txtAddContainer.Text.Length > 2)
                {
                    if (validarInput(txtAddContainer.Text))
                    {
                        if ((txtAddContainer.Text.StartsWith("LOCA")||(txtAddContainer.Text.StartsWith("ITEM"))))
                        {
                            getContainerLocation(txtAddContainer.Text);
                        }
                        else
                        {
                            lblStatusLoad.Text = "Ubicación " + txtAddContainer.Text + " Invalida";
                            lblStatusLoad.ForeColor = Color.DarkRed;
                            txtAddContainer.Text = "";
                        }
                    }
                    else
                    {
                        string primerosCaracteres = txtAddContainer.Text.Substring(0, 3);
                        var resultado = Array.Find(containerLocation.db_accepted_raw_material, item => item.Equals(primerosCaracteres));
                        if (resultado == null)
                        {
                            if (WS.getContents(txtAddContainer.Text))
                            {
                                string input = Interaction.InputBox("", "Ingrese Ubicación en el Contenedor " + txtAddContainer.Text);
                                containerLocation.cntr_name = txtAddContainer.Text;
                                containerLocation.cntr_location = input;
                                loadInContainer.containerToLoad = containerLocation.cntr_name + " " + containerLocation.cntr_location;
                                getContainerLocation(txtAddContainer.Text);
                            }
                            else
                            {
                                lblStatusLoad.Text = "Ubicación " + txtAddContainer.Text + " Invalida";
                                lblStatusLoad.ForeColor = Color.DarkRed;
                                txtAddContainer.Text = "";
                            }
                        }
                        else
                        {
                            lblStatusLoad.Text = txtAddContainer.Text + " Es un material, no una ubicación";
                            lblStatusLoad.ForeColor = Color.DarkRed;
                            txtAddContainer.Text = "";
                        }
                    }
                }
                else if (!containerLocation.flag)
                {
                    try
                    {
                        if (txtAddContainer.Text.Length > 2 && dgContainer.Enabled == true && validarInput(txtAddContainer.Text))
                        {
                            if (chkNotIntDataGrid(txtAddContainer.Text))
                            {
                                containerLocation.containerInfo = WS.getContainerInfo(txtAddContainer.Text);
                                if (containerLocation.containerInfo != null)
                                {
                                    dgContainer.Rows.Add(txtAddContainer.Text);
                                    dgContainer.CurrentCell = dgContainer.Rows[dgContainer.Rows.Count - 1].Cells[0];

                                    dgContainer.Rows[dgContainer.CurrentCell.RowIndex].Cells[1].Value = containerLocation.containerInfo[0]; //agrego el PN
                                    dgContainer.Rows[dgContainer.CurrentCell.RowIndex].Cells[2].Value = containerLocation.containerInfo[1].Replace(".0", ""); //agrego la Cantidad
                                    lblCounter.Text = dgContainer.Rows.Count.ToString();
                                    btnDelete.Enabled = true;
                                    btnDeleteAll.Enabled = true;
                                    txtAddContainer.Text = "";
                                }
                                else
                                {
                                    txtAddContainer.SelectAll();
                                }
                            }
                        }
                        else
                        {
                            txtAddContainer.SelectAll();
                        }
                    }
                    catch (Exception ex)
                    {
                        Conexion.insertErrorInDB(ex.ToString());
                    }
                }
                else if (txtAddContainer.Text.Length < 3)
                {
                    lblStatusLoad.Text = "Código demasiado corto";
                    lblStatusLoad.ForeColor = Color.DarkRed;
                }
            }
        }

        private bool chkNotIntDataGrid(string input)
        {
            if (dgContainer.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgContainer.Rows)
                {
                    string elemento = row.Cells[0].Value.ToString();
                    if (input == elemento)
                    {
                        txtAddContainer.BackColor = Color.LightCoral;
                        txtAddContainer.SelectAll();
                        return false;
                    }
                }
            }
            return true;
        }

        private void getContainerLocation(string location)
        {
            int limit = location.Length - 4;
            string locationKey = location.Substring(4, limit);
            if (location.StartsWith("LOCA"))
            {
                if (ConexionDB2.queryLocation(locationKey))
                {
                    fillLocation();
                }
            }
            else if (location.StartsWith("ITEM"))
            {
                if (ConexionDB2.queryItemInfo(locationKey))
                {
                    fillLocation();
                }
            }
            else
            {
                fillLocation();
            }
        }

        private void fillLocation()
        {
            lblSelectedContainer.Text = containerLocation.cntr_name;
            lblSelectedLocation.Text = containerLocation.cntr_location;
            txtAddContainer.Text = "";
            dgContainer.Enabled = true;
            lblStatusLoad.Text = "Escanee Nuevamente La Ubicación " + containerLocation.cntr_name +" " + containerLocation.cntr_location + " \npara finalizar";
            lblStatusLoad.ForeColor = Color.Blue;
            containerLocation.flag = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            dgContainer.Rows.RemoveAt(dgContainer.CurrentRow.Index);
            txtAddContainer.Focus();
            lblCounter.Text = dgContainer.Rows.Count.ToString();
        }

        private bool validarInput(string texto)
        {
            string primerosCaracteres = texto.Substring(0, 3);
            var resultado = Array.Find(containerLocation.db_accepted_raw_material, item => item.Equals(primerosCaracteres));
            if (resultado != null)
            {
                if (!containerLocation.flag) return true;
                else return false;
            }
            else
            {
                if ((containerLocation.flag) && ((primerosCaracteres == "LOC") || (primerosCaracteres == "ITE")))
                {

                    loadInContainer.containerToLoad = txtAddContainer.Text;
                    return true;
                }
                else if ((!containerLocation.flag) && (txtAddContainer.Text == loadInContainer.containerToLoad))
                {
                    if (Global.isLogged)
                    {
                        txtAddContainer.Text = "";
                        if (loadRawMaterial())
                        {
                            lblSelectedContainer.Text = "";
                            lblSelectedLocation.Text = "";
                            dgContainer.Rows.Clear();
                            lblStatusLoad.Text = "Operación Exitosa";
                            lblStatusLoad.ForeColor = Color.DarkGreen;
                            containerLocation.flag = true;
                            txtAddContainer.BackColor = Color.White;
                            btnDelete.Enabled = false;
                            btnDeleteAll.Enabled = false;
                            return false;
                        }

                        else
                        {
                            lblStatusLoad.Text = "Ocurrió un problema al intentar Cargar los elementos";
                            lblStatusLoad.ForeColor = Color.DarkRed;
                        }
                        return true;
                    }
                    else { MessageBox.Show("Debe Loguearse antes de Cargar los Materiales", "No se detecto usuario logueado"); }
                }
            }
            return false;
        }

        private bool loadRawMaterial()
        {
            lblStatusLoad.Text = "Espere por favor...";
            foreach (DataGridViewRow row in dgContainer.Rows)
            {
                if (!WS.loadInContainer(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), containerLocation.cntr_name, containerLocation.cntr_location))
                {return false;}
            }
            
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult res =  MessageBox.Show("¿Está seguro que desea eliminar todos los Items de la tabla?","Atención",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                dgContainer.Rows.Clear();
                btnDelete.Enabled = false;
                btnDeleteAll.Enabled = false;
                txtAddContainer.Focus();
                lblCounter.Text = dgContainer.Rows.Count.ToString();
            }

        }

        private void estadoDeMaterialesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("En Desarrollo...","Atención",MessageBoxButtons.OK,MessageBoxIcon.Information);
            //if (Global.rmw.Visible != true)
            //{
            //    Global.rmw.Show(this);
            //    estadoDeMaterialesToolStripMenuItem.Text = "Cerrar Estado de Materiales";
            //}
            //else
            //{
            //    Global.rmw.Hide();
            //    estadoDeMaterialesToolStripMenuItem.Text = "Estado de Materiales";

            //}
        }

        private void rbtSplitc_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtSplitc.Checked)
            { rbtContc.Checked = true; }
            else
            { rbtContc.Checked = false; }
        }

        private void rbtSplit200dpi_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtSplit200dpi.Checked)
            { rbtCont200dpi.Checked = true; }
            else
            { rbtCont200dpi.Checked = false; }
        }

        private void rbtContc_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtContc.Checked)
            { rbtSplitc.Checked = true; }
            else
            { rbtSplitc.Checked = false; }
        }

        private void rbtCont200dpi_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtCont200dpi.Checked)
            { rbtSplit200dpi.Checked = true; }
            else
            { rbtSplit200dpi.Checked = false; }
        }

        private void cbTipoSM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTipoSM.Text == "REEL")
                Global.tipoContenedor = "REEL";
            else
                Global.tipoContenedor = "TRAY";
        }

        private void validarMaterialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Global.loginType = "qcUser";
            Login li = new Login();
            li.ShowDialog();
        }

  
    }
}
