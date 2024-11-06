using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static doan.ThucThi;

namespace doan
{
    public class Calendar
    {
        private int year;
        private int month;
        private int day;
        private List<CalendarDay> days;
        private static List<Calendar> canlendar2024;

        public Calendar(int year, int month)
        {
            this.year = year;
            this.month = month;
            this.days = GenerateCalendarDays(year, month);
            if (canlendar2024 == null)
            {
                canlendar2024 = new List<Calendar> { };
                canlendar2024.Add(this);
            }
            canlendar2024.Add(this);
        }
        public static List<Calendar> getCanlendar2024()
        {
            return canlendar2024;
        }


        public List<CalendarDay> getDays()
        {
            return days;
        }

        // Phương thức để lấy CalendarDay dựa trên month và day
        public static CalendarDay getCalendarDay(int day, int month)
        {
            // Kiểm tra xem danh sách các Calendar có tồn tại hay không
            if (canlendar2024 != null)
            {
                // Duyệt qua từng Calendar trong danh sách
                foreach (Calendar calendar in canlendar2024)
                {
                    // Duyệt qua từng CalendarDay trong Calendar
                    foreach (CalendarDay calendarDay in calendar.days)
                    {
                        // Kiểm tra điều kiện tìm kiếm
                        if (calendarDay.Day == day && calendarDay.Calendar.getMonth() == month)
                        {
                            return calendarDay; // Trả về CalendarDay tìm thấy
                        }
                    }
                }
            }

            // Trường hợp không tìm thấy
            return null;
        }

        public int getYear()
        {
            return year;
        }
        public int getMonth()
        {
            return month;
        }


        private List<CalendarDay> GenerateCalendarDays(int year, int month)
        {
            List<CalendarDay> calendarDays = new List<CalendarDay>();

            // Tạo một DateTime đại diện cho ngày đầu tiên của tháng
            DateTime firstDayOfMonth = new DateTime(year, month, 1);

            // Tạo một DateTime đại diện cho ngày cuối cùng của tháng
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Lặp qua tất cả các ngày từ ngày đầu tiên đến ngày cuối cùng của tháng và thêm vào danh sách
            for (DateTime currentDate = firstDayOfMonth; currentDate <= lastDayOfMonth; currentDate = currentDate.AddDays(1))
            {
                // Tạo một đối tượng CalendarDay mới với giá trị ngày và tham chiếu tới đối tượng Calendar gốc
                CalendarDay calendarDay = new CalendarDay(currentDate.Day, this);
                calendarDays.Add(calendarDay);
            }

            return calendarDays;
        }

    }
    public class CalendarDay
    {
        public int Day { get; }
        public Calendar Calendar { get; set; }
        private List<Channel> listChannels;
        private List<TimeSet> listTimeSets;
        public CalendarDay(int day, Calendar calendar)
        {
            this.Day = day;
            Calendar = calendar;
            this.listChannels = new List<Channel>();
            this.listTimeSets = new List<TimeSet>();
        }

        public void setListChannels(List<Channel> input)
        {
            listChannels = input;
        }
        public List<Channel> getListChannels()
        {
            // Trả về bản sao của danh sách để ngăn chặn sự thay đổi từ bên ngoài
            return listChannels;
        }

        // Thêm bản tin vào kênh của CalendarDay

        public void addBanTinToChannel(BanTin banTin, Channel channel, string period, int day, int month)
        {
            channel.channelAddBanTin(banTin.getName(), period);
            listTimeSets.Add(new TimeSet(banTin.getTime(), period, day, month, channel.getName(), banTin.getName()));
        }


        public int getDay()
        {
            return Day;
        }



        public override string ToString()
        {
            return $"{Day}/{Calendar.getMonth()}/{Calendar.getYear()}";
        }
    }
    internal class Category
    {
        private static int i = 1;
        public string name { get; set; }
        private List<New> news { get; set; }
        private static List<Category> categories = new List<Category>();


