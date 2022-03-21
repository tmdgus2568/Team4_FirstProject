using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; // nuget에서 패키지 설치 Json 다루는 가장 많은 네임스페이스
using System.Net; // Http 응답 네임스페이스
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace TeamProject_Airplane
{
    [Serializable]
    class AirplaneSchedule
    {
        public string AirplanceNo { get; set; } //스펠링?
        public string TakeOffTime { get; set; }
        public string Eta { get; set; }
        public string DeparturePoint { get; set; }
        public string DestinationPoint { get; set; }

        private Dictionary<string, Captain> captains;

       // private MainSystem mainSystem;
        public Captain CaptainInfo;
        public Captain CoCaptainInfo;
        public int PriceInfo { get; set; }
        public Dictionary<string, string> AviationWeatherService { get; set; }
        public int[,] SeatList { get; set; }
        public AirplaneSchedule()
        {


        }
        public AirplaneSchedule(string airplanceNo, string takeOffTime, string eta, string destinationPoint, Captain captainInfo, Captain coCaptainInfo, int priceInfo)
        {
            this.AirplanceNo = airplanceNo;
            this.TakeOffTime = takeOffTime;
            this.Eta = eta;
            this.DestinationPoint = destinationPoint;
            this.CaptainInfo = captainInfo;
            this.CoCaptainInfo = coCaptainInfo;
            this.PriceInfo = priceInfo;
            this.SeatList = new int[30, 6];
        }
    }

}
