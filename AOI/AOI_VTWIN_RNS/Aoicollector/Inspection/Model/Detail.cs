﻿using System.Collections.Generic;
using System.IO;

namespace CollectorPackage.Aoicollector.Inspection
{
    public class Detail
    {
        public int componentNo;
        public int bloqueId;

        public string referencia;
        public string faultcode;
        public string realFaultcode;
        public string descripcionFaultcode;
        public string estado;
        public string barcode;

        public List<FileInfo> faultImages;
    }
}
