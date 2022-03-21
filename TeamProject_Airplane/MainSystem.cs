using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace TeamProject_Airplane
{
    [Serializable]
    class MainSystem
    {
        private Dictionary<string, Member> members;
        private Dictionary<string, Manager> managers;
        private Dictionary<string, ReservationInfo> reservationInfos;
        private Dictionary<string, AirplaneSchedule> airplaneSchedules;
        private List<ReservationInfoDetail> reservationInfoDetails;

        public MainSystem()
        {
            this.loadAirplaneSchedule();
            this.loadReservationInfoDetail();
            this.loadReservationInfo();
            this.loadManagerlist();
            this.loadMemberlist();         
            this.display();

        }


        private void display() //생성시 시작함수
        {
            while (true)
            {
                Console.WriteLine("저희 항공사를 이용해주셔서 감사합니다.");
                Console.WriteLine("=====================================\n");
                Console.WriteLine("원하시는 서비스를 선택해 주세요.\n");
                Console.WriteLine("1. 로그인 2. 비회원 3. 회원가입 0. 프로그램 종료");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        this.login();
                        break;
                    case "2":
                        this.selectNonmembermenu();
                        break;
                    case "3":
                        this.signUp();
                        break;
                    case "0":
                        Console.WriteLine("이용해 주셔서 감사합니다.(__)");
                        return;
                    default:
                        Console.WriteLine("잘못 입력하셨습니다.");
                        break;
                }
            }
        }


        private void login() //관리자, 회원 둘 다 로그인
        {
            Console.Write("아이디: ");
            string id = Console.ReadLine();
            Console.Write("비밀번호: ");
            string password = Console.ReadLine();


            if (this.managers.ContainsKey(id)) //관리자 로그인  //아이디가 있는지? true/false
            {
                if (this.managers[id].Password == password) //id, 로그인 일치 시..
                {
                    Console.WriteLine($"반갑습니다. 관리자 {id}님 ^^");
                    this.managers[id].airplaneSchedules = this.airplaneSchedules; //객체주소 동기화
                    this.managers[id].reservationInfos = this.reservationInfos;
                    this.selectManagermenu(id);
                }
            }
            else if (this.members.ContainsKey(id)) //회원 로그인
            {
                if (this.members[id].Password == password)
                {
                    Console.WriteLine($"반갑습니다. 회원 {id}님 ^^");
                    this.members[id].AirplaneSchedules = this.airplaneSchedules;//객체주소 동기화
                    this.members[id].ReservationInfos = this.reservationInfos;//객체주소 동기화
                    this.selectMembermenu(id);
                }

            }
            else //아이디가 없을 경우
            {
                Console.WriteLine("[알림] 아이디가 존재하지 않습니다. 메인화면으로 돌아갑니다.");
            }

        }


        private void selectManagermenu(string id) //관리자 업무 창
        {
            Manager manager = this.managers[id];

            Console.WriteLine($"원하시는 업무를 선택해 주세요.");
            Console.WriteLine($"1. 비행 일정표 확인\t2. 운행 일정 생성\t3. 운행 일정 수정\n");
            Console.WriteLine($"4. 운행 일정 삭제\t5. 항공기상정보 확인\t0. 프로그램 종료");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    manager.checkSchedule();
                    this.selectManagermenu(id);
                    break;
                case "2":
                    manager.addSchedule();
                    this.selectManagermenu(id);
                    break;
                case "3":
                    manager.editSchedule();
                    this.selectManagermenu(id);
                    break;
                case "4":
                    manager.deleteSchedule();
                    this.selectManagermenu(id);
                    break;
                case "5":
                    manager.checkAviationWeatherService();
                    this.selectManagermenu(id);
                    break;
                case "0":
                    Console.WriteLine("감사합니다. 안녕히가십시오(__)");

                    //업무 관련 정보 저장..@@
                    this.saveAirplaneSchedule();
                    this.saveManagerlist();
                    //this.saveCatainInfo();
                    return;
                default:
                    Console.WriteLine("잘못 선택하셨습니다.");
                    this.selectManagermenu(id); //재귀
                    break;

            }
        }


        private void selectMembermenu(string id) //회원 업무 창         //관리자업무창과 로직이 다름... 어떤게 좋을지?@@@
        {
            Member member = this.members[id];
            //회원 메뉴판 출력..
            Console.WriteLine($"원하시는 업무를 선택해 주세요.");
            Console.WriteLine($"1. 예매   2. 예매확인 3. 예매변경\n");
            Console.WriteLine($"4. 예매 취소    5. 운행일정 확인  0. 프로그램 종료");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    member.addReservation();
                    this.selectMembermenu(id);
                    break;
                case "2":
                    member.checkReservation(null);
                    this.selectMembermenu(id);
                    break;
                case "3":
                    member.editReservation(null);
                    this.selectMembermenu(id);
                    break;
                case "4":
                    member.cancelReservation(null);
                    this.selectMembermenu(id);
                    break;
                case "5":
                    this.checkSchedule();
                    this.selectMembermenu(id);
                    break;
                case "0":
                    Console.WriteLine("감사합니다. 안녕히가십시오(__)");
                    //업무 관련 정보 저장
                    this.saveAirplaneSchedule();
                    this.saveReservationInfo();
                    this.saveMemberlist();
                    //this.saveReservationInfoDetail();
                    break;
                default:
                    Console.WriteLine("잘못 선택하셨습니다.");
                    break;
            }
        }


        private void selectNonmembermenu()
        {
            NonMember nonMember = new NonMember(this.reservationInfos, this.airplaneSchedules);
            Console.WriteLine($"원하시는 업무를 선택해 주세요.");
            Console.WriteLine($"1. 예매 \t2. 예매확인\t3. 예매변경\n");
            Console.WriteLine($"4. 예매 취소\t5. 운행일정 확인    0. 프로그램 종료");
            string input = Console.ReadLine();
            string reservationNo;
            switch (input)
            {
                case "1":
                    nonMember.addReservation();
                    this.selectNonmembermenu();
                    break;
                case "2":
                    reservationNo = checkReservationNo();
                    nonMember.checkReservation(reservationNo);
                    this.selectNonmembermenu();
                    break;
                case "3":
                    reservationNo = checkReservationNo();
                    nonMember.editReservation(reservationNo);
                    this.selectNonmembermenu();
                    break;
                case "4":
                    reservationNo = checkReservationNo();
                    nonMember.cancelReservation(reservationNo);
                    this.selectNonmembermenu();
                    break;
                case "5":
                    this.checkSchedule();
                    this.selectNonmembermenu();
                    break;
                case "0":
                    Console.WriteLine("감사합니다. 안녕히가십시오(__)");
                    this.saveAirplaneSchedule();
                    this.saveReservationInfo();
                    this.saveReservationInfoDetail();
                    break;
                default:
                    Console.WriteLine("[알림] 잘못 선택하셨습니다.");
                    this.selectNonmembermenu();
                    break;
            }
            
        }


        private string checkReservationNo()
        {
            Console.Write("예매 번호를 입력해 주세요 : ");
            return Console.ReadLine();
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


        private void signUp()
        {
            //회원 생성자.. 전화번호, 이메일, 아이디에 정규식

            Member member = this.checkID();
            members.Add(member.ID, member);

            this.saveMemberlist(); //유저리스트 최신화

            Console.WriteLine("가입을 축하드립니다 ^^");
            Console.WriteLine("가입 기념으로 포인트 5000을 적립해 드렸습니다. 현재 등급은 '{0}'입니다.", member.MemberRating);
        }


        private Member checkID() //아이디 재확인
        {
            string id;
            Regex idRegex = new Regex(@"^[a-zA-Z0-9-_]{5,20}$");

            while (true)
            {
                Console.WriteLine("ID를 입력해 주세요^^");
                id = Console.ReadLine();
                if (!(members.ContainsKey(id)))
                {
                    if (idRegex.IsMatch(id))
                    {
                        Console.WriteLine("아이디 재확인\n한 번 더 입력해 주세요^^");

                        string checkID_str = Console.ReadLine();
                        if (id == checkID_str)
                        {
                            return this.checkPassword(id);
                        }
                        else
                        {
                            Console.WriteLine("아이디가 일치하지 않습니다!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("5~20자의 영문 소문자, 숫자와 특수기호 (_)(-)만 사용가능합니다.");
                    }
                }
                else
                {
                    Console.WriteLine("이미 사용중인 아이디입니다.");
                }
            }
        }


        private Member checkPassword(string id) //비밀번호 재확인
        {
            string password;
            Regex pwRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z0-9$@$!%*#?&]{8,16}$");

            while (true)
            {
                Console.WriteLine("비밀번호를 입력해 주세요.");
                password = Console.ReadLine();

                if (pwRegex.IsMatch(password)) //패스워드 일치 시
                {
                    Console.WriteLine("비밀번호 재확인\n한 번 더 입력해 주세요^^");

                    string checkPassword_str = Console.ReadLine();
                    if (password == checkPassword_str)
                    {
                        //this.addMemberInfo(id, password);
                        return this.addMemberInfo(id, password);
                    }
                    else
                    {
                        Console.WriteLine("비밀번호가 일치하지 않습니다!");
                    }
                }
                else
                {
                    Console.WriteLine("8~16자 영문 대 소문자, 숫자, 특수문자를 사용하세요.");
                }
            }
        }


        public Member addMemberInfo(string id, string password) //회원가입 id, password 입력 후 -> 정보입력
        {
            string passportNo;
            string name;
            string phoneNo;
            string email;
            int point;
            string memberRating;

            Regex passportRegex = new Regex(@"^([M,S,R,G,D][ ]?)[0-9]{8}$"); //여권번호 정규식
            Regex phoneRegex = new Regex(@"(^01[167]\d{7,8}$)|(^010\d{8}$)"); //핸드폰 정규식
            Regex emailRegex = new Regex(@"^[a-zA-z]([._]?[a-zA-Z0-9]){1,15}@([a-zA-Z0-9]){1,15}\.(com|kr|net|jp|en|ru)$"); //이메일 정규식


            while (true)
            {
                Console.WriteLine("여권 번호를 알려주세요.");
                Console.WriteLine("ex)M 12345678");
                passportNo = Console.ReadLine();

                if (passportRegex.IsMatch(passportNo))
                {
                    Console.WriteLine("이름을 알려주세요.");
                    name = Console.ReadLine();

                    Console.WriteLine("연락처를 알려주세요.\n(-)기호 없이 입력");

                    phoneNo = Console.ReadLine();

                    if (phoneRegex.IsMatch(phoneNo))
                    {
                        Console.WriteLine("이메일을 알려주세요.");
                        email = Console.ReadLine();

                        if (emailRegex.IsMatch(email))
                        {
                            point = 5000;
                            memberRating = "Gold";
                            Member member = new Member(passportNo, id, password, name, phoneNo, email, memberRating, point, reservationInfos, airplaneSchedules);

                            return member;
                        }
                        else
                        {
                            Console.WriteLine("올바른 이메일을 입력하세요");
                        }
                    }
                    else
                    {
                        Console.WriteLine("올바른 연락처를 입력하세요");
                    }
                }
                else
                {
                    Console.WriteLine("올바른 여권번호를 입력하세요.");
                }
            }

        }


        //회원
        private void saveMemberlist()
        {
            if (Directory.Exists(@"Members"))
            {
                using (Stream memberList = new FileStream(@"Members\member_list.txt", FileMode.OpenOrCreate))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(memberList, this.members); //managerlist파일에 members 딕셔너리 직렬화..
                }
            }
            else
            {
                Directory.CreateDirectory(@"Members");
                this.saveMemberlist();
            }
        }

        
        private void loadMemberlist()
        {
            //첫 생성시 if문으로 예외 거르기
            if (File.Exists(@"Members\member_list.txt"))
            {
                using (Stream memberList = new FileStream(@"Members\member_list.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    this.members = (Dictionary<string, Member>)bf.Deserialize(memberList);
                }
            }
            else
            {
                this.members = new Dictionary<string, Member>();
            }
        }

        //매니저
        private void saveManagerlist()
        {
            if (Directory.Exists(@"Managers"))
            {
                using (Stream managerList = new FileStream(@"Managers\manager_list.txt", FileMode.OpenOrCreate))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(managerList, this.managers); //managerlist파일에 manager 딕셔너리 직렬화..
                }
            }
            else
            {
                Directory.CreateDirectory(@"Managers");
                this.saveManagerlist();
            }

        }

        
        private void loadManagerlist()
        {
            if (File.Exists(@"Managers\manager_list.txt"))
            {
                using (Stream managerList = new FileStream(@"Managers\manager_list.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    this.managers = (Dictionary<string, Manager>)bf.Deserialize(managerList);
                }
            }
            else
            {
                this.managers = new Dictionary<string, Manager>();
                this.managers.Add("admin", new Manager("admin", "1234", this.airplaneSchedules, this.reservationInfos));
            }
        }

        //예약정보
        private void saveReservationInfo() 
        {
            if (Directory.Exists(@"Reservations"))
            {
                using (Stream reservationList = new FileStream(@"Reservations\reservation_info.txt", FileMode.OpenOrCreate))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(reservationList, this.reservationInfos);
                }
            }
            else
            {
                Directory.CreateDirectory(@"Reservations");
                this.saveReservationInfo();
            }
        }


        private void loadReservationInfo()
        {
            if (File.Exists(@"Reservations\reservation_info.txt"))
            {
                using (Stream reservationList = new FileStream(@"Reservations\reservation_info.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    this.reservationInfos = (Dictionary<string, ReservationInfo>)bf.Deserialize(reservationList);
                }
            }
            else
            {
                this.reservationInfos = new Dictionary<string, ReservationInfo>();
            }
        }


        //비회원
        //private
        private void saveReservationInfoDetail()
        {
            if (File.Exists(@"Reservations\reservation_info_detail.txt"))
            {
                using (Stream reservationDeatailList = new FileStream(@"Reservations\reservation_info_detail.txt", FileMode.OpenOrCreate))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(reservationDeatailList, this.reservationInfoDetails);
                }
            }
            else
            {
                if(Directory.Exists(@"Reservations"))
                {
                    this.saveReservationInfo();
                }
                else
                {
                    Directory.CreateDirectory(@"Reservations");
                    this.saveReservationInfo();
                }

            }
        }


        private void loadReservationInfoDetail()
        {
            if (File.Exists(@"Reservations\reservation_info_detail.txt"))
            {
                using (Stream reservationDeatailList = new FileStream(@"Reservations\reservation_info_detail.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    this.reservationInfoDetails = (List<ReservationInfoDetail>)bf.Deserialize(reservationDeatailList);
                }
            }
            else
            {
                this.reservationInfoDetails = new List<ReservationInfoDetail>();
            }
        }
        private void saveAirplaneSchedule()
        {
            if (Directory.Exists(@"AirplaneSchedule"))
            {
                using (Stream ScheduleList = new FileStream(@"AirplaneSchedule\airplane_schedule.txt", FileMode.OpenOrCreate))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ScheduleList, this.airplaneSchedules);
                }
            }
            else
            {
                Directory.CreateDirectory(@"AirplaneSchedule");
                this.saveAirplaneSchedule();
            }
        }


        private void loadAirplaneSchedule()
        {
            if (File.Exists(@"AirplaneSchedule\airplane_schedule.txt"))
            {
                using (Stream ScheduleList = new FileStream(@"AirplaneSchedule\airplane_schedule.txt", FileMode.Open))
                {
                    
                    BinaryFormatter bf = new BinaryFormatter();
                    this.airplaneSchedules = (Dictionary<string, AirplaneSchedule>)bf.Deserialize(ScheduleList);
                }
            }
            else
            {
                this.airplaneSchedules = new Dictionary<string, AirplaneSchedule>();

            }
        }
    }

}