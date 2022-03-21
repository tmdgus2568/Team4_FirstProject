using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject_Airplane
{
    [Serializable]
    class NonMember : IReservation
    {
        private Dictionary<string, ReservationInfo> reservationInfos;

        public NonMember(Dictionary<string, ReservationInfo> reservationInfos)
        {
            this.reservationInfos = reservationInfos;
        }
        // 예약을 한다 
        public void addReservation(AirplaneSchedule airplaneSchedule)
        {
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

            Console.WriteLine("=================================");
            while (true)
            {

                // 인원수 선택 
                Console.Write("인원수를 선택해 주세요 : ");

                string peopleNum_str = Console.ReadLine();
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
                        Console.Write($"[{count + 1}] 원하는 자리를 선택해 주세요(예 : 1-1) : ");
                        string seat_str = Console.ReadLine();
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
                    while (count < peopleNum_int)
                    {
                        int[] seat = seat_list[count];
                        Console.WriteLine($"[알림] [{seat[0]}-{seat[1]}]좌석의 정보를 입력받습니다");

                        Console.Write("여권번호 : ");
                        string passportNo = Console.ReadLine();

                        Console.Write("이름 : ");
                        string name = Console.ReadLine();

                        Console.Write("휴대폰번호 : ");
                        string phoneNo = Console.ReadLine();

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

            Console.WriteLine("=================================");

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
    }
}