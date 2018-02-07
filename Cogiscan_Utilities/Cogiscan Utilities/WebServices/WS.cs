using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Cogiscan_Utilities.CogiscanWebServices;
using System.Xml.Linq;
using System.Windows.Forms;
using Cogiscan_Utilities.Entitys;

namespace Cogiscan_Utilities.WebServices
{
    class WS
    {
        public static RPCServicesClient RPC = new CogiscanWebServices.RPCServicesClient();

        public static bool chkPN(string PN)
        {
            try
            {
                executeCommandRequest exec = new executeCommandRequest("queryPartNumber", @"
                    <Parameters>
                        <Parameter name =""itemTypeClass"">Component Lot</Parameter>
                        <Parameter name =""partNumber"">" + PN + @"</Parameter>
                    </Parameters>"

                    );
                executeCommandResponse res = RPC.executeCommand(exec);
                if (res != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Conexion.insertErrorInDB(e.ToString());
                return false;
            }
        }
        public static string[] getContainerInfo(string contenedor)
        {
            string pn;
            string qty;
            string containerId;
            string locationInContainer;
            string[] array = { };

            if (contenedor.StartsWith(")"))
            { contenedor = contenedor.Replace(")", "("); }

            try
            {
                executeCommandRequest exec = new executeCommandRequest("queryItem", @"
                    <Parameters>
                        <Parameter name =""itemId"">"+ contenedor +@"</Parameter>
                    </Parameters>"

                    );
                executeCommandResponse res = RPC.executeCommand(exec);
                if (res != null)
                {
                    XDocument arr = XDocument.Parse(res.result, LoadOptions.None);
                    XElement Elemento = arr.Element("Item");
                    IEnumerable<XElement> elementos = Elemento.Elements();
                    pn = elementos.ElementAt(0).Attribute("partNumber").Value;
                    qty = elementos.ElementAt(0).Attribute("quantity").Value;
                    if (Elemento.ToString().Contains("containerId"))
                    {
                        containerId = Elemento.Attribute("containerId").Value;
                        if (Elemento.ToString().Contains("locationInContainer"))
                        {
                            locationInContainer = Elemento.Attribute("locationInContainer").Value;
                            array = new string[4] { pn, qty, containerId, locationInContainer };
                        }
                        else
                        {
                            array = new string[3] { pn, qty, containerId};
                        }
                        
                    }
                    else
                    { array = new string[2] { pn, qty }; }
                
                    return array;
                }
                else
                {
                }
            }
            catch (Exception e)
            {
                Conexion.insertErrorInDB(e.ToString());
                return null;
            }
            return null;
        }
        public static string initRawMaterial(string partNumber,string contenedor, int qty,string tipo,string modelo, string lote)
        {
            try
            {
                if (tipo == "REEL")//Si no posee barcode de referencia lo inicializo normalmente, de otra manera, se dirige al "else"
                {
                    executeCommandRequest execIRM = new executeCommandRequest("initializeRawMaterial", @"
                    <Parameters>
                        <Parameter name=""itemId"">" + contenedor + @"</Parameter>
                        <Parameter name=""partNumber"">" + partNumber + @"</Parameter>
                        <Parameter name=""containerType"">" + tipo + @"</Parameter>
                        <Parameter name=""supplierId"">Default</Parameter>
                        <Parameter name=""quantity"">" + qty + @"</Parameter>
                    </Parameters>");
                    executeCommandResponse resIRM = RPC.executeCommand(execIRM);  
                }
                else
                {
                    executeCommandRequest execIRM = new executeCommandRequest("initializeRawMaterial", @"
                    <Parameters>
                        <Parameter name=""itemId"">" + contenedor + @"</Parameter>
                        <Parameter name=""partNumber"">" + partNumber + @"</Parameter>
                        <Parameter name=""msLevel"">1</Parameter>
                        <Parameter name=""containerType"">" + tipo + @"</Parameter>
                        <Parameter name=""supplierId"">Default</Parameter>
                        <Parameter name=""quantity"">" + qty + @"</Parameter>
                        <Parameter name=""itemBarcode"">" + Global.barcode + @"</Parameter>
                        <Parameter name=""updateExisting"">true</Parameter>
                    </Parameters>");
                    executeCommandResponse resIRM = RPC.executeCommand(execIRM);
                }
                //Inserto los datos en la base de datos de IA Server
                Conexion.insertInContainerDataBase(partNumber, contenedor, qty, tipo, modelo, lote,'N');
                return "Operación Exitosa";
            }
            catch (Exception e)
            {
                Conexion.insertErrorInDB(e.ToString());
                return "Ah ocurrido un error";
            }
        }
        public static bool unLoadContentId(string contenedor, string idContenedor, string Locacion)
        {
            try
            {
                executeCommandRequest exeULCI = new executeCommandRequest("unload", @"
                    <Parameters>
                        <Parameter name=""contentId"">"+contenedor+@"</Parameter>
                        <Parameter name=""containerId"">"+idContenedor+@"</Parameter>
                        <Parameter name=""location"">"+Locacion+@"</Parameter>
                        <Parameter name=""deleteContent"">false</Parameter>
                    </Parameters>");
                executeCommandResponse resULCI = RPC.executeCommand(exeULCI);
            }
            catch (Exception e)
            {
                Conexion.insertErrorInDB(e.ToString());
                return false;
            }

            return true;
        }
        public static bool LoadContent(string contenedor, string idContenedor, string Location)
        {
            try
            {
                executeCommandRequest exeLCI = new executeCommandRequest("load", @"
                    <Parameters>
                        <Parameter name=""contentId"">"+contenedor+@"</Parameter>
                        <Parameter name=""containerId"">"+idContenedor+@"</Parameter>
                        <Parameter name=""location"">"+Location+@"</Parameter>
                        <Parameter name=""unloadPrevious"">true</Parameter>
                        <Parameter name=""deleteContent"">false</Parameter>
                    </Parameters>");
                executeCommandResponse resLCI = RPC.executeCommand(exeLCI);
            }
            catch (Exception e)
            {
                Conexion.insertErrorInDB(e.ToString());
                return false;
            }
            return true;
        }
        public static string splitRawMaterial(string contenedor,int qtyInicial, int qtySplit,string contenedorNuevo, string modelo, string lote)
        {
            if (qtyInicial >= qtySplit)
            {
                try
                {
                    executeCommandRequest execSRM = new executeCommandRequest("splitRawMaterial", @"
                    <Parameters>
                        <Parameter name=""itemId"">" + contenedor + @"</Parameter>
                        <Parameter name=""quantity"">" + qtySplit + @"</Parameter>
                        <Parameter name=""targetItemId"">" + contenedorNuevo + @"</Parameter>
                    </Parameters>

                    ");
                    executeCommandResponse resSRM = RPC.executeCommand(execSRM);
                    Conexion.insertInSplitDataBase(contenedor, qtySplit, contenedorNuevo);
                    return "Operación Exitosa";
                }
                catch (Exception e)
                {
                    Conexion.insertErrorInDB(e.ToString());
                    return "Ah ocurrido un error";
                }
            }
            else
            { return "La cantidad de Subdivision no puede ser Mayor a la Inicial"; }
        }
        public static string[] queryTooling()
        {
            string[] tooling= {};
            executeCommandRequest execQT = new executeCommandRequest("queryPartNumber",@"
<Parameters>
  <Parameter name =""itemTypeClass"">Tooling</Parameters>
  <Parameter name =""partNumber"">%%</Parameter>
</Parameters>");
            executeCommandResponse resQT = RPC.executeCommand(execQT);

            return tooling;
        }
        public static bool loadInContainer(string rawMaterial,string partNumber, string qty, string contenedor, string ubicacionEnContenedor)
        {
            executeCommandRequest execLoad = new executeCommandRequest("load", @"
                    <Parameters>
                        <Parameter name =""contentId"">" + rawMaterial + @"</Parameter>
                        <Parameter name =""containerId"">" + contenedor + @"</Parameter>
                        <Parameter name =""location"">" + ubicacionEnContenedor + @"</Parameter>
                        <Parameter name =""unloadPrevious"">false</Parameter>
                        <Parameter name =""deletePrevious"">false</Parameter>
                    </Parameters>"

                    );
            executeCommandResponse res = RPC.executeCommand(execLoad);
            if (res.result=="<Success />")
            {
                Conexion.insertLoadEvent(rawMaterial, partNumber, qty, contenedor, Global.userLogged);
                return true; 
            }
            else
            {return false;}
        }
        public static bool queryItem(string item)
        {
            try
            {
                executeCommandRequest exec = new executeCommandRequest("queryItem", @"
                    <Parameters>
                        <Parameter name =""itemId"">Component Lot</Parameter>
                    </Parameters>"

                    );
                executeCommandResponse res = RPC.executeCommand(exec);
                if (res != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Conexion.insertErrorInDB(e.ToString());
                return false;
            }
        }
        public static bool getContents(string item)
        {
            try
            {
                executeCommandRequest exec = new executeCommandRequest("getContents", @"
                    <Parameters>
                        <Parameter name =""containerId"">" + item + @"</Parameter>
                    </Parameters>"

                    );
                executeCommandResponse res = RPC.executeCommand(exec);
                if (res.result != @"<Exception exceptionType=""java.lang.Exception"" message=""Container does not exist."" />")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Conexion.insertErrorInDB(e.ToString());
                return false;
            }
        }
        public static Array getContentsAndReturnValues(string container)
        {
            DataTable dt = new DataTable();
            List<RMWentity> propiedadesMateriales = new List<RMWentity>();
            try
            {
                executeCommandRequest exec = new executeCommandRequest("getContents", @"
                    <Parameters>
                        <Parameter name =""containerId"">" + container + @"</Parameter>
                    </Parameters>"

                    );
                executeCommandResponse res = RPC.executeCommand(exec);
                if (res != null)
                {
                    string partNumber;
                    string rawMaterialId;
                    string qty;
                    XDocument arr = XDocument.Parse(res.result, LoadOptions.None);
                    XElement Elemento = arr.Element("Contents");
                    IEnumerable<XElement> lista = Elemento.Elements();
                    foreach (XElement item in lista)
                    {
                        IEnumerable<XElement> subLista = item.Elements();//Genero una sublista de los elementos del primer elemento
                        partNumber = subLista.ElementAt(0).Attribute("partNumber").Value;
                        if (chkifNotInList(partNumber,propiedadesMateriales))
                        {

                        }
                        //Conexion.getRawMaterialHistory(rawMaterialId);
                        qty = subLista.ElementAt(0).Attribute("quantity").Value;
                        propiedadesMateriales.Add(new RMWentity() 
                        { 
                            partNumber = partNumber, 
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Conexion.insertErrorInDB(e.ToString());
            }
            Array propMat = propiedadesMateriales.ToArray();
            return propMat;
        }

        private static bool chkifNotInList(string partNumber, List<RMWentity> lista)
        {
            foreach (RMWentity item in lista)
            {
                if (item.ToString() == partNumber)
                {
                    return true;
                }
            }
            return false;
        }

        public static Array getRawMaterialStatus(string container)
        {

            DataTable dt = new DataTable();
            List<RMWstatusEntity> propiedadesMateriales = new List<RMWstatusEntity>();
            try
            {
                executeCommandRequest exec = new executeCommandRequest("getContents", @"
                    <Parameters>
                        <Parameter name =""containerId"">" + container + @"</Parameter>
                    </Parameters>"

                    );
                executeCommandResponse res = RPC.executeCommand(exec);
                if (res != null)
                {
                    string partNumber;
                    string rawMaterialId;
                    List<RMWentity> listaProMat = new List<RMWentity>();
                    XDocument arr = XDocument.Parse(res.result, LoadOptions.None);
                    XElement Elemento = arr.Element("Contents");
                    IEnumerable<XElement> lista = Elemento.Elements();
                    foreach (XElement item in lista)
                    {
                        IEnumerable<XElement> subLista = item.Elements();//Genero una sublista de los elementos del primer elemento
                        partNumber = subLista.ElementAt(0).Attribute("partNumber").Value;
                        rawMaterialId = item.Attribute("itemId").Value;
                        listaProMat = Conexion.getRawMaterialHistory(rawMaterialId, partNumber);
                        propiedadesMateriales.Add(new RMWstatusEntity()
                        {
                            partNumber = partNumber,
                            primerCarro = "",
                            segundoCarro = "",
                            tercerCarro = "",
                            cuartoCarro = "",
                            quintoCarro = ""
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Conexion.insertErrorInDB(e.ToString());
            }
            Array propMat = propiedadesMateriales.ToArray();
            return propMat;
        }
    }
}
