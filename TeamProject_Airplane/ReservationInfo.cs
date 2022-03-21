using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject_Airplane
{
    [Serializable]
    class ReservationInfo
    {

        public string reservationNo { get; set; }
        public int count { get; set; }
        public Member member { get; set; }
        public List<ReservationInfoDetail> reservationInfoDetail { get; set; }
        public AirplaneSchedule airplaneSchedule { get; set; }

        public ReservationInfo(int count, Member member, List<ReservationInfoDetail> reservationInfoDetail, AirplaneSchedule airplaneSchedule)
        {
            string uuid = Guid.NewGuid().ToString();
            this.reservationNo = uuid.Split('-')[4];
            this.count = count;
            this.member = member;
            this.reservationInfoDetail = reservationInfoDetail;
            this.airplaneSchedule = airplaneSchedule;
        }

        public ReservationInfo(int count, List<ReservationInfoDetail> reservationInfoDetail, AirplaneSchedule airplaneSchedule)
        {
            string uuid = Guid.NewGuid().ToString();
            this.reservationNo = uuid.Split('-')[4];
            this.count = count;
            this.reservationInfoDetail = reservationInfoDetail;
            this.airplaneSchedule = airplaneSchedule;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(); // 보조용. 여러 군데서 쓰는 예매내역 출력 
       
            
            if (this.member != null)
                sb.Append($"해당 예매번호 대표자 : {this.member.Name}\n");
            else
                sb.Append($"해당 예매번호 대표자 : 비회원\n");
            sb.Append($"해당 예매번호로 예매한 자리의 수 : {this.count}\n");
            sb.Append($"해당 예매번호로의 비행일정 : \n");
            sb.Append($"출발지 : 인천 출발시간 : {this.airplaneSchedule.TakeOffTime}\n");
            sb.Append($"도착지 : {this.airplaneSchedule.DestinationPoint} 도착시간 : {this.airplaneSchedule.Eta}\n");

            sb.Append($"\n해당 예매번호로 예매한 자리 상세정보 : ");

            foreach (var item in this.reservationInfoDetail)
            {
                sb.Append(item.SeatNo).Append("\n");
                //Console.WriteLine(item.SeatType);
                sb.Append($"탑승자 여권번호 : {item.PassportNo}\n");
                sb.Append($"탑승자 이름 : {item.Name}\n");
                sb.Append($"탑승자 연락처 : {item.PhoneNo}\n");
            }
        
            return sb.ToString();
        }
    }

}

