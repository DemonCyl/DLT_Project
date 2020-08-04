using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLT_Project.Entity
{
    public class MesInfo
    {

        public string BarCode { get; set; }

        public int Type { get; set; }

        public float WalkInLight { get; set; }

        public float Heater { get; set; }

        public int Bukle { get; set; }

        public float Safety { get; set; }

        public float SBROff { get; set; }

        public float SBROn { get; set; }

        public override string ToString()
        {
            return "Barcode:"+BarCode;
        }
    }
}
