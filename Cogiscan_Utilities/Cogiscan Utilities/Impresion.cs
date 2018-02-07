using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Cogiscan_Utilities
{
    class Impresion
    {
        bool flag = true;
        public void print(string partNumber, string contenedor, int qty, string modelo, string lote, string boton, string impresora, RadioButton rbtContg, RadioButton rbtSplitg,RadioButton rbtCont200dpi,RadioButton rbtSplit200dpi,int qtySplit, string contSplit,CheckBox chkReimp)
        {
            string partNumberToRead = "";
            if (lastPrinted.Descripcion == "")
            { Conexion.queryDesc(partNumber, modelo, lote); }
            if (partNumber.Contains("-"))
            {
                string[] splitted = partNumber.Split('-');
                for (int i = 0; i < splitted.Length; i++)
                {
                    if (i < (splitted.Length - 1))
                    {
                        if (splitted[i] == "")
                        {
                            if (splitted[i - 1] == "")
                            {
                                partNumberToRead = partNumberToRead + "-";
                            }
                            else
                            {
                                partNumberToRead = partNumberToRead + "--";
                            }
                        }
                        else
                        {
                            partNumberToRead = partNumberToRead + splitted[i];
                            if (Char.IsLetter(splitted[i], splitted[i].Length - 1))
                            {
                                if (splitted[i + 1] != "")
                                {
                                    partNumberToRead = partNumberToRead + "->5";
                                }
                            }
                            else if (Char.IsNumber(splitted[i], splitted[i].Length - 1))
                            {
                                if (splitted[i + 1] != "")
                                {
                                    if (Char.IsLetter(splitted[i + 1], 0))
                                    {
                                        partNumberToRead = partNumberToRead + ">6-";
                                    }
                                    else
                                    {
                                        partNumberToRead = partNumberToRead + ">6->5";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        partNumberToRead = partNumberToRead + splitted[i];
                    }
                }
                //partNumberToRead = "";
                ////Este es para el caso "Letra - Nro - Letra/Nro"
                //string pattern = @"(?'letra'[A-Z]\-)(?'centro'\w+)(?'digito'\d)(\-)";
                //string replacement = "${letra}>5${centro}${digito}>6->5";
                //Regex rgx = new Regex(pattern);
                //partNumberToRead = rgx.Replace(partNumberToRead, replacement);
            }
            else
            {
                partNumberToRead = partNumber;
            }
            #region Zebra GK420t 203dpi Etiqueta Grande
            string printStringGrande200dpi = "";
            //printStringGrande200dpi += @"^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR2,2~SD26^JUS^LRN^CI0^XZ";
            printStringGrande200dpi += @"^XA";
            //printStringGrande200dpi += @"^MMT";
            printStringGrande200dpi += @"^PW799";
            printStringGrande200dpi += @"^LL0400";
            printStringGrande200dpi += @"^LS0";
            printStringGrande200dpi += @"^FO576,125^GB218,132,8^FS";
            printStringGrande200dpi += @"^FO6,125^GB177,132,8^FS";
            printStringGrande200dpi += @"^FO4,126^GB790,132,8^FS";
            printStringGrande200dpi += @"^FO5,5^GB789,129,8^FS";
            printStringGrande200dpi += @"^FO5,5^GB789,389,8^FS";
            printStringGrande200dpi += @"^FT18,223^A0N,96,88^FH\^FDCGS^FS";
            printStringGrande200dpi += @"^FT221,240^A0N,34,21^FH\^FDCANTIDAD^FS";
            printStringGrande200dpi += @"^FT317,243^A0N,51,48^FH\^FD" + qty + "^FS";
            printStringGrande200dpi += @"^FT21,50^A0N,28,28^FH\^FDP/N:^FS";
            printStringGrande200dpi += @"^FT75,49^A0N,34,33^FH\^FD" + partNumber + " - " + lastPrinted.Descripcion + "^FS";
            printStringGrande200dpi += @"^FT586,209^A0N,42,43^FH\^FD" + DateTime.Today + "^FS";
            printStringGrande200dpi += @"^BY2,3,68^FT28,122^BCN,,N,N";
            printStringGrande200dpi += @"^FD>:" + partNumberToRead + "^FS";
            printStringGrande200dpi += @"^BY4,3,59^FT221,201^BCN,,N,N";
            printStringGrande200dpi += @"^FD>;" + qty + "^FS";
            printStringGrande200dpi += @"^BY4,3,83^FT128,345^BCN,,Y,N";
            printStringGrande200dpi += @"^FD>:" + contenedor + "^FS";
            printStringGrande200dpi += @"^PQ1,0,1,Y^XZ";  
            #endregion

            #region Zebra 105SL Plus 300dpi Etiqueta Grande
            string printStringGrande300dpi = "";
            //printStringGrande300dpi += @"^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR2,2~SD28^JUS^LRN^CI0^XZ";
            printStringGrande300dpi += @"^XA";
            //printStringGrande300dpi += @"^MMT";
            printStringGrande300dpi += @"^PW1181";
            printStringGrande300dpi += @"^LL0591";
            printStringGrande300dpi += @"^LS0";
            printStringGrande300dpi += @"^FO851,185^GB323,195,12^FS";
            printStringGrande300dpi += @"^FO8,185^GB263,195,12^FS";
            printStringGrande300dpi += @"^FO6,186^GB1167,195,12^FS";
            printStringGrande300dpi += @"^FO7,8^GB1167,189,12^FS";
            printStringGrande300dpi += @"^FO7,8^GB1167,575,12^FS";
            printStringGrande300dpi += @"^FT27,329^A0N,142,132^FH\^FDCGS^FS";
            printStringGrande300dpi += @"^FT326,355^A0N,50,33^FH\^FDCANTIDAD^FS";
            printStringGrande300dpi += @"^FT469,359^A0N,75,72^FH\^FD" + qty + "^FS";
            printStringGrande300dpi += @"^FT31,74^A0N,42,43^FH\^FDP/N:^FS";
            printStringGrande300dpi += @"^FT110,73^A0N,50,50^FH\^FD" + partNumber + " - " + lastPrinted.Descripcion + "^FS";
            printStringGrande300dpi += @"^FT866,310^A0N,62,64^FH\^FD" + DateTime.Today + "^FS";
            printStringGrande300dpi += @"^BY4,3,101^FT41,181^BCN,,N,N";
            printStringGrande300dpi += @"^FD>;" + partNumberToRead + "^FS";
            printStringGrande300dpi += @"^BY6,3,87^FT327,297^BCN,,N,N";
            printStringGrande300dpi += @"^FD>:"+qty+"^FS";
            printStringGrande300dpi += @"^BY6,3,141^FT189,530^BCN,,Y,N";
            printStringGrande300dpi += @"^FD>:"+contenedor+"^FS";
            printStringGrande300dpi += @"^PQ1,0,1,Y^XZ";
            #endregion

            #region Zebra GK420t 203dpi Etiqueta Chica
            string printStringChica200dpi = "";
            //printStringChica200dpi += @"^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR3,3~SD24^JUS^LRN^CI0^XZ";
            printStringChica200dpi += @"^XA";
            //printStringChica200dpi += @"^MMT";
            printStringChica200dpi += @"^PW480";
            printStringChica200dpi += @"^LL0152";
            printStringChica200dpi += @"^LS0";
            printStringChica200dpi += @"^BY2,3,28^FT25,131^BCN,,N,N";
            printStringChica200dpi += @"^FD>:"+contenedor+"^FS";
            printStringChica200dpi += @"^BY2,3,35^FT23,78^BCN,,N,N";
            printStringChica200dpi += @"^FD>:"+ partNumberToRead + "^FS";
            printStringChica200dpi += @"^FT20,147^A0N,20,19^FH\^FDContenedor:^FS";
            printStringChica200dpi += @"^FT346,147^A0N,23,19^FH\^FD"+qty+"^FS";
            printStringChica200dpi += @"^FT290,148^A0N,20,14^FH\^FDCantidad:^FS";
            printStringChica200dpi += @"^FT114,148^A0N,17,21^FH\^FD"+contenedor+"^FS";
            printStringChica200dpi += @"^FT93,98^A0N,25,19^FH\^FD"+partNumber+"^FS";
            printStringChica200dpi += @"^FT26,98^A0N,25,19^FH\^FDCodigo:^FS";
            printStringChica200dpi += @"^PQ1,0,1,Y^XZ";
            #endregion

            #region Zebra 105SL Plus 300dpi Etiqueta Chica
            string printStringChica300dpi = "";
            //printStringChica300dpi += @"^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR4,4~SD25^JUS^LRN^CI0^XZ";
            printStringChica300dpi += @"^XA";
            //printStringChica300dpi += @"^MMT";
            printStringChica300dpi += @"^PW709";
            printStringChica300dpi += @"^LL0224";
            printStringChica300dpi += @"^LS0";
            printStringChica300dpi += @"^BY3,3,41^FT85,146^BCN,,N,N";
            printStringChica300dpi += @"^FD>:"+contenedor+"^FS";
            printStringChica300dpi += @"^BY3,3,52^FT82,69^BCN,,N,N";
            printStringChica300dpi += @"^FD>:"+ partNumberToRead + "^FS";
            printStringChica300dpi += @"^FT77,170^A0N,29,28^FH\^FDContenedor:^FS";
            printStringChica300dpi += @"^FT558,169^A0N,33,31^FH\^FD"+qty+"^FS";
            printStringChica300dpi += @"^FT476,170^A0N,29,21^FH\^FDCantidad:^FS";
            printStringChica300dpi += @"^FT215,171^A0N,25,31^FH\^FD"+contenedor+"^FS";
            printStringChica300dpi += @"^FT184,98^A0N,37,28^FH\^FD"+partNumber+"^FS";
            printStringChica300dpi += @"^FT86,97^A0N,37,28^FH\^FDCodigo:^FS";
            printStringChica300dpi += @"^PQ1,0,1,Y^XZ ";
            #endregion

            string printString = "";
            string tipoPrint =validarImpresora(boton, rbtContg, rbtSplitg,rbtCont200dpi,rbtSplit200dpi); 
            if (tipoPrint == "Grande200")
            {
                printString = printStringGrande200dpi;
            }
            else if (tipoPrint == "Grande300")
            {
                printString = printStringGrande300dpi;
            }
            else if (tipoPrint =="Chica200")
            {
                printString = printStringChica200dpi;
            }
            else if (tipoPrint == "Chica300")
            {
                printString = printStringChica300dpi;
            }

            RawPrinterHelper.SendStringToPrinter(impresora, printString);
            if (!chkReimp.Checked) flag = false;
            if (flag)
            {
                flag = false;
                if (boton == "btnDividir")
                {
                    qty = qtySplit - qty;
                    print(partNumber, contSplit, qty, modelo, lote, boton, impresora, rbtContg, rbtSplitg, rbtCont200dpi, rbtSplit200dpi, qtySplit, contSplit, chkReimp);
                }
            }
        }

        private string validarImpresora(string boton,RadioButton rbtContg,RadioButton rbtSplitg,RadioButton rbtCont200dpi,RadioButton rbtSplit200dpi)
        {
            string tamaño = "";
            if (boton == "btnGenerar")
            {
                if ((rbtContg.Checked) && (rbtCont200dpi.Checked))
                { tamaño = "Grande200"; }
                else if ((rbtContg.Checked) && (!rbtCont200dpi.Checked))
                { tamaño = "Grande300"; }
                else if ((!rbtContg.Checked) && (rbtCont200dpi.Checked))
                { tamaño = "Chica200"; }
                else if ((!rbtContg.Checked) && (!rbtCont200dpi.Checked))
                { tamaño = "Chica300"; }
            }
            else
            {
                if ((rbtSplitg.Checked) && (rbtSplit200dpi.Checked))
                { tamaño = "Grande200"; }
                else if ((rbtSplitg.Checked) && (!rbtSplit200dpi.Checked))
                { tamaño = "Grande300"; }
                else if ((!rbtSplitg.Checked) && (rbtSplit200dpi.Checked))
                { tamaño = "Chica200"; }
                else if ((!rbtSplitg.Checked) && (!rbtSplit200dpi.Checked))
                { tamaño = "Chica300"; }
            }
            return tamaño;
        }
    }
}
