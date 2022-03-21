using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TeamProject_Airplane
{
    [Serializable]
    class NonMember : IReservation
    {
        private Dictionary<string, ReservationInfo> reservationInfos;
        private Dictionary<string, AirplaneSchedule> airplaneSchedules;

        public NonMember(Dictionary<string, ReservationInfo> reservationInfos, Dictionary<string, AirplaneSchedule> airplaneSchedules)
        {
            this.reservationInfos = reservationInfos;
            this.airplaneSchedules = airplaneSchedules;
        }
        // 예약을 한다 
        public void addReservation()
        {
            // 일정 목록을 보여준다
            Console.WriteLine("[알림] 현재 비행기 일정 목록");
            checkSchedule();
            Console.Write("원하는 비행일정의 비행기넘버를 입력해주세요 : "); 
            string wantThisSchedule = Console.ReadLine();
            //그런비행기가 있는지 검색
            if (!this.airplaneSchedules.ContainsKey(wantThisSchedule))
            {
                Console.WriteLine("[알림] 해당 비행기넘버를 가진 일정은 없습니다");
                return;
            }

            AirplaneSchedule airplaneSchedule = this.airplaneSchedules[wantThisSchedule];
            // 자리 현황을 보여주고 인원수를 선택한다

            // seatList : 스케쥴의 현재 좌석 현황 
            int[,] seatList = airplaneSchedule.SeatList;
            Console.WriteLine("[알림] 좌석 현황\n");
            for (int i = 0; i < seatList.GetLength(0); i++)
            {
                for (int j = 0; j < seatList.GetLength(1); j++)
                {
                    var item = seatList[i, j];
                    Console.Write(item != 0 ? "[매진]\t" : $"[{i + 1}-{j + 1}]\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine("==============================================");
            while (true)
            {

                // 인원수 선택 
                Console.Write("인원수를 선택해 주세요(나가기 : q) : ");

                string peopleNum_str = Console.ReadLine();
                if(peopleNum_str == "q" || peopleNum_str == "Q") return;
                try
                {
                    int peopleNum_int = int.Parse(peopleNum_str);

                    if (peopleNum_int == 0)
                    {
                        Console.WriteLine("[알림] 1 이상 입력해 주세요");
                        continue;
                    }
                    // 인원수만큼 while문
                    int count = 0;

                    // 자리 입력받은거 저장 
                    List<int[]> seat_list = new List<int[]>();
                    while (count < peopleNum_int)
                    {
                        Console.Write($"[{count + 1}] 원하는 자리를 선택해 주세요(예 : 1-1)(나가기 : q) : ");
                        string seat_str = Console.ReadLine();
                        if(seat_str == "q" || seat_str == "Q") return;
                        string[] seat_arr = seat_str.Split('-');

                        try
                        {
                            int i = int.Parse(seat_arr[0]);
                            int j = int.Parse(seat_arr[1]);

                            // 범위안에있고, 예매 가능한 자리이면 
                            if ((i <= seatList.GetLength(0) && i >= 1) && (j <= seatList.GetLength(1) && j >= 1))
                            {
                                if (seatList[i - 1, j - 1] == 0)
                                {
                                    seat_list.Add(new int[] { i, j });
                                    count++;

                                }

                                else Console.WriteLine("[알림] 매진된 좌석입니다");


                            }

                            // 범위안에 없으면 다시 입력 받음
                            else Console.WriteLine("[알림] 존재하는 자리인지 확인해 주세요");



                        }
                        catch (Exception e) // 알맞은 형태로 입력 받았는지 
                        {
                            Console.WriteLine("알맞은 형태로 입력해 주세요(예 : 1-1)");
                            continue;
                        }
                    }

                    Console.WriteLine("[알림] 좌석 선택이 완료되었습니다");

                    // 자리 받기완료 !
                    // 정보 입력 시작
                    count = 0;
                    List<ReservationInfoDetail> reservationInfoDetails = new List<ReservationInfoDetail>();
                    
                    Regex passportRegex = new Regex(@"^([M,S,R,G,D][ ]?)[0-9]{8}$"); //여권번호 정규식
                    Regex phoneRegex = new Regex(@"(^01[167]\d{7,8}$)|(^010\d{8}$)"); //핸드폰 정규식
                    Regex emailRegex = new Regex(@"^[a-zA-z]([._]?[a-zA-Z0-9]){1,15}@([a-zA-Z0-9]){1,15}\.(com|kr|net|jp|en|ru)$"); //이메일 정규식

                    while (count < peopleNum_int)
                    {
                        int[] seat = seat_list[count];
                        Console.WriteLine($"[알림] [{seat[0]}-{seat[1]}]좌석의 정보를 입력받습니다");

                        string passportNo = "";
                        string phoneNo = "";
                            
                        while (true)
                        {
                            Console.Write("여권번호(예 : M 12345678) : ");
                            passportNo = Console.ReadLine();
                            if(!passportRegex.IsMatch(passportNo)) Console.WriteLine("[알림] 형식이 알맞지 않습니다");
                            else break;
                        }


                        Console.Write("이름 : ");
                        string name = Console.ReadLine();

                        while (true)
                        {   Console.Write("휴대폰번호( - 기호 없이 입력) : ");
                            phoneNo = Console.ReadLine();
                            if(!phoneRegex.IsMatch(phoneNo)) Console.WriteLine("[알림] 형식이 알맞지 않습니다");
                            else break;

                        }

                        ReservationInfoDetail reservationInfoDetail = new ReservationInfoDetail(passportNo, name, phoneNo, $"{seat[0]}-{seat[1]}");

                        reservationInfoDetails.Add(reservationInfoDetail);

                        count++;

                        // 스케쥴에도 좌석 
                        airplaneSchedule.SeatList[seat[0] - 1, seat[1] - 1] = 1;


                    }

                    Console.WriteLine("[알림] 예매를 완료하였습니다");

                    // 예매 생성 
                    ReservationInfo reservationInfo = new ReservationInfo(peopleNum_int, reservationInfoDetails, airplaneSchedule);
                    this.reservationInfos.Add(reservationInfo.reservationNo, reservationInfo);

                    // 출력 
                    printReservationInfo(reservationInfo);

                    break;



                }
                catch (Exception e1) // int 에러로 수정 
                {
                    Console.WriteLine("[알림] 숫자만 입력해 주세요");
                    continue;
                }
            }
        }


        // 예약을 확인한다 
        public void checkReservation(string reservationNo)
        {
            if (this.reservationInfos.ContainsKey(reservationNo))
                printReservationInfo(this.reservationInfos[reservationNo]);
            else Console.WriteLine("[알림] 해당 예매번호로 예매가 되어있지 않습니다");

        }


        // 예약을 수정한다 
        public void editReservation(string reservationNo)
        {
            // 예매 번호를 가지고있는 예매 정보를 가져온다 
            if (!this.reservationInfos.ContainsKey(reservationNo))
            {
                Console.WriteLine("[알림] 해당 예매번호로 예매가 되어있지 않습니다");
                return;
            }
            ReservationInfo reservationInfo = this.reservationInfos[reservationNo];
            AirplaneSchedule airplaneSchedule = reservationInfo.airplaneSchedule;

            // seatList : 스케쥴의 현재 좌석 현황 
            int[,] seatList = airplaneSchedule.SeatList;

            Console.WriteLine("[알림] 좌석 현황\n");
            for (int i = 0; i < seatList.GetLength(0); i++)
            {
                for (int j = 0; j < seatList.GetLength(1); j++)
                {
                    var item = seatList[i, j];
                    if (item != 0)
                    {
                        for (int k = 0; k < reservationInfo.count; k++)
                        {
                            string seatNo_str = reservationInfo.reservationInfoDetail[k].SeatNo;
                            //Console.WriteLine($"자리번호 : {seatNo_str}");
                            string[] seatNo_list = seatNo_str.Split('-');

                            int num1 = int.Parse(seatNo_list[0]);
                            int num2 = int.Parse(seatNo_list[1]);

                            // null이 아니어도 예매한 정보이면 보여준다 
                            if (num1 - 1 == i && num2 - 1 == j)
                            {
                                Console.Write($"[{num1}-{num2}]\t");
                                break;
                            }
                            if (k == reservationInfo.count - 1) Console.Write("[매진]\t");

                        }

                    }
                    else Console.Write($"[{i + 1}-{j + 1}]\t");

                }
                Console.WriteLine();
            }

            Console.WriteLine("==============================================");

            // 나가기 할때까지 계속..
            while (true)
            {
                Console.WriteLine("[알림] 예매 상세 정보 확인");
                for (int i = 0; i < reservationInfo.count; i++)
                {
                    Console.WriteLine($"[{i + 1}] {reservationInfo.reservationInfoDetail[i]}");
                }
                Console.Write("변경할 번호를 입력해 주세요(나가기 : q) : ");
                string num_str = Console.ReadLine();

                if (num_str == "q" || num_str == "Q") break;

                try
                {
                    int num_int = int.Parse(num_str);

                    if (num_int > 0 && num_int <= reservationInfo.count)
                    {
                        Console.Write("원하는 자리를 선택해 주세요(예 : 1-1) : ");
                        string seat_str = Console.ReadLine();
                        string[] seat_arr = seat_str.Split('-');

                        try
                        {
                            int i = int.Parse(seat_arr[0]);
                            int j = int.Parse(seat_arr[1]);

                            // 범위안에있고, 예매 가능한 자리이거나 내가 예매한 번호중에 있다면 
                            if ((i <= seatList.GetLength(0) && i >= 1) && (j <= seatList.GetLength(1) && j >= 1))
                            {


                                if (seatList[i - 1, j - 1] == 0)
                                {
                                    string prevSeatNo_str = reservationInfo.reservationInfoDetail[num_int - 1].SeatNo;
                                    string[] prevSeat_arr = prevSeatNo_str.Split('-');
                                    // 전에 좌석은 0을 넣는다  
                                    seatList[int.Parse(prevSeat_arr[0]) - 1, int.Parse(prevSeat_arr[1]) - 1] = 0;

                                    reservationInfo.reservationInfoDetail[num_int - 1].SeatNo = seat_str;

                                    // 바뀐 좌석은 1을 넣는다
                                    seatList[i - 1, j - 1] = 1;
                                }

                                else
                                {
                                    for (int k = 0; k < reservationInfo.count; k++)
                                    {
                                        string seatNo_str = reservationInfo.reservationInfoDetail[k].SeatNo;
                                        string[] seatNo_list = seatNo_str.Split('-');

                                        int num1 = int.Parse(seatNo_list[0]);
                                        int num2 = int.Parse(seatNo_list[1]);

                                        // null이 아니어도 예매한 정보이면 수정한다.
                                        if (num1 == i && num2 == j)
                                        {
                                            string prevSeatNo_str = reservationInfo.reservationInfoDetail[num_int - 1].SeatNo;
                                            string[] prevSeat_arr = prevSeatNo_str.Split('-');
                                            // 전에 좌석은 0을 넣는다  
                                            seatList[int.Parse(prevSeat_arr[0]) - 1, int.Parse(prevSeat_arr[1]) - 1] = 0;

                                            reservationInfo.reservationInfoDetail[num_int - 1].SeatNo = seat_str;

                                            // 바뀐 좌석은 1을 넣는다
                                            seatList[i - 1, j - 1] = 1;

                                            break;

                                        }
                                        if (k == reservationInfo.count - 1) Console.WriteLine("[알림] 매진된 좌석입니다");

                                    }

                                }

                            }

                            // 범위안에 없으면 다시 입력 받음
                            else Console.WriteLine("[알림] 존재하는 자리인지 확인해 주세요");



                        }
                        catch (Exception e) // 알맞은 형태로 입력 받았는지 
                        {
                            Console.WriteLine("[알림] 알맞은 형태로 입력해 주세요(예 : 1-1)");
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[알림] 숫자만 입력해 주세요");
                    continue;
                }

            }

            Console.WriteLine("[알림] 수정이 완료되었습니다");
            printReservationInfo(reservationInfo);



        }
        // 예약을 취소한다
        public void cancelReservation(string reservationNo)
        {
            if (this.reservationInfos.ContainsKey(reservationNo))
            {
                ReservationInfo reservationInfo = this.reservationInfos[reservationNo];
                printReservationInfo(reservationInfo);
                bool auto = true;
                while (auto)
                {
                    Console.Write("해당 예매를 취소하시겠습니까?(Y/N) : ");
                    switch (Console.ReadLine())
                    {
                        case "y":
                        case "Y":
                            cancelSeat(reservationInfo);
                            auto = false;

                            return;
                        case "n":
                        case "N":
                            auto = true;
                            return;
                        default:
                            Console.WriteLine("다시 입력해 주세요");
                            break;
                    }
                }
            }
            else Console.WriteLine("[알림] 해당 예매번호로 예매가 되어있지 않습니다");

        }

        // 자리 번호를 이용하여 schedule의 seats에서 제거하고
        // Dictionary 에서 지운다 
        private void cancelSeat(ReservationInfo reservationInfo)
        {
            // schedule의 seats에서 제거
            foreach (var item in reservationInfo.reservationInfoDetail)
            {
                string[] idx_list = item.SeatNo.Split('-');
                reservationInfo.airplaneSchedule.SeatList[int.Parse(idx_list[0]) - 1, int.Parse(idx_list[1]) - 1] = 0;
            }

            // reservationInfos에서 제거
            this.reservationInfos.Remove(reservationInfo.reservationNo);

            Console.WriteLine("[알림] 예매를 취소하였습니다");
        }


        private void printReservationInfo(ReservationInfo reservationInfo)
        {

            Console.WriteLine("\n[알림] 예매 좌석 정보");
            for (int i = 0; i < reservationInfo.count; i++)
                Console.WriteLine($"[{i + 1}] {reservationInfo.reservationInfoDetail[i]}");


            Console.WriteLine("\n[알림] 예매 정보");
            Console.WriteLine($"예매번호 : {reservationInfo.reservationNo}");
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

            if (this.airplaneSchedules.Count > 0) //로딩이 있거나 데이터 파일이 있다는거 그럼 검증하고 출력
            {
                for (int i = 0; i < 7; i++) // 현재 시간으로부터 7일 더하고 list에 add
                {
                    Console.WriteLine($"###########################[{DateTime.Now.AddDays(i).ToString("yy/MM/dd")}]###########################\n\n\n");
                    for (int j = 0; j <= 24; j++) // 시간 출력
                    {
                        Console.Write($"{string.Format("{0:D2}", j)}시:     ");
                        foreach (var item in this.airplaneSchedules)
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
        }
    }
}