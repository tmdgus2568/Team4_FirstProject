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

    }

}

