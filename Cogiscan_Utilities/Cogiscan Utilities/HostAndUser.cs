using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Principal;

namespace Cogiscan_Utilities
{
    class HostAndUser
    {
        
        public static string[] host()
        {
            string[] resultado= {};
            string _ip = "";
            string _host = Dns.GetHostName().ToString();
            string _user = WindowsIdentity.GetCurrent().Name.Split('\\').Last();
            IPAddress[] _ipaddressList = Dns.GetHostAddresses(_host);
            foreach (IPAddress ip in _ipaddressList)
            {
                if (ip.ToString().Split('.').Count() == 4) _ip = ip.ToString();
            }
            resultado = new string[3] { _host,_user,_ip};
            return resultado;
        }
    }
}
