using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using Cogiscan_Utilities.CogiscanWebServices;
using System.Text.RegularExpressions;
using Cogiscan_Utilities.Clases;
using Cogiscan_Utilities.Entitys;
using System.Data;
using System.Threading;

namespace Cogiscan_Utilities
{
    class Conexion
    {
        ///<summary>
        ///Conexiones configuradas en app.config
        ///</summary>
        ///<returns></returns>
        #region Conexiones
        // **** Parametros de conexión base CogiscanIaServer****
        static string Conn = ConfigurationSettings.AppSettings["MySqlCogiscan"];
        static MySqlConnection ConexionMysql = new MySqlConnection(Conn);
        // ********************************

        // **** Parametros de conexión base MaterialesSmt****
        static string ConnectionMaterialesSmt = ConfigurationSettings.AppSettings["smtDataBase"];
        static MySqlConnection ConexionSMTIaServer = new MySqlConnection(ConnectionMaterialesSmt);
        // **************************************************

        // **** Parametros de conexión base Cogiscan Utilities ****
        static string ConnCgsUtils = ConfigurationSettings.AppSettings["MySqlCogiscanUtilities"];
        static MySqlConnection ConexionCogiscanUtilities = new MySqlConnection(ConnCgsUtils);
        // ******************************************************** 
        #endregion
        
