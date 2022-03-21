using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

//비행기번호+선착순카운트
namespace TeamProject_Airplane
{
    [Serializable]
    class Member : IReservation
    {
        public string PassportNo { get; set; }
        public string ID { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string MemberRating { get; private set; } // 고객등급
        public int Point { get; private set; } // 마일리지

        [NonSerialized]
        public Dictionary<string, ReservationInfo> ReservationInfos;
        [NonSerialized]
        public Dictionary<string, AirplaneSchedule> AirplaneSchedules;


        public Member(string passportNo, string id, string password, string name, string phoneNo, string email, string memberRating, int point, Dictionary<string, ReservationInfo> ReservationInfos, Dictionary<string, AirplaneSchedule> AirplaneSchedules)
        {

            this.Name = name;
            this.PassportNo = passportNo;
            this.ID = id;
            this.Password = password;
            this.PhoneNo = phoneNo;
            this.Email = email;
            this.MemberRating = memberRating;
            this.Point = point;
            this.ReservationInfos = ReservationInfos;
            this.AirplaneSchedules = AirplaneSchedules;

        }

        public void addReservation()
        {
            //비행기 먼저 골라야됨
            Console.WriteLine("현재 비행기 일정 목록");
            checkSchedule();
            Console.WriteLine("원하는 비행일정의 비행기넘버를 입력해주세요"); string wantThisSchedule = Console.ReadLine();
            //그런비행기가 있는지 검색
            if (!AirplaneSchedules.ContainsKey(wantThisSchedule))
            {
                Console.WriteLine("그런 비행기 엄슴");
                return;
            }


            //전체 자리목록 출력, 그렇게 해야 이거 보고 예약
            printAllSeats(wantThisSchedule);
            //while문 써서 반복돌리고 종료로 빠져나가기 만들어야됨?

            //예약하고 싶은 자리 수
            Console.WriteLine("예약할 자리 수를 입력해주세요");
            int count = int.Parse(Console.ReadLine());

            //예약하고 싶은 좌석들 입력받음 // 일단 여러명 기준으로 생각
            //예매상세정보 생성
            List<string[]> seatWantList = new List<string[]>(); // 예약하고 싶은 좌석들 담을거

            for (int i = 0; i < count; i++)
            {
                Console.WriteLine("예약할 자리위치를 입력해주세요.");
                Console.WriteLine("여러개의 자리를 예약하실 경우 제일 처음입력하는 자리가 대표회원님의 자리가 됩니다.");
                string[] seatNoWantToReserve = (Console.ReadLine()).Split('-'); // 좌석값 입력값 담을 통

                if (AirplaneSchedules[wantThisSchedule].SeatList[int.Parse((seatNoWantToReserve[0])) - 1, int.Parse(seatNoWantToReserve[1]) - 1] == 1) //? +1 또는 -1해야될수도있음
                {
                    Console.WriteLine("이미 예약됨");
                    return;
                }
                seatWantList.Add(seatNoWantToReserve); // 이상없는 자리니 담아줌

            }

            //희망한 자리들이 전부 빈자리. 고로 입력시작
            List<ReservationInfoDetail> reservedList = new List<ReservationInfoDetail>(); //출력 때도 쓸 reservationInfoDetail 새 리스트
                                                                                          //먼저 당사자꺼
            reservedList.Add(new ReservationInfoDetail(this.PassportNo, this.Name, this.PhoneNo, seatWantList[0][0] + "-" + seatWantList[0][1])); // 주의체크
            //예매번호
            ReservationInfo reservationInfo = new ReservationInfo(count, this, reservedList, AirplaneSchedules[wantThisSchedule]);
            ReservationInfos.Add(reservationInfo.reservationNo, reservationInfo);
            string reservedNumber = reservationInfo.reservationNo;
            AirplaneSchedules[wantThisSchedule].SeatList[int.Parse(seatWantList[0][0]) - 1, int.Parse(seatWantList[0][1]) - 1] = 1;


            //나머지 사람들
            for (int i = 1; i < count; i++)
            {
                Console.WriteLine($"{seatWantList[i][0]}-{seatWantList[i][1]}의 자리를 입력해주세요");
                Console.WriteLine("여권번호를 입력해주세요"); string passport = Console.ReadLine();
                Console.WriteLine("이름을 입력해주세요"); string name = Console.ReadLine();
                Console.WriteLine("전화번호를 입력해주세요"); string phoneNo = Console.ReadLine();

                ReservationInfos[reservedNumber].reservationInfoDetail.Add(new ReservationInfoDetail(passport, name, phoneNo, seatWantList[i][0] + "-" + seatWantList[i][1]));
                AirplaneSchedules[wantThisSchedule].SeatList[int.Parse(seatWantList[i][0]) - 1, int.Parse(seatWantList[i][1]) - 1] = 1;
            }


            //예약완료 후 내역 출력
            Console.WriteLine("예매과정이 완료되었습니다.");
            printReservationInfoDetail(reservedNumber);
            Console.WriteLine($"예매번호 : {reservedNumber}");

            //마일리지 적립
            double mile = (double)AirplaneSchedules[wantThisSchedule].PriceInfo;

            switch (this.MemberRating)
            {
                case "Diamond":
                    mile *= 0.1;
                    break;
                case "Vip":
                    mile *= 0.15;
                    break;
                default:
                    mile *= 0.2;
                    break;
            }

            this.Point += (int)mile;
        }


        public void checkReservation(string reservationNo) // 예매번호로 내역 출력
        {
            Console.WriteLine("예매번호를 입력해주세요"); string input = Console.ReadLine();
            if (!ReservationInfos.ContainsKey(input))
            {
                Console.WriteLine("입력된 예매번호가 없습니다.");
                return;

            }
            else
            {
                printReservationInfoDetail(input);
            }

        }


        public void editReservation(string reservationNo)
        {
            Console.WriteLine("예매번호를 입력해주세요"); string reservedNo = Console.ReadLine();
            if (!ReservationInfos.ContainsKey(reservedNo))
            {
                Console.WriteLine("입력된 예매번호가 없습니다.");
                return;

            }
            else
            {
                //예매한 내역 출력
                printReservationInfoDetail(reservedNo);

                Console.WriteLine("변경하고 싶은 사람은 몇 명입니까?"); int count = int.Parse(Console.ReadLine());
                Console.WriteLine("변경하고 싶은 사람의 여권번호를 입력해주세요");
                List<string> changelist = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine("입력");
                    string temp = Console.ReadLine();
                    changelist.Add(temp);
                }

                //변경시작
                //좌석표도 출력
                printAllSeats(ReservationInfos[reservedNo].airplaneSchedule.AirplanceNo);


                foreach (var item in changelist)//여권번호 들어잇는
                {
                    Console.WriteLine($"변경할 사람의 여권번호 : {item}");
                    Console.WriteLine("바꿀 좌석을 고르세요");
                    string[] input = Console.ReadLine().Split('-');


                    //여기수정
                    if (AirplaneSchedules[ReservationInfos[reservedNo].airplaneSchedule.AirplanceNo].SeatList[int.Parse(input[0]) - 1, int.Parse(input[1]) - 1] == 1)
                    {
                        Console.WriteLine("이 자리는 비어있지 않습니다.");
                        return;
                    }


                    //이전기록 삭제

                    string erasedName = "";
                    string erasedPhone = "";
                    string[] erasedSeatNo = input; //초기화값, 의미없음
                    foreach (var item2 in ReservationInfos[reservedNo].reservationInfoDetail)
                    {
                        if (item2.PassportNo == item && item2.SeatNo != input[0] + "-" + input[1])
                        {
                            erasedName = item2.Name;
                            erasedPhone = item2.PhoneNo;
                            erasedSeatNo = item2.SeatNo.Split('-');
                            ReservationInfos[reservedNo].reservationInfoDetail.Remove(item2);
                            break;
                        }
                    }
                    AirplaneSchedules[ReservationInfos[reservedNo].airplaneSchedule.AirplanceNo].SeatList[int.Parse(erasedSeatNo[0]) - 1, int.Parse(erasedSeatNo[1]) - 1] = 0;

                    //새 기록 추가
                    ReservationInfos[reservedNo].reservationInfoDetail.Add(new ReservationInfoDetail(item, erasedName, erasedPhone, input[0] + "-" + input[1]));
                    AirplaneSchedules[ReservationInfos[reservedNo].airplaneSchedule.AirplanceNo].SeatList[int.Parse(input[0]) - 1, int.Parse(input[1]) - 1] = 1;


                }
                //변경된 예매내역 출력
                printReservationInfoDetail(reservedNo);
            }
        }