        public Category(string name)
        {
            this.name = name;
            //kiem tra list BanTin
            if (news == null)
                news = new List<New>();

            if (categories == null)
            {
                categories = new List<Category>();
                categories.Add(this);
            }
            categories.Add(this);
        }

        public string getName()
        {
            return name;
        }

        public List<New> getNews()
        {
            if (news == null)
            {
                news = new List<New>();
            }
            return news;
        }
        public static List<Category> getCategories()
        {
            if (categories == null)
            {
                categories = new List<Category>();
            }
            return categories;
        }



        public void printAllBanTin()
        {
            Console.WriteLine("The loai " + name + " gom cac ban tin :");
            if (news == null || news.Count == 0)
                Console.WriteLine("khong co du lieu");
            else
                foreach (New banTin in news)
                {
                    Console.WriteLine(banTin.getName());
                }
        }



        public void setCategory(string banTinName)
        {
            bool check = false;
            BanTin foundBanTin = null;
            foreach (New banTin in BanTin.listBanTins)
            {
                if (banTin.getName() == banTinName)
                {
                    check = true;
                    foundBanTin = (BanTin)banTin;
                    break;
                }
            }

            if (check && foundBanTin != null || foundBanTin is BanTin)
            {
                news.Add(foundBanTin);
                foundBanTin.setCategoryName(this.name);
            }
        }

        public void removeCategory(string banTinName)
        {
            foreach (New banTin in BanTin.listBanTins)
            {
                if (banTin.getName() == banTinName)
                {
                    news.Remove(banTin);
                    break;
                }
            }
        }


        static public void printAllCategory()
        {
            foreach (Category category in categories)
            {
                Console.WriteLine("The loai " + i + " : " + category.name);
                i++;
            }
        }



    }


    public class TimeSet
    {

        private double time;
        private string period;
        private string nameChannel;
        private string nameBanTin;
        private int day;
        private int month;
        private DateTime timeStart;
        private string calendarShow;
        public static List<TimeSet> listAllTimeSet = new List<TimeSet>();
        private TimeSpan timeEnd { get; set; }
        public string Period { get { return period; } set { period = value; } }
        public string NameChannel { get {return nameChannel; }
        set { nameChannel = value; }
    }
        public string NameBanTin { get { return nameBanTin; } set { nameBanTin = value; } }
        public int Day { get { return day; } set { day = value; } }

        public TimeSet(double time, string period, int day, int month, string nameChannel, string nameBanTin)
        {
            this.day = day;
            this.month = month;
            this.time = time;
            this.period = period;
            this.nameChannel = nameChannel;
            this.nameBanTin = nameBanTin;
            listAllTimeSet.Add(this);
            calendarShow = day + "/" + month;
        }
        public void setTime(double input) { this.time = input; }
        public double getTime() { return time; }
        public void setTimeStart(DateTime input)
        {
            timeStart = input;
        }
        public int getDay() { return day; }
        public int getMonth() { return month; }
        public void setChannelOfTimeSet(string input)
        {
            nameChannel = input;
        }
        public string getChannelOfTimeSet()
        {
            return nameChannel;
        }
        public void setBanTinOfTimeSet(string input)
        {
            nameBanTin = input;
        }
        public void setDayOfTimeSet(string input)
        {
            calendarShow = input;
        }
        public DateTime getTimeStart()
        {
            return timeStart;
        }

        public override string ToString()
        {
            return "Thoi luong: " + time + "s" + "\n" +
                   "Khung chieu: " + period + "\n" +
                   "Thời gian bat dau: " + timeStart + " tại kênh " + nameChannel +
                   " Ngày: " + day + "/" + month;
        }

