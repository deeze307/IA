using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IBM.Data.DB2;
using System.Configuration;

namespace Cogiscan_Utilities
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            LogIn();
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            txtUser.BackColor = Color.White;
            txtPass.BackColor = Color.White;
        }

        private void txtPass_TextChanged(object sender, EventArgs e)
        {
            txtUser.BackColor = Color.White;
            txtPass.BackColor = Color.White;
        }


        private void Login_Load(object sender, EventArgs e)
        {
            this.Text = (Global.loginType == "globalUser") ? "Iniciar Sesion" : "Usuario QA";
            lblStatus.Text = "";
            if ((Global.isLogged) && (Global.loginType == "globalUser" ))
            { changeLoggedProperties(); }
        }

        private void changeLoggedProperties()
        {
            txtUser.Text = Global.userLogged;
            txtUser.Enabled = false;
            txtPass.Enabled = false;
            btnIniciarSesion.Text = "Cerrar Sesion";

        }
        bool usr = true;
        bool pass = true;
        private void txtUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtUser.Text == "")
                {
                    txtUser.BackColor = Color.OrangeRed;
                    usr = false;
                    btnIniciarSesion.Text = "Iniciar Sesion";
                }
                else if (txtPass.Text == "")
                {
                    txtPass.BackColor = Color.OrangeRed;
                    pass = false;
                    btnIniciarSesion.Text = "Iniciar Sesion";
                }
                else if ((txtUser.Text!="") && (txtPass.Text!=""))
                {
                    usr = true;
                    pass = true;
                    LogIn();}
            }
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtUser.Text == "")
                {
                    txtUser.BackColor = Color.OrangeRed;
                    usr = false;
                    btnIniciarSesion.Text = "Iniciar Sesion";
                }
                else if (txtPass.Text == "")
                {
                    txtPass.BackColor = Color.OrangeRed;
                    pass = false;
                    btnIniciarSesion.Text = "Iniciar Sesion";
                }
                else if ((txtUser.Text != "") && (txtPass.Text != ""))
                {
                    usr = true;
                    pass = true;
                    LogIn(); }
            }
        }

        //Funcion para loguearse o desloguearse
        private void LogIn()
        {
            btnIniciarSesion.Enabled = false;
            btnIniciarSesion.Text = ((Global.isLogged) && (Global.loginType == "globalUser")) ? "Cerrando Sesion..." : "Espere...";
            LoginOut();
            loginTimer.Tick += new EventHandler(loginTimer_Tick);
            //loginTimer.Enabled = true;
            btnIniciarSesion.Enabled = true;
        }

        private void loginTimer_Tick(object sender, EventArgs e)
        {
            loginTimer.Enabled = false;
            loginTimer.Stop();
            
            
        }

        private void LoginOut()
        {
            if (btnIniciarSesion.Text == "Espere...")
            {
                
                if (usr && pass)
                {
                    if (DB2.ConexionDB2.queryUser(txtUser.Text,txtPass.Text))
                    {
                        if (Global.loginType =="globalUser")
                        { 
                            Global.isLogged = true;
                            Global.userLogged = txtUser.Text;
                            Form loginForm = this.Owner;
                            MenuStrip menuStrip = (MenuStrip)loginForm.Controls["menuStrip1"];
                            menuStrip.Items["login"].Text = txtUser.Text;
                            menuStrip.Items["login"].ForeColor = Color.Black;
                            Global.chkUserLogged();
                            this.Close();
                        }
                        else if (Global.loginType == "qcUser")
                        {
                            if (valQuser(txtUser.Text))
                            {
                                this.Hide();
                                Global.vrm.QCUser = txtUser.Text;
                                validar v = new validar();
                                v.ShowDialog();
                            }
                            else
                            {
                                lblStatus.ForeColor = Color.DarkRed;
                                lblStatus.Text = "El usuario no tiene Privilegios"; 
                                btnIniciarSesion.Enabled = true;
                                btnIniciarSesion.Text = "Iniciar Sesion";
                            }
                        }
                    }
                    else
                    {
                        lblStatus.ForeColor = Color.DarkRed;
                        lblStatus.Text = "Usuario o Contraseña invalidos";
                        btnIniciarSesion.Enabled = true;
                        btnIniciarSesion.Text = "Iniciar Sesion";
                        //MessageBox.Show("Usuario o Contraseña invalidos", "Error de autenticación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            else
            {
                if (Global.isLogged)
                {
                    Global.userLogged = "";
                    Form loginForm = this.Owner;
                    MenuStrip lblUser = (MenuStrip)loginForm.Controls["menuStrip1"];
                    lblUser.Items["login"].Text = "Iniciar Sesion";
                    lblUser.Items["login"].ForeColor = Color.Blue;
                    Global.isLogged = false;
                    this.Close();
                }
            }
        }

        private bool valQuser(string user)
        {
            List<string> listaUsers = Conexion.queryQuser();
            foreach (string _user in listaUsers)
            {
                if (_user == user) return true;
            }
            return false;
        }
    }
}