        public void cancelReservation(string reservationNo)
        {
            Console.WriteLine("예매번호를 입력해주세요"); string reservedNo = Console.ReadLine();
            if (!ReservationInfos.ContainsKey(reservedNo))
            {
                Console.WriteLine("입력된 예매번호가 없습니다.");
                return;

            }
            else
            {
                //마일리지 취소
                double mile = (double)AirplaneSchedules[ReservationInfos[reservedNo].airplaneSchedule.AirplanceNo].PriceInfo;

                switch (this.MemberRating)
                {
                    case "Diamond":
                        mile *= 0.1;
                        break;
                    case "Vip":
                        mile *= 0.15;
                        break;
                    default:
                        mile *= 0.2;
                        break;
                }

                this.Point -= (int)mile;


                foreach (var item in ReservationInfos[reservedNo].reservationInfoDetail)
                {
                    string[] idx = item.SeatNo.Split('-');
                    AirplaneSchedules[ReservationInfos[reservedNo].airplaneSchedule.AirplanceNo].SeatList[int.Parse(idx[0]) - 1, int.Parse(idx[1]) - 1] = 0;
                }
                ReservationInfos.Remove(reservedNo);
                Console.WriteLine($"예매번호 {reservedNo}의 모든 예매가 취소되었습니다.");
            }


        }

