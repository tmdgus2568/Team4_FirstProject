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
using System.Text.RegularExpressions;
// 패키지는 각자에 맞는


namespace TeamProject_Airplane
{
    [Serializable]
    class Manager
    {
        public string ID { get; }
        public string Password { get; }

        private List<string> dateHourly; //시간별 48시간 list
        private List<string> weatherHourly; // 시간별 날씨
        private List<string> visibilityHourly; // 시간별 가시성 wind_deg
        private List<string> windDegHourly; // 시간별 돌풍
        private List<string> dateDaily; //일자별 7일 list
        private List<string> weatherDaily; // 일자별 날씨
        private List<string> uviDaily; // 일자별 자외선 지수
        private List<string> windDegDaily; // 일자별 돌풍
        [NonSerialized]
        public Dictionary<string, AirplaneSchedule> airplaneSchedules; //key: 비행기번호
        [NonSerialized]
        public Dictionary<string, ReservationInfo> reservationInfos;

        public Manager(string id, string password, Dictionary<string, AirplaneSchedule> airplaneSchedules, Dictionary<string, ReservationInfo> reservationInfos)
        {
            this.ID = id;
            this.Password = password;
            this.airplaneSchedules = airplaneSchedules; //, Dictionary<string, AirplaneSchedule> airplaneSchedules
            this.reservationInfos = reservationInfos;
            this.dateHourly = new List<string>(); //시간별 48시간 list
            this.weatherHourly = new List<string>(); // 시간별 날씨
            this.visibilityHourly = new List<string>(); // 시간별 가시성 wind_deg
            this.windDegHourly = new List<string>(); // 시간별 돌풍
            this.dateDaily = new List<string>(); //일자별 7일 list
            this.weatherDaily = new List<string>(); // 일자별 날씨
            this.uviDaily = new List<string>(); // 일자별 가시성 wind_deg
            this.windDegDaily = new List<string>(); // 일자별 돌풍

        }
        private void alert(string str)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + str);
            Console.ForegroundColor = ConsoleColor.White;
        }
        private void adminAlert(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n" + str);
            Console.ForegroundColor = ConsoleColor.White;
        }


        public void checkSchedule() // 비행기 일정표 확인  
        {   
            weatherInfo();
            adminAlert("##########################################################################################################현재 비행기 일정##########################################################################################################\n");
            Regex regDay = new Regex(@"(\d\d)-(\d\d)-(\d\d)");
            Regex regHour = new Regex(@"(\d\d)$");
            MatchCollection resultDay;
            MatchCollection resultHour;
            MatchCollection resultWeatherDay;
            MatchCollection resultWeatherHour;
            string matchEtaDay = "";
            string matchEtaHour = "";
            string matchEtaWeatherDay = "";
            string matchEtaWeatherHour = "";
            int weatherHourCount = 0;
            int weatherDayCount = 0;
            if (this.airplaneSchedules.Count > 0) //로딩이 있거나 데이터 파일이 있다는거 그럼 검증하고 출력
            {
                for (int i = 0; i < 7; i++) // 현재 시간으로부터 7일 더하고 list에 add
                {
                    Console.WriteLine($"#################################################################     [날짜 : {DateTime.Now.AddDays(i).ToString("yy/MM/dd")}]     (날씨 : {weatherDaily[i]}    돌풍 지수 : {windDegDaily[i]}    자외선 지수 : {uviDaily[i]})#################################################################\n\n\n");
                    for (int j = 0; j < 24; j++) // 시간 출력
                    {
                        try
                        {
                            resultWeatherDay = regDay.Matches(dateHourly[weatherDayCount]);
                            resultWeatherHour = regHour.Matches(dateHourly[weatherHourCount]);
                            foreach (var itemDay in resultWeatherDay) { matchEtaWeatherDay = itemDay.ToString(); }
                            foreach (var itemHour in resultWeatherHour) { matchEtaWeatherHour = itemHour.ToString(); }

                            if (string.Format("{0:D2}", j) == matchEtaWeatherHour && weatherHourCount < 47)
                            {
                                weatherHourCount++;
                                Console.Write($"{string.Format("{0:D2}", j)}시 (날씨 :{weatherHourly[j]} 돌풍 지수: {windDegHourly[j]} 가시성: {visibilityHourly[j]}): ");
                                if (int.Parse(matchEtaWeatherHour) == 23)
                                {
                                    weatherDayCount++;
                                }
                            }
                            else
                            {
                                Console.Write($"{string.Format("{0:D2}", j)}시:     ");
                            }
                        }
                        catch (Exception e) { }

                        foreach (var item in this.airplaneSchedules)
                        {
                            resultDay = regDay.Matches(item.Value.TakeOffTime);
                            resultHour = regHour.Matches(item.Value.TakeOffTime);
                            foreach (var itemDay in resultDay) { matchEtaDay = itemDay.ToString(); }
                            foreach (var itemHour in resultHour) { matchEtaHour = itemHour.ToString(); }

                            if (DateTime.Now.AddDays(i).ToString("yy/MM/dd") == matchEtaDay && string.Format("{0:D2}", j) == matchEtaHour)
                            {
                                Console.Write($"[++++비행기 번호 : {item.Value.AirplanceNo}  출발예정시간 : {item.Value.TakeOffTime}  도착예정시간:{item.Value.Eta}  출발예정시간 : {item.Value.TakeOffTime}  목적지 : {item.Value.DestinationPoint}  기준가격 : {item.Value.PriceInfo}  기장 : {item.Value.CaptainInfo.Name}  부기장 : {item.Value.CoCaptainInfo.Name}++++]\n");
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
                    Console.WriteLine($"###########################[{DateTime.Now.AddDays(i).ToString("yy/MM/dd")}]     날씨:{weatherDaily[i]}    돌풍 지수:{windDegDaily[i]}    자외선 지수:{uviDaily[i]}###########################\n\n\n");
                    for (int j = 0; j <= 24; j++) // 시간 출력
                    {
                        Console.WriteLine($"{string.Format("{0:D2}", j)}시:     \n\n");
                    }
                }
            }

        }


        public void addSchedule() // 비행기 일정 추가  [완성]
        {
            AirplaneSchedule airplaneSchedule = new AirplaneSchedule();
            adminAlert("########################################################################################################[비행기 일정 추가]########################################################################################################");
            Console.Write("비행기 번호를 입력해 주세요. : ");
            string airplanceNo = Console.ReadLine();

            if (!this.airplaneSchedules.ContainsKey(airplanceNo))
            {
                Console.Write("이륙 시간을 입력해 주세요.( ex 22-03-19.15 ) : ");
                string takeOffTime = Console.ReadLine();
                Console.Write("도착 예정 시간을 입력해 주세요.( ex 22-03-19.15 ) : ");
                string eta = Console.ReadLine();
                Console.Write("목적지를 입력해 주세요. : ");
                string destinationPoint = Console.ReadLine();
                Console.Write("기장 이름을 입력해주세요. : ");
                string captainName = Console.ReadLine();
                Console.Write("기장 휴대폰 번호를 입력해주세요.( - 기호 없이 입력 ) : ");
                string captainPhoneNo = Console.ReadLine();
                Console.Write("기장 메일을 입력해주세요.:");
                string captainEmail = Console.ReadLine();
                Console.Write("부기장 이름을 입력해주세요.:");
                string cCaptainName = Console.ReadLine();
                Console.Write("부기장 휴대폰 번호를 입력해주세요.( - 기호 없이 입력 ) : ");
                string cCaptainPhoneNo = Console.ReadLine();
                Console.Write("부기장 메일을 입력해주세요. : ");
                string cCaptainEmail = Console.ReadLine();
                Console.Write("기반이 되는 가격을 입력해주세요. : "); // 이유 기반이 되는 가격을 정해야 퍼스트 비즈니스 등의 가격이 측정된다
                int priceInfo = int.Parse(Console.ReadLine());
                this.airplaneSchedules.Add(airplanceNo, new AirplaneSchedule(airplanceNo, takeOffTime, eta, destinationPoint,
                                      new Captain(true, captainName, captainPhoneNo, captainEmail),
                                      new Captain(false, cCaptainName, cCaptainPhoneNo, cCaptainEmail), priceInfo));
            }
            else
            {
                alert("[알림] 이미 배치되어 있는 비행기입니다.");
            }
        }


        public void editSchedule() // 비행기 일정표 변경  [완성] //음.. 변경이 어떤 ... 기장과 부기장 정도밖에 없는데..
        {
            //일정표 한번 출력
            adminAlert("########################################################################################################[비행기 일정 변경]########################################################################################################");
            Console.Write("변경하실 비행기 번호를 입력해주세요.: ");
            string editTargetNo = Console.ReadLine();
            string captainTarget;
            string captainName = "";
            string captainPhoneNo = "";
            string captainEmail = "";
            string cCaptainName = "";
            string cCaptainPhoneNo = "";
            string cCaptainEmail = "";
            bool whileJug = true;
            bool targetIsCaptain = false;
            bool targetIsCcaptain = false;
            if (this.airplaneSchedules.ContainsKey(editTargetNo))
            {
                while (whileJug)
                {
                    Console.WriteLine("변경하고 싶은 메뉴를 선택해주세요.");
                    Console.Write("1. 기장 2. 부기장 3. 종료 : ");
                    captainTarget = Console.ReadLine();
                    switch (captainTarget)
                    {
                        case "1":
                            Console.Write("기장 이름을 입력해주세요. : ");
                            captainName = Console.ReadLine();
                            Console.Write("기장 휴대폰 번호를 입력해주세요.( - 기호 없이 입력 ) : ");
                            captainPhoneNo = Console.ReadLine();
                            Console.Write("기장 메일을 입력해주세요. : ");
                            captainEmail = Console.ReadLine();
                            targetIsCaptain = true;
                            break;
                        case "2":
                            Console.Write("부기장 이름을 입력해주세요. : ");
                            cCaptainName = Console.ReadLine();
                            Console.Write("부기장 휴대폰 번호를 입력해주세요.( - 기호 없이 입력 ) : ");
                            cCaptainPhoneNo = Console.ReadLine();
                            Console.Write("부기장 메일을 입력해주세요. : ");
                            cCaptainEmail = Console.ReadLine();
                            targetIsCcaptain = true;
                            break;
                        case "3":
                            whileJug = false;
                            break;
                        default:
                            alert("[알림] 잘못된 입력입니다.");
                            break;
                    }

                    if (!whileJug) //3번 입력의 경우 탈출
                    {
                        break;
                    }

                    foreach (var item in this.airplaneSchedules)
                    {
                        if (item.Key == editTargetNo)
                        {
                            if (targetIsCaptain) // 기장의 경우
                            {
                                item.Value.CaptainInfo.Name = captainName;
                                item.Value.CaptainInfo.PhoneNo = captainPhoneNo;
                                item.Value.CaptainInfo.Email = captainEmail;
                                break;
                            }
                            else if (targetIsCcaptain)  // 부기장의 경우
                            {
                                item.Value.CoCaptainInfo.Name = cCaptainName;
                                item.Value.CoCaptainInfo.PhoneNo = cCaptainPhoneNo;
                                item.Value.CoCaptainInfo.Email = cCaptainEmail;
                                break;
                            }
                        }
                    }
                    targetIsCaptain = false; // 다시 비활성
                    targetIsCcaptain = false; // 다시 비활성
                }
            }
            else
            {
                alert("[알림] 해당된 비행기 번호는 존재하지 않습니다.");
            }

        }


        public void deleteSchedule() // 비행기 일정 삭제 [완성]
        {
            //일정표 한번 출력
            adminAlert("#########################################################################################################[비행기 일정 삭제]#########################################################################################################");
            Console.Write("삭제하실 비행기 번호를 입력해주세요. : ");
            string deleteTarget = Console.ReadLine(); //airplaneNo

            if (this.airplaneSchedules.ContainsKey(deleteTarget))
            {
                alert("[알림] 해당 비행기 일정이 삭제되었습니다.");

                foreach (var item in this.reservationInfos.Values.ToList())
                {
                    if (item.airplaneSchedule.AirplanceNo == deleteTarget)
                    {
                        reservationInfos.Remove(item.reservationNo);
                    }
                }
                this.airplaneSchedules.Remove(deleteTarget);
            }
            else
            {
                alert("[알림] 해당된 비행기 번호는 존재하지 않습니다.");
            }
        }


        public void checkAviationWeatherService() // 항공기상정보 제공 [완성]
        {
            string icao = "RKSI";
            string dataType = "JSON";
            string numOfRows = "10";
            string pageNo = "4";
            string aviationWeatherServiceUrlMetar = "http://apis.data.go.kr/1360000/AmmService/getMetar?ServiceKey=" + secretKey + "&pageNo=" + pageNo + "&numOfRows=" + numOfRows + "&dataType=" + dataType + "&icao=" + icao;
            string aviationWeatherServiceUrlTaf = "http://apis.data.go.kr/1360000/AmmService/getTaf?ServiceKey=" + secretKey + "&pageNo=" + pageNo + "&numOfRows=" + numOfRows + "&dataType=" + dataType + "&icao=" + icao;
            string aviationWeatherServiceUrlWarning = "http://apis.data.go.kr/1360000/AmmService/getWarning?ServiceKey=" + secretKey + "&pageNo=" + pageNo + "&numOfRows=" + numOfRows + "&dataType=" + dataType;
            //string aviationWeatherServiceUrlSigmet = "http://apis.data.go.kr/1360000/AmmService/getSigmet?ServiceKey=" + secretKey + "&pageNo=" + pageNo + "&numOfRows=" + numOfRows + "&dataType=" + dataType; //응답코드 03 no data
            //string aviationWeatherServiceUrlAirmet = "http://apis.data.go.kr/1360000/AmmService/getAirmet?ServiceKey=" + secretKey + "&pageNo=" + pageNo + "&numOfRows=" + numOfRows + "&dataType=" + dataType; //응답코드 03 no data
            HttpWebRequest request;
            HttpWebResponse response;
            string tafMessage = "No data";
            string metarMessage = "No data";
            string warningMessage = "No data";
            //string sigmetMessage = "No data";
            //string airmetMessage = "No data";
            Dictionary<int, List<string>> warningMessages = new Dictionary<int, List<string>>();
            int warningCount = 0;
            Stream stream;
            StreamReader reader;
            string aviationWeatherText;
            List<JObject> aviationWeatherTextJsons = new List<JObject>();
            List<string> urls = new List<string>() { aviationWeatherServiceUrlMetar, aviationWeatherServiceUrlTaf, aviationWeatherServiceUrlWarning };//, aviationWeatherServiceUrlSigmet, aviationWeatherServiceUrlAirmet };

            try
            {
                foreach (var item in urls)
                {
                    request = (HttpWebRequest)HttpWebRequest.Create(item);// http 통신 모듈
                    response = (HttpWebResponse)request.GetResponse(); // 응답보냄
                    stream = response.GetResponseStream();//스트림으로 내용 받음
                    reader = new StreamReader(stream, Encoding.UTF8); // 스트림 리더로 인코딩 설정 후
                    aviationWeatherText = reader.ReadToEnd(); // 받은 응답 내용을 스트림으로 읽음
                    aviationWeatherTextJsons.Add(JObject.Parse(aviationWeatherText)); // json으로 변환
                    reader.Close();
                }

                metarMessage = aviationWeatherTextJsons[0]["response"]["body"]["items"]["item"][0]["metarMsg"].ToString(); // 단 한개만 출력
                tafMessage = aviationWeatherTextJsons[1]["response"]["body"]["items"]["item"][0]["metarMsg"].ToString(); // //와 원래 Taf는 tafMsg인데 얘네가 실수로 metar랑 똑같이함..
                warningMessage = aviationWeatherTextJsons[2]["response"]["body"]["items"].ToString();

                foreach (var item in aviationWeatherTextJsons[2]["response"]["body"]["items"]["item"])// waring코드는 여러 개 이상일 수 있어서 딕셔너리 배열로 받음
                {
                    warningCount++;
                    warningMessages.Add(warningCount, new List<string>() { item["airportName"].ToString() ,
                                        DateTime.ParseExact(item["tm"].ToString(), "yyyyMMddHHmm", null).ToString("yy/MM/dd.HH:mm"),
                                        DateTime.ParseExact(item["validTm1"].ToString(), "yyyyMMddHHmm", null).ToString("yy/MM/dd.HH:mm"),
                                        DateTime.ParseExact(item["validTm2"].ToString(), "yyyyMMddHHmm", null).ToString("yy/MM/dd.HH:mm"),
                                        item["wrngMsg"].ToString() });
                }

            }
            catch (Exception e)
            {
                //Console.WriteLine(e); //에러 출력
            }

            //display
            adminAlert("######################################################################################################AviationWeatherService######################################################################################################\n\n");
            Console.WriteLine($"#현재 인천공항의 METAR/SPECI CODE: {metarMessage}\n");
            Console.WriteLine($"#현재 인천공항의 TAF CODE: {tafMessage}\n");
            Console.WriteLine($"#현재 전국의 WARNING CODE:\n");

            if (warningMessages.Count > 1)
            {
                foreach (var item in warningMessages) // warning 딕셔너리 출력 테스트
                {
                    Console.WriteLine($"공항 경보 코드: {item.Value[4]}");
                    Console.WriteLine($"CODE {item.Key}:  {item.Value[0]} 발표 시각: {item.Value[1]} 발효 시간: {item.Value[2]} 발효 종료: {item.Value[3]}\n");

                }
            }
            else
            {
                Console.WriteLine("현재 경보 없음\n\n");
            }
            adminAlert("####################################################################################################################################################################################################################################\n\n");

        }


        public void weatherInfo() // 48시간 시간별 일기예보, 일일 예보 [완성]
        {
            string lat = "37.4692"; // 위도
            string lon = "126.451"; // 경도
         
            string exclude = "minutely"; // 받지 않을 조건
            string unit = "metric"; // metric은 섭씨, 풍속(미터/초) imperial은 화씨, 마일/시간
            string openWeatherUrl = "https://api.openweathermap.org/data/2.5/onecall?lat=" + lat + "&lon=" + lon + "&exclude=" + exclude + "&units=" + unit + "&appid=" + secretKey; // openweather one call api 

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); //현재 시간대로 변환
                                                                                                 //ex) 22-03-1922
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(openWeatherUrl);// http 통신 모듈
                HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // 응답보냄

                using (Stream stream = response.GetResponseStream())//스트림으로 내용 받음
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8); // 스트림 리더로 인코딩 설정 후
                    string weatherText = reader.ReadToEnd(); // 받은 응답 내용을 스트림으로 읽음
                    JObject weatherTextJson = JObject.Parse(weatherText); // json으로 변환

                    foreach (var item in weatherTextJson["hourly"]) // item type = JToken
                    {
                        this.dateHourly.Add(startTime.AddSeconds((double)item["dt"]).ToString("yy/MM/dd.HH"));//시간
                        this.weatherHourly.Add(item["weather"][0]["description"].ToString());
                        this.visibilityHourly.Add(item["visibility"].ToString());
                        this.windDegHourly.Add(item["wind_gust"].ToString());
                    }

                    foreach (var item in weatherTextJson["daily"]) // item type = JToken
                    {
                        this.dateDaily.Add(startTime.AddSeconds((double)item["dt"]).ToString("yy/MM/dd"));//시간
                        this.weatherDaily.Add(item["weather"][0]["description"].ToString());
                        this.uviDaily.Add(item["uvi"].ToString()); // ToString(), as string으로 변환시 개체 참조 오류
                        this.windDegDaily.Add(item["wind_gust"].ToString());
                    }
                    response.Close();
                }
            }
            catch (Exception e)
            {
                alert("[알림] 다음에 시도해주세요.");
            }
        }
    }
}