        public static string queryLastInsert()
        {
            string contenedor="";
            try
            {
                if (ConexionCogiscanUtilities.State == ConnectionState.Closed)ConexionCogiscanUtilities.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter("spLastInsertIRM", ConexionCogiscanUtilities);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable tabla = new DataTable();
                adapter.Fill(tabla);
                ConexionCogiscanUtilities.Close();
                contenedor = tabla.Rows[0][0].ToString();
            }
            catch(Exception e)
            { Conexion.insertErrorInDB(e.ToString()); }
            return contenedor;
        }
        public static string queryLastInsertSRM()
        {
            string contenedor = "";
            try
            {
                DataTable tabla = new DataTable();
                if (ConexionCogiscanUtilities.State == ConnectionState.Closed) ConexionCogiscanUtilities.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter("spLastInsertSRM", ConexionCogiscanUtilities);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                Thread.Sleep(500);
                adapter.Fill(tabla);
                ConexionCogiscanUtilities.Close();
                contenedor = tabla.Rows[0][0].ToString();
            }
            catch (Exception e)
            { Conexion.insertErrorInDB(e.ToString()); }
            return contenedor;
        }
        public static string queryDesc(string pn,string modelo, string lote)
        {
            DataTable tabla = new DataTable();
            if (ConexionSMTIaServer.State == ConnectionState.Closed) ConexionSMTIaServer.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("spQueryDesc", ConnectionMaterialesSmt);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.AddWithValue("pn",pn);
            adapter.SelectCommand.Parameters.AddWithValue("modelo",modelo);
            adapter.SelectCommand.Parameters.AddWithValue("lote", lote);
            Thread.Sleep(1000);
            adapter.Fill(tabla);
            ConexionSMTIaServer.Close();
            lastPrinted.Descripcion = "";
            if (tabla.Rows.Count != 0)
            { lastPrinted.Descripcion = tabla.Rows[0][0].ToString(); }
            return lastPrinted.Descripcion;
        }
        public static List<string> queryModelos()
        {
            List<string> listaModelos = new List<string>();
            DataTable tablaModelos = new DataTable();
            string[] modelos={};
            try
            {
                if (ConexionSMTIaServer.State == ConnectionState.Closed) ConexionSMTIaServer.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter("spQueryModels", ConnectionMaterialesSmt);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.Fill(tablaModelos);
                //recupero los modelos y los meto en una lista
                listaModelos.Add("-Seleccione-");
                foreach (DataRow dr in tablaModelos.Rows)
                {
                    listaModelos.Add(dr["modelo"].ToString());
                }
                ConexionSMTIaServer.Close();
            }
            catch (Exception e)
            {
                Conexion.insertErrorInDB(e.ToString());
            }
            return listaModelos;
        }
        public static List<string> queryLotes(string modelo)
        {
            List<string> listaLotes = new List<string>();
            DataTable tablaLotes = new DataTable();
            try
            {
                if (ConexionSMTIaServer.State == ConnectionState.Closed) ConexionSMTIaServer.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter("spQueryBatch",ConnectionMaterialesSmt);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.Parameters.AddWithValue("_modelo",modelo);
                adapter.Fill(tablaLotes);
                //recupero los Lotes y los meto en una lista
                listaLotes.Add("-Seleccione-");
                foreach (DataRow dr in tablaLotes.Rows)
                {
                    listaLotes.Add(dr["lote"].ToString());
                }
                ConexionSMTIaServer.Close();
            }
            catch(Exception e)
            {Conexion.insertErrorInDB(e.ToString());}
            return listaLotes;
        }
        public static void insertInContainerDataBase(string pn,string contenedor,int qty,string tipo, string modelo, string lote,char reimpresion)
        {
            if (ConexionCogiscanUtilities.State == ConnectionState.Closed) ConexionCogiscanUtilities.Open();
            string[] usuario = HostAndUser.host();
            using (MySqlCommand cmdInsertIRM = ConexionCogiscanUtilities.CreateCommand())
            {
                cmdInsertIRM.CommandType = CommandType.StoredProcedure;
                cmdInsertIRM.CommandText = "spInsertNewContainer";
                cmdInsertIRM.Parameters.AddWithValue("_pn", pn);
                cmdInsertIRM.Parameters.AddWithValue("_contenedor", contenedor);
                cmdInsertIRM.Parameters.AddWithValue("_qty", qty);
                cmdInsertIRM.Parameters.AddWithValue("_barcode", Global.barcode);
                cmdInsertIRM.Parameters.AddWithValue("_host", usuario[0]);
                cmdInsertIRM.Parameters.AddWithValue("_usuario", Global.userLogged);
                cmdInsertIRM.Parameters.AddWithValue("_tipo", tipo);
                cmdInsertIRM.Parameters.AddWithValue("_modelo", modelo);
                cmdInsertIRM.Parameters.AddWithValue("_lote", lote);
                cmdInsertIRM.Parameters.AddWithValue("_reimpresion", reimpresion);
                cmdInsertIRM.ExecuteNonQuery();
            }
            ConexionCogiscanUtilities.Close();
        }
        public static void insertInSplitDataBase(string contenedorInicial,int qty,string contenedorNuevo)
        {
            if (ConexionCogiscanUtilities.State == ConnectionState.Closed) ConexionCogiscanUtilities.Open();
            string[] usuario = HostAndUser.host();
            using (MySqlCommand cmdInsertSplit = ConexionCogiscanUtilities.CreateCommand())
            {
                cmdInsertSplit.CommandText = "spInsertSplitMaterial";
                cmdInsertSplit.CommandType = CommandType.StoredProcedure;
                cmdInsertSplit.Parameters.AddWithValue("_contenedor_inicial", contenedorInicial);
                cmdInsertSplit.Parameters.AddWithValue("_qty_division", qty);
                cmdInsertSplit.Parameters.AddWithValue("_contenedor_nuevo", contenedorNuevo);
                cmdInsertSplit.Parameters.AddWithValue("_host", usuario[0]);
                cmdInsertSplit.Parameters.AddWithValue("_usuario", Global.userLogged);
                cmdInsertSplit.ExecuteNonQuery();
            }
            ConexionCogiscanUtilities.Close();
        }
        public static void insertErrorInDB(string error)
        {
            if (ConexionCogiscanUtilities.State == ConnectionState.Closed)
            { ConexionCogiscanUtilities.Open(); }
            using (MySqlCommand cmdInsertError = ConexionCogiscanUtilities.CreateCommand())
            {
                cmdInsertError.CommandText = "spSaveErrorIntoDB";
                cmdInsertError.CommandType = CommandType.StoredProcedure;
                cmdInsertError.Parameters.AddWithValue("_error",error);
                cmdInsertError.ExecuteNonQuery();
            }
            if (ConexionCogiscanUtilities.State == ConnectionState.Open)
            { ConexionCogiscanUtilities.Close(); }
        }
        public static void checkClientOnHeartBreathTable()
        {
            //Obtengo los datos del cliente
            string[] hostInfo = HostAndUser.host();
            if (ConexionCogiscanUtilities.State == ConnectionState.Closed)
            { ConexionCogiscanUtilities.Open(); }
            using (MySqlCommand cmdInsertError = ConexionCogiscanUtilities.CreateCommand())
            {
                cmdInsertError.CommandText = "spCheckClientHearthBreath";
                cmdInsertError.CommandType = CommandType.StoredProcedure;
                cmdInsertError.Parameters.AddWithValue("_hostname", hostInfo[0]);
                cmdInsertError.Parameters.AddWithValue("_ip_address", hostInfo[2]);
                cmdInsertError.ExecuteNonQuery();
            }
            if (ConexionCogiscanUtilities.State == ConnectionState.Open)
            { ConexionCogiscanUtilities.Close(); }
        }
        public static void insertHearthBreath()
        {
            //Obtengo los datos del cliente
            string[] hostInfo = HostAndUser.host();
            if (ConexionCogiscanUtilities.State == ConnectionState.Closed)
            { ConexionCogiscanUtilities.Open(); }
            using (MySqlCommand cmdInsertError = ConexionCogiscanUtilities.CreateCommand())
            {
                cmdInsertError.CommandText = "spUpdHearthBreath";
                cmdInsertError.CommandType = CommandType.StoredProcedure;
                cmdInsertError.Parameters.AddWithValue("_hostname", hostInfo[0]);
                cmdInsertError.Parameters.AddWithValue("_ip_address", hostInfo[2]);
                cmdInsertError.ExecuteNonQuery();
            }
            if (ConexionCogiscanUtilities.State == ConnectionState.Open)
            { ConexionCogiscanUtilities.Close(); }
        }
        public static void insertLoadEvent(string contenedor, string codigo, string cantidad, string ubicacion, string usuario)
        {
            if (ConexionCogiscanUtilities.State == ConnectionState.Closed) ConexionCogiscanUtilities.Open();
            using (MySqlCommand cmdinsertLoadEvent = ConexionCogiscanUtilities.CreateCommand())
            {
                cmdinsertLoadEvent.CommandText = "spInsertLoadEvent";
                cmdinsertLoadEvent.CommandType = CommandType.StoredProcedure;
                cmdinsertLoadEvent.Parameters.AddWithValue("_contenedor",contenedor);
                cmdinsertLoadEvent.Parameters.AddWithValue("_codigo",codigo);
                cmdinsertLoadEvent.Parameters.AddWithValue("_cantidad",cantidad);
                cmdinsertLoadEvent.Parameters.AddWithValue("_ubicacion",ubicacion);
                cmdinsertLoadEvent.Parameters.AddWithValue("_usuario",usuario);
                cmdinsertLoadEvent.ExecuteNonQuery();
            }
            ConexionCogiscanUtilities.Close();
        }
        public static List<string> queryQuser()
        {
            List<string> listaQusers = new List<string>();
            DataTable dt = new DataTable();
            try
            {
                if (ConexionCogiscanUtilities.State == ConnectionState.Closed) ConexionCogiscanUtilities.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter("spQuser", ConexionCogiscanUtilities);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.Fill(dt);
                //recupero los Lotes y los meto en una lista
                foreach (DataRow dr in dt.Rows)
                {
                    listaQusers.Add(dr["user_name"].ToString());
                }
                ConexionCogiscanUtilities.Close();
            }
            catch (Exception e)
            { Conexion.insertErrorInDB(e.ToString()); }
            return listaQusers;

        }