        public static void deleteTimeSet()
        {

            // Nhập tên kênh
            Console.WriteLine("Nhập tên kênh:");
            string nameofChannel = Console.ReadLine();

            // Nhập tên bản tin
            Console.WriteLine("Nhập tên bản tin:");
            string nameofnew = Console.ReadLine();

            // Nhập thời gian
            Console.WriteLine("Nhập khoảng thời gian (sang/toi):");
            string period = Console.ReadLine();

            // Nhập ngày và tháng
            Console.WriteLine("Nhập ngày:");
            int day;
            while (!int.TryParse(Console.ReadLine(), out day) || day < 1 || day > 31)
            {
                Console.WriteLine("Ngày không hợp lệ! Nhập lại:");
            }

            Console.WriteLine("Nhập tháng:");
            int month;
            while (!int.TryParse(Console.ReadLine(), out month) || month < 1 || month > 12)
            {
                Console.WriteLine("Tháng không hợp lệ! Nhập lại:");
            }

            foreach (TimeSet index in listAllTimeSet)
            {
                if (nameofChannel == index.nameChannel && nameofnew == index.nameBanTin && period == index.period && day == index.day && month == index.month)
                    listAllTimeSet.Remove(index);
                Console.WriteLine("Xoá thành công");
                break;
            }

        }

        

    }
    class Program
    {
        private static readonly string filePath = "C:\\doan\\data.json";
        static BanTinList banTinList1 = new BanTinList();
        static BanTinList banTinList2 = new BanTinList();
       
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                BanTinList banTinList = BanTinList.DeserializeFromJson(jsonString);

                // Lưu trữ vào listBanTins nhưng chỉ thêm các bản tin chưa tồn tại
                foreach (BanTin banTin in banTinList.Bantins)
                {
                    if (!BanTin.listBanTins.Contains(banTin))
                    {
                        BanTin.listBanTins.Add(banTin);
                    }
                }

                Console.WriteLine("Dữ liệu đã được đọc từ file JSON và lưu trữ vào listBanTins.");
            }
            else
            {
                Console.WriteLine("File JSON không tồn tại.");
            }



            /*     // Đọc nội dung JSON từ file
              if (File.Exists(filePath))
                 {
                     string jsonString = File.ReadAllText(filePath);
                     BanTinList banTinList = BanTinList.DeserializeFromJson(jsonString);

                     // Lưu trữ vào listBanTins
                     foreach (BanTin banTin in banTinList.Bantins)
                     {
                         BanTin.listBanTins.Add(banTin);
                     }

                     Console.WriteLine("Dữ liệu đã được đọc từ file JSON và lưu trữ vào listBanTins.");
                 }
                 else
                 {
                     Console.WriteLine("File JSON không tồn tại.");
                 }
            */

            // Đảm bảo rằng có dữ liệu trong listBanTins
            if (BanTin.listBanTins.Count > 0)
            {
                // Duyệt qua từng BanTin trong danh sách listBanTins
                foreach (BanTin banTin in BanTin.listBanTins)
                {
                    Console.WriteLine("Tên bản tin: " + banTin.Name);
                    Console.WriteLine("Nội dung: " + banTin.NoiDung);
                    Console.WriteLine("Thời gian: " + banTin.Time);
                    Console.WriteLine("Thể loại: " + banTin.CategoryName);
                    Console.WriteLine(); // Dòng trống để ngăn cách các bản tin
                }
            }
            else
            {
                Console.WriteLine("Danh sách listBanTins hiện đang trống.");
            }
            Console.WriteLine(  "helo");
            