        public void printReservationInfoDetail(string input) // 보조용. 여러 군데서 쓰는 예매내역 출력 
        {
            if (ReservationInfos[input].member != null)
                Console.WriteLine($"해당 예매번호 대표자 : {ReservationInfos[input].member.Name}");
            else
                Console.WriteLine($"해당 예매번호 대표자 : 비회원");
            Console.WriteLine($"해당 예매번호로 예매한 자리의 수 : {ReservationInfos[input].count}");
            Console.WriteLine($"해당 예매번호로의 비행일정 : ");
            Console.WriteLine($"출발지 : 인천 출발시간 : {ReservationInfos[input].airplaneSchedule.TakeOffTime}");
            Console.WriteLine($"도착지 : {ReservationInfos[input].airplaneSchedule.DestinationPoint} 도착시간 : {ReservationInfos[input].airplaneSchedule.Eta}");

            Console.WriteLine($"\n해당 예매번호로 예매한 자리 상세정보 : ");

            foreach (var item in ReservationInfos[input].reservationInfoDetail)
            {
                Console.WriteLine(item.SeatNo);
                //Console.WriteLine(item.SeatType);
                Console.WriteLine($"탑승자 여권번호 : {item.PassportNo}");
                Console.WriteLine($"탑승자 이름 : {item.Name}");
                Console.WriteLine($"탑승자 연락처 : {item.PhoneNo}");
            }
        }

        public void printAllSeats(string wantThisSchedule)
        {
            //고른 스케쥴의 현재 좌석 현황
            int[,] seatList = AirplaneSchedules[wantThisSchedule].SeatList;
            for (int i = 0; i < seatList.GetLength(0); i++)
            {
                for (int j = 0; j < seatList.GetLength(1); j++)
                {
                    var item = seatList[i, j];
                    Console.Write(item != 0 ? "[매진]\t" : $"[{i + 1}-{j + 1}]\t");
                }
                Console.WriteLine();
            }
        }
        public void checkSchedule() // 비행기 일정표 확인 (고객용)
        {
            Console.WriteLine("###############################################현재 비행기 일정###############################################");
            Regex regDay = new Regex(@"(\d\d)-(\d\d)-(\d\d)");
            Regex regHour = new Regex(@"(\d\d)$");
            MatchCollection resultDay;
            MatchCollection resultHour;
            string matchEtaDay = "";
            string matchEtaHour = "";

            if (this.AirplaneSchedules.Count > 0) //로딩이 있거나 데이터 파일이 있다는거 그럼 검증하고 출력
            {
                for (int i = 0; i < 7; i++) // 현재 시간으로부터 7일 더하고 list에 add
                {
                    Console.WriteLine($"###########################[{DateTime.Now.AddDays(i).ToString("yy/MM/dd")}]###########################\n\n\n");
                    for (int j = 0; j <= 24; j++) // 시간 출력
                    {
                        Console.Write($"{string.Format("{0:D2}", j)}시:     ");
                        foreach (var item in this.AirplaneSchedules)
                        {
                            resultDay = regDay.Matches(item.Value.TakeOffTime);
                            resultHour = regHour.Matches(item.Value.TakeOffTime);
                            foreach (var itemDay in resultDay)
                            {

                                matchEtaDay = itemDay.ToString();
                            }
                            foreach (var itemHour in resultHour)
                            {
                                matchEtaHour = itemHour.ToString();
                            }

                            if (DateTime.Now.AddDays(i).ToString("yy/MM/dd") == matchEtaDay && j.ToString() == matchEtaHour)
                            {
                                Console.Write($"비행기 번호: {item.Value.AirplanceNo}  출발예정시간:{item.Value.TakeOffTime}  도착예정시간:{item.Value.Eta}  출발예정시간:{item.Value.TakeOffTime}  목적지:{item.Value.DestinationPoint}  티켓 가격:{item.Value.PriceInfo}");
                            }
                        }
                        Console.WriteLine("\n");
                    }
                }
            }
            else // 로딩 파일이 없을경우 생성
            {
                for (int i = 0; i < 7; i++) // 현재 시간으로부터 7일 더하고 list에 add
                {
                    Console.WriteLine($"###########################[{DateTime.Now.AddDays(i).ToString("yy/MM/dd")}]###########################\n\n\n");
                    for (int j = 0; j <= 24; j++) // 시간 출력
                    {
                        Console.WriteLine($"{string.Format("{0:D2}", j)}시:     \n\n");
                    }
                }
            }

        }

    }

}