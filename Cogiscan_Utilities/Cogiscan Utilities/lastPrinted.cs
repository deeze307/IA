using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cogiscan_Utilities
{
    class lastPrinted
    {
        public static string PartNumber;
        public static string Contenedor;
        public static string ContenedorInicial;
        public static string boton;
        public static string modelo;
        public static string lote;
        public static string Printer;
        public static string Descripcion="";
        public static string tipo;
        
        public static int Cantidad;
        public static int CantidadInicial;

        public static ComboBox cbModeloSM;
        public static ComboBox cbLoteSM;

        public static CheckBox chkReimp; 

        public static RadioButton rbtContg;
        public static RadioButton rbtSplitg;
        public static RadioButton rbtCont200dpi;
        public static RadioButton rbtSplit200dpi;
    }
}