        /// <summary>
        /// SP de Estado de Materiales
        /// </summary>
        /// <returns></returns>
        #region Estado de Materiales
        public static string[] queryAcceptedContainer()
        {
            List<string> listaAcceptedContainer = new List<string>();
            DataTable dt = new DataTable();
            if (ConexionCogiscanUtilities.State == ConnectionState.Closed) ConexionCogiscanUtilities.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("spQueryAcceptedContainer", ConexionCogiscanUtilities);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                listaAcceptedContainer.Add(dr["code_begin"].ToString());
            }
            ConexionCogiscanUtilities.Close();

            return listaAcceptedContainer.ToArray();
        }
        public static string[] queryAcceptedRawMaterial()
        {
            List<string> listaAcceptedRawMaterial = new List<string>();
            DataTable dt = new DataTable();
            if (ConexionCogiscanUtilities.State == ConnectionState.Closed) ConexionCogiscanUtilities.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("spQueryAcceptedRawMaterial", ConexionCogiscanUtilities);
            adapter.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                listaAcceptedRawMaterial.Add(dr["code_begin"].ToString());
            }
            ConexionCogiscanUtilities.Close();

            return listaAcceptedRawMaterial.ToArray();
        }
        public static List<RMWstatusEntity> getRawMaterialStatus(string partNumber, DateTime _horaInicio, DateTime _horaFin)
        {
            List<RMWstatusEntity> estado = new List<RMWstatusEntity>();

            return estado;

        }
        public static List<RMWentity> getRawMaterialHistory(string contenedor,string partNumber)
        {
            List<RMWentity> lista = new List<RMWentity>();
            if (ConexionCogiscanUtilities.State == ConnectionState.Closed) ConexionCogiscanUtilities.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("spGetRawMaterialHistory", ConexionCogiscanUtilities);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.AddWithValue("_rawMaterialId", contenedor);
            DataTable tabla = new DataTable();
            adapter.Fill(tabla);
            ConexionCogiscanUtilities.Close();
            if (tabla.Rows.Count != 0)
            {
                foreach (DataRow row in tabla.Rows)
                {
                    lista.Add(new RMWentity()
                    {
                        quantity = row[0].ToString(),
                        location = row[1].ToString(),
                        rawMaterialId = contenedor,
                        partNumber = partNumber
                    });
                }
            }
            else
            { }
            return lista;
        }
        public static List<string> queryRMWModels()
        {
            List<string> Models = new List<string>();
            DataTable dt = new DataTable();
            if (ConexionCogiscanUtilities.State == ConnectionState.Closed) ConexionCogiscanUtilities.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("spGetRMWModels", ConexionCogiscanUtilities);
            adapter.Fill(dt);
            Models.Add("-- Seleccione Modelo --");
            foreach (DataRow row in dt.Rows)
            {
                Models.Add(row[0].ToString());
            }
            return Models;
        }
        public static Array queryRMWBOM(string modelo)
        {
            List<RMWstatusEntity> bom = new List<RMWstatusEntity>();
            DataTable dt = new DataTable();
            if (ConexionCogiscanUtilities.State == ConnectionState.Closed) ConexionCogiscanUtilities.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("spGetBOM", ConexionCogiscanUtilities);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.AddWithValue("_modelo",modelo);
            adapter.Fill(dt);
            ConexionCogiscanUtilities.Close();
            foreach (DataRow row in dt.Rows)
            {
                bom.Add(new RMWstatusEntity()
                {
                    partNumber = row[0].ToString(),
                    primerCarro = "",
                    segundoCarro = "",
                    tercerCarro = "",
                    cuartoCarro = "",
                    quintoCarro = ""
                });
            }
            return bom.ToArray();
        }
        #endregion
    }
}
