using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBM.Data.DB2;
using System.Data;
using System.Configuration;
using Cogiscan_Utilities.Clases;
using Cogiscan_Utilities.WebServices;

namespace Cogiscan_Utilities.DB2
{
    class ConexionDB2
    {
        //Conexión 10.30.10.90
        static string ConnDB2CGS = ConfigurationSettings.AppSettings["ConnectToDB2CGS"];
        static DB2Connection ConnectDB2CGS = new DB2Connection(ConnDB2CGS);
        //Conexión 10.30.10.89
        static string ConnDB2CGSDW = ConfigurationSettings.AppSettings["ConnectToDB2CGSDW"];
        static DB2Connection ConnectDB2CGSDW = new DB2Connection(ConnDB2CGSDW);
        

        public static bool queryUser(string user, string pass)
        {
            DataTable dt = new DataTable();
            string queryUser = "SELECT u.USER_KEY, u.USER_ID, p.PASSWORD FROM CGS.\"USER\" u ";
                    queryUser+="LEFT JOIN CGS.USER_PASSWORD p ";
                    queryUser+="ON p.USER_KEY=u.USER_KEY ";
                    queryUser+="WHERE u.USER_ID='"+user+"'";
            ConnectDB2CGS.Open();
            DB2DataAdapter adapter = new DB2DataAdapter(queryUser, ConnectDB2CGS);
            adapter.Fill(dt);
            ConnectDB2CGS.Close();
            string userId = dt.Rows[0][0].ToString();
            string usuario = dt.Rows[0][1].ToString();
            string password = dt.Rows[0][2].ToString();
            string usuarioFormateado = usuario.Replace(" ","");

            if ((Hash.getHash(pass, password))&&(user==usuarioFormateado))
            { return true; }
            else { return false; }
        }
        public static bool queryLocation(string location)
        {
            DataTable dtLocation = new DataTable();
            string queryLocation = "SELECT I.ITEM_ID,L.LOCATION_NAME FROM CGS.\"LOCATION\" L ";
                    queryLocation += "LEFT JOIN CGS.\"ITEM_INFO\" I ON L.CNTR_KEY = I.ITEM_KEY ";
                    queryLocation += "WHERE L.LOCATION_KEY='"+location+"'";
            ConnectDB2CGS.Open();
            DB2DataAdapter adapterLocation = new DB2DataAdapter(queryLocation, ConnectDB2CGS);
            adapterLocation.Fill(dtLocation);
            ConnectDB2CGS.Close();
            if (dtLocation.Rows.Count != 0)
            {
                containerLocation.cntr_name = dtLocation.Rows[0][0].ToString().Replace(" ","");
                containerLocation.cntr_location = dtLocation.Rows[0][1].ToString().Replace(" ","");
                return true;
            }
            return false;
        }
        public static bool queryItemInfo(string item)
        {
            DataTable dtItem = new DataTable();
            string queryItem = "SELECT ITEM_ID FROM CGS.\"ITEM_INFO\" WHERE ITEM_KEY='" + item + "'";
            ConnectDB2CGS.Open();
            DB2DataAdapter adapterItem = new DB2DataAdapter(queryItem,ConnectDB2CGS);
            adapterItem.Fill(dtItem);
            ConnectDB2CGS.Close();
            if (dtItem.Rows.Count != 0)
            {
                containerLocation.cntr_name= dtItem.Rows[0][0].ToString();
                containerLocation.cntr_location = "";
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public static string[] AllLocations()
        {
            List<string> ubicaciones = new List<string>();
            ubicaciones.Add("-- Seleccione Contenedor --");
            containerLocation.locationType.Add("Todos");
            DataTable dtLocations = new DataTable();
            string queryLocations = "SELECT ITEM_ID, ITEM_TYPE_NAME FROM CGS.\"ITEM_INFO\" WHERE ITEM_CLASS_KEY='11' ORDER BY ITEM_ID ASC";
            ConnectDB2CGS.Open();
            DB2DataAdapter adapterLocations = new DB2DataAdapter(queryLocations, ConnectDB2CGS);
            adapterLocations.Fill(dtLocations);
            foreach (DataRow row in dtLocations.Rows)
            {
                ubicaciones.Add(row[0].ToString().Replace(" ", ""));
                containerLocation.locationType.Add(row[1].ToString().Replace(" ", ""));
            }
            ConnectDB2CGS.Close();

            return ubicaciones.ToArray();
        }
        public static string[] FilteredLocations(string filtro)
        {
            List<string> ubicacionesFiltradas = new List<string>();
            ubicacionesFiltradas.Add("-- Seleccione Contenedor --");
            DataTable dtLocationFiltered = new DataTable();
            string queryFilteredLocations = "SELECT ITEM_ID FROM CGS.\"ITEM_INFO\" WHERE ITEM_CLASS_KEY='11' AND ITEM_TYPE_NAME='"+filtro+"' ORDER BY ITEM_ID ASC";
            ConnectDB2CGS.Open();
            DB2DataAdapter adapterAllLocations = new DB2DataAdapter(queryFilteredLocations,ConnectDB2CGS);
            adapterAllLocations.Fill(dtLocationFiltered);
            ConnectDB2CGS.Close();
            foreach (DataRow row in dtLocationFiltered.Rows)
            {
                ubicacionesFiltradas.Add(row[0].ToString().Replace(" ",""));
            }
            return ubicacionesFiltradas.ToArray();
        }
        
        /// <summary>
        /// Todas las Acciones realizadas al invocar la funcion de cuarentena en Cogiscan
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="titulo"></param>
        /// <returns></returns>
        #region Acciones_Cuarentena
        //***************************//
        // Agregar item a Cuarentena //
        //***************************//
        public static bool SetQuarantine(string itemId, string titulo)
        {
            try
            {
                string[] itemInfo = WS.getContainerInfo(itemId);
                string pnKey = getPNKey(itemInfo[0]);
                string descripcion = "Esperando validación de Control de Calidad";
                string setQuarantine = "INSERT INTO CGS.\"QUARANTINE_RULE\"";
                setQuarantine += " (CREATE_TMST,CREATE_USER_ID,TITLE,DESCRIPTION,ITEM_CLASS_KEY,PART_NUMBER_KEY,ITEM_ID,QUARANTINE_RULE_ENABLED,LAST_UPDATE_TMST,LAST_UPDATE_USER_ID)";
                setQuarantine += " VALUES";
                setQuarantine += " (CURRENT_TIMESTAMP,'" + Global.userLogged + "','" + titulo + "','" + descripcion + "',1,'" + pnKey + "','" + itemId + "','Y',CURRENT_TIMESTAMP,'" + Global.userLogged + "')";
                DB2Command cmd = new DB2Command();
                cmd.Connection = ConnectDB2CGS;
                cmd.CommandText = setQuarantine;
                ConnectDB2CGS.Open();
                if (cmd.ExecuteNonQuery() == 1)
                {
                    ConnectDB2CGS.Close();
                    string lastQuarantine = "SELECT MAX(QUARANTINE_RULE_KEY) FROM CGS.\"QUARANTINE_RULE\"";
                    DataTable maxQid = new DataTable();
                    DB2DataAdapter da = new DB2DataAdapter(lastQuarantine, ConnectDB2CGS);
                    da.Fill(maxQid);
                    ConnectDB2CGSDW.Open();
                    //Inserto en tabla QUARANTINE_RULE_HIST
                    if (insertQRhistory(maxQid.Rows[0][0].ToString(), titulo, descripcion, itemInfo[0], itemId,"Y",Global.userLogged))
                    {
                        //Inserto en tabla ITEM_HISTORY_025
                        insertIhistory(itemId,"QUARANTINE LOCK",Global.userLogged);
                    }

                    ConnectDB2CGSDW.Close();

                    return true;
                }
                else return false;
                return true;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
        }
        private static bool insertQRhistory(string maxQid, string titulo, string descripcion, string partNumber, string itemId,string qRuleEnabled,string usuario)
        {
            string insertQhistory = "INSERT INTO CGS.\"QUARANTINE_RULE_HIST\"";
            insertQhistory += " (EVENT_DATE,EVENT_TMST,EVENT_TYPE,QUARANTINE_RULE_KEY,USER_ID,TITLE,TITLE_UP,DESCRIPTION,ITEM_CLASS_NAME,PART_NUMBER,ITEM_ID,QUARANTINE_RULE_ENABLED,NB_AFFECTED)";
            insertQhistory += " VALUES";
            insertQhistory += " ('" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','CREATE','" + maxQid + "','" + usuario + "','" + titulo + "','" + titulo.ToUpper() + "','" + descripcion + "','Component Lot','" + partNumber + "','" + itemId + "','"+ qRuleEnabled + "','1')";
            DB2Command cmddw = new DB2Command();
            cmddw.Connection = ConnectDB2CGSDW;
            cmddw.CommandText = insertQhistory;
            if (cmddw.ExecuteNonQuery() == 1) return true;
            else return false;
        }
        private static bool insertIhistory(string itemId, string qLock, string usuario)
        {
            DataTable itemInfo = getItemInfo(itemId);
            string insIhist = "INSERT INTO CGS.\"ITEM_HISTORY_025\"";
            insIhist += " (EVENT_DATE,EVENT_TMST,EVENT_TYPE,DEGREE,ITEM_KEY,ITEM_QUANTITY,USER_ID,ITEM_TYPE_NAME,ITEM_ID,ITEM_DESC,ITEM_PN,ITEM_SUPPLIER_NAME)";
            insIhist += " VALUES";
            insIhist += " ('" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','"+ qLock + "','0','" + itemInfo.Rows[0][0] + "','" + itemInfo.Rows[0][2] + "','" + usuario + "','REEL','" + itemId + "',' ','" + itemInfo.Rows[0][1] + "','DEFAULT')";
            DB2Command cmddw = new DB2Command();
            cmddw.Connection = ConnectDB2CGSDW;
            cmddw.CommandText = insIhist;
            cmddw.ExecuteNonQuery();
            return false;
        }
        private static string getPNKey(string pn)
        {
            string pnKey = "";
            DataTable dt = new DataTable();
            string getkey = "SELECT PART_NUMBER_KEY FROM CGS.\"PART_NUMBER\" WHERE PART_NUMBER='" + pn + "'";
            ConnectDB2CGS.Open();
            DB2DataAdapter da = new DB2DataAdapter(getkey, ConnectDB2CGS);
            da.Fill(dt);
            ConnectDB2CGS.Close();
            pnKey = dt.Rows[0][0].ToString();
            return pnKey;
        }
        private static DataTable getItemInfo(string item)
        {
            DataTable dt = new DataTable();
            string getItemKey = "SELECT I.ITEM_KEY,P.PART_NUMBER,I.QUANTITY  FROM CGS.\"ITEM\" I";
            getItemKey += " LEFT JOIN CGS.\"PART_NUMBER\" P ON P.PART_NUMBER_KEY = I.PART_NUMBER_KEY";
            getItemKey += " WHERE I.ITEM_ID = '" + item + "'";
            ConnectDB2CGS.Open();
            DB2DataAdapter da = new DB2DataAdapter(getItemKey, ConnectDB2CGS);
            da.Fill(dt);
            ConnectDB2CGS.Close();
            return dt;
        } 
        //********************************//
        // Desbloquear item de Cuarentena //
        //********************************//
        public static bool UnlockQuarantine(string itemId)
        {
            string updQuarantine = "UPDATE CGS.\"QUARANTINE_RULE\" SET QUARANTINE_RULE_ENABLED='N' WHERE ITEM_ID='"+itemId+"'";
            DB2Command cmdcgs = new DB2Command();
            cmdcgs.Connection = ConnectDB2CGS;
            cmdcgs.CommandText = updQuarantine;
            ConnectDB2CGS.Open();
            if (cmdcgs.ExecuteNonQuery() == 1)
            {
                string[] itemInfo = WS.getContainerInfo(itemId);
                ConnectDB2CGS.Close();
                string getQuarantine = "SELECT QUARANTINE_RULE_KEY FROM CGS.\"QUARANTINE_RULE\" WHERE ITEM_ID='"+itemId+"'";
                DataTable Qid = new DataTable();
                DB2DataAdapter da = new DB2DataAdapter(getQuarantine, ConnectDB2CGS);
                da.Fill(Qid);
                ConnectDB2CGSDW.Open();
                //Inserto en tabla QUARANTINE_RULE_HIST
                if (insertQRhistory(Qid.Rows[0][0].ToString(), "DESBLOQUEO DE MATERIAL", "Material validado por "+Global.vrm.QCUser, itemInfo[0], itemId, "N",Global.vrm.QCUser))
                {
                    //Inserto en tabla ITEM_HISTORY_025
                    insertIhistory(itemId, "QUARANTINE UNLOCK", Global.vrm.QCUser);
                }

                ConnectDB2CGSDW.Close();
                return true;
            }
            else
            {
                ConnectDB2CGS.Close();
                return false;
            }
        }
        #endregion
    }
}
