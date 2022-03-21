using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject_Airplane
{
    [Serializable]
    class ReservationInfoDetail
    {
        public string PassportNo { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string SeatNo { get; set; }

        public ReservationInfoDetail(string passportNo, string name, string phoneNo, string seatNo)
        {
            this.PassportNo = passportNo;
            this.Name = name;
            this.PhoneNo = phoneNo;
            this.SeatNo = seatNo;

        }

        public override string ToString()
        {
            return $"여권번호 : {this.PassportNo} | 이름 : {this.Name} | 휴대폰번호 : {this.PhoneNo} | 좌석번호 : {this.SeatNo} ";
        }
    }
}
