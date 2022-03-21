using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject_Airplane
{
    [Serializable]
    class Airplane
    {
        public string AirplaneNo { get; set; }
        public string AirplaneKind { get; set; }
        public string[] SeatList { get; set; }
        public Airplane() { }

    }
}
