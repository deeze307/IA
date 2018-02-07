using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cogiscan_Utilities.Entitys;
namespace Cogiscan_Utilities
{
    class Global
    {
        public static bool isLogged = false;
        public static string userLogged="";
        public static string tipoContenedor="REEL";
        public static string caller = "";//Formulario que solicita asignar un codigo a una division o creacion de material.
        public static string barcode = "";
        public static string contenedor = "";
        public static Form1 frm1 = new Form1();
        public static rawMaterialWatcher rmw = new rawMaterialWatcher();
        public static validateRM vrm = new validateRM();
        public static string loginType = "";

        public static void chkUserLogged()
        {
            
            TabControl tab = (TabControl)frm1.Controls["utils"];
            if (isLogged)
            {
                foreach (Control control in tab.Controls)
                {
                    if (control is TabPage)
                    {
                        control.Enabled = true;
                    }
                }
            }
            else
            {
                foreach (Control control in tab.Controls)
                {
                    if (control is TabPage)
                    {
                        control.Enabled = false;
                    }
                }
            }
        }
    }
}