            ThucThi.createCalendar();
            KhoiTao();
            Menu();

        }

        public static void KhoiTao()
        {
            // Tao kenh VTV1 o tat ca cac ngay
            ThucThi.addChannel("VTV1", 5);
            ThucThi.addChannel("VTV2", 5);
            ThucThi.addChannel("VTV3", 5);

            // Tạo các bản tin
           
            BanTin banTin1 = new BanTin("BanTin1", 180, "Nội dung bản tin 1");
            BanTin banTin2 = new BanTin("BanTin2", 150, "Nội dung bản tin 2");
            BanTin banTin3 = new BanTin("BanTin3", 120, "Nội dung bản tin 3");
            BanTin banTin4 = new BanTin("BanTin4", 90, "Nội dung bản tin 4");
            BanTin banTin5 = new BanTin("BanTin5", 90, "Nội dung bản tin 4");
                
            // tạo thể loại + 
            Category thethao = new Category("The Thao");
            Category giaitri = new Category("Giai Tri");
            Category thoisu = new Category("Thoi Su");

            thethao.setCategory("BanTin1");
            thethao.setCategory("BanTin2");
            giaitri.setCategory("BanTin3");
            giaitri.setCategory("BanTin4");
            thoisu.setCategory("BanTin5");


            //Đặt thời gian cho các bản tin
            Console.WriteLine("--------------");
            Console.WriteLine("--------------");
            ThucThi.setTimeAndBanTinForChannel("VTV1", "BanTin1", "sang", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV1", "BanTin2", "sang", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV1", "BanTin3", "sang", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV1", "QuangCao1", "sang", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV2", "Live", "sang", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV2", "BanTin2", "sang", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV2", "BanTin3", "sang", 3, 3);

            ThucThi.setTimeAndBanTinForChannel("VTV1", "BanTin1", "toi", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV1", "BanTin2", "toi", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV1", "BanTin3", "toi", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV1", "QuangCao1", "toi", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV2", "Live", "toi", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV2", "BanTin2", "toi", 3, 3);
            ThucThi.setTimeAndBanTinForChannel("VTV2", "BanTin3", "toi", 3, 3);
            
        }

      

        public static void Menu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("---- Quan ly ban tin truyen hinh ----");
                Console.WriteLine("---- 1. Quản lý bản tin ");
                Console.WriteLine("---- 2. Quản lý nhân sự ");
                Console.WriteLine("---- 3. Quản lý kênh ");
                Console.WriteLine("---- 4. Quản lý thể loại ");
                Console.WriteLine("---- 5. Quản lý thời gian trình chiếu ");
                Console.WriteLine("---- 6. Đọc dữ liệu từ file JSON ");
                Console.WriteLine("---- 7. Lưu dữ liệu vừa thao tác vào file JSON ");
                Console.WriteLine("---- 8. Thoát ");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("--------------------------------------");

                Console.WriteLine("Nhập lựa chọn của bạn:");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ThucThi.QuanLiBanTin();
                        break;

                    case "3":
                        ThucThi.QuanLiKenh();
                        break;
                    case "4":
                        ThucThi.QuanLiTheLoai();
                        break;
                    case "5":
                        ThucThi.QuanLiSetTime();
                        break;
                    case "6":
                        BanTinList deserializeBanTin = ThucThi.DeserializeJsonToObject<BanTinList>(filePath);
                        foreach (BanTin bantin in deserializeBanTin.Bantins)
                        {
                            Console.WriteLine(bantin.ToString());
                        }
                        break;
                    case "7":
                        // Lựa chọn banTinList để ghi vào file
                        BanTinList chooseBanTinToJson = banTinList1 == ThucThi.banTinListNew ? banTinList1 : ThucThi.banTinListNew;

                        // Gọi phương thức để serializila và ghi vào file
                        ThucThi.SerializeObjectToJsonFile(chooseBanTinToJson, "banTinData.json");

                        // Đảm bảo dữ liệu đã được thêm
                        Console.WriteLine("Số lượng bản tin trong chooseBanTinToJson: " + chooseBanTinToJson.Bantins.Count);

                        break;
                    case "8":
                        exit = true; // Đặt biến exit thành true để thoát khỏi vòng lặp
                        Console.WriteLine("Thoát chương trình");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ");
                        break;
                }

                Console.WriteLine();
            }
        }

    }
}
