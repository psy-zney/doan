using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace doan
{
    public interface New
    {

        double getTime();
        void setTime(string period, string nameChannel, int inputDay, int inputMonth);
        void print();
        List<string> getListDays();
        List<string> getListChannels();
        List<TimeSet> getListTime();
        string getName();
        void setName(string name);
        void setChanelName(string input);
        void setNoiDung(string input);
        void setTime(double time);
        string getNoiDung();
    }
    [Serializable]
    public class BanTin : New
    {
        public static List<New> listBanTins = new List<New>();
        private string name;
        private string noiDung;
        private double time;
        private string categoryName;

        public string Name { get { return name; } set { name = value; } }
        public string NoiDung { get { return noiDung; } set { noiDung = value; } }
        public double Time { get {return time; } set {time = value; }
    }
        public string CategoryName { get { return categoryName; } set { categoryName = value; } }
        

        private List<string> listChannels;
        private List<string> listDays;
        private List<TimeSet> listTime;
      

        [JsonConstructor] 
        public BanTin(string name, double time, string noiDung)
        {
            CategoryName = " Chưa có dữ liệu thể loại ";
            
            this.Name = name;
            this.Time = time;
            this.NoiDung = noiDung;
            listTime = new List<TimeSet>();
            listChannels = new List<string>();
            listDays = new List<string>();
            getListNew().Add(this);
           
            

        }
        // Kiểm tra xem "name" đã tồn tại trong danh sách hay chưa
        private bool IsNameUnique(string name)
        {
            foreach (BanTin bantin in getListNew())
            {
                if (bantin.Name == name)
                {
                    return false; // Tên đã tồn tại trong danh sách
                }
            }
            return true; // Tên chưa tồn tại trong danh sách
        }
        
            public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", Name);
            info.AddValue("noiDung", NoiDung);
            // info.AddValue("time", time);
            info.AddValue("categoryName", CategoryName);
            
        }

        public BanTin(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("name");
            NoiDung = info.GetString("noiDung");
            Time = info.GetDouble("time");
            CategoryName = info.GetString("categoryName");
           
        }

        public void setNoiDung(string input)
        {
            this.NoiDung = input;
        }
        public string getNoiDung() { return NoiDung; }
        public List<TimeSet> getListTime()
        {
            return listTime;
        }

        public void setChanelName(string input)
        {
            listChannels.Add(input);
        }

        public double getTime() { return Time; }
        public void setTime(double input) { this.Time = input; }
        public List<string> getListDays()
        {
            return listDays;
        }
        public List<string> getListChannels()
        {
            return listChannels;
        }
        public static List<New> getListNew()
        {
            return BanTin.listBanTins;
        }

        public void setCategoryName(string input)
        {
            CategoryName = input;
        }

        public string getCategoryName()
        {
            foreach (Category iCategory in Category.getCategories())
            {
                if (iCategory.name == this.Name)
                {
                    CategoryName = iCategory.name;
                }
            }
            return CategoryName;
        }

        

      
        public void setName(string name) { this.Name = name; }
        public string getName()
        {
            return Name;
        }

        


        public void print()
        {
            Console.WriteLine("Tên bản tin: " + Name + "\n" +
                   "Nội dung: " + NoiDung + "\n" + "Thời lượng: " + Time);
            foreach (TimeSet item in listTime)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine("-----");
        }
        public void addBanTinToCalendarDay(CalendarDay calendarDay, Channel channel, string period, int inputDay, int inputMonth)
        {
            setTime(period, channel.getName(), inputDay, inputMonth);
            calendarDay.addBanTinToChannel(this, channel, period, inputDay, inputMonth);
        }
        public override string ToString()
        {
            return "Tên bản tin: " + Name + "\n" +
                   "Nội dung: " + NoiDung + "\n" +
                   "Thời lượng: " + Time + "\n";
        }
        public void setTime(string period, string nameChannel, int inputDay, int inputMonth)
        {
            TimeSpan currentTime;

            if (period == "sang")
            {
                currentTime = new TimeSpan(8, 0, 0);
            }
            else if (period == "toi")
            {
                currentTime = new TimeSpan(18, 0, 0);
            }
            else
            {
                Console.WriteLine("Dữ liệu không đúng");
                return;
            }

            foreach (Calendar iCalendar in Calendar.getCanlendar2024())
            {
                if (iCalendar.getMonth() == inputMonth)
                {
                    foreach (CalendarDay iCalendarDay in iCalendar.getDays())
                    {
                        if (inputDay == iCalendarDay.getDay())
                        {
                            foreach (Channel iChannel in iCalendarDay.getListChannels())
                            {
                                if (iChannel.getName() == nameChannel)
                                {
                                    double xTime = 0;

                                    // Duyệt qua tất cả các phần tử New trong ListPeriod
                                    foreach (New newElement in iChannel.getListPeriod(period))
                                    {
                                        // Kiểm tra xem New hiện tại có phải là New đang được xử lý không
                                        if (newElement == this)
                                        {
                                            break; // Dừng duyệt khi bạn đến New hiện tại
                                        }

                                        // Cộng dồn thời gian từ các New trước đó
                                        xTime += newElement.getTime();
                                    }

                                    // Khởi tạo TimeSet
                                    TimeSet iTime = new TimeSet(Time, period, inputDay, inputMonth, nameChannel, this.Name);
                                    iTime.setChannelOfTimeSet(nameChannel);
                                    iTime.setBanTinOfTimeSet(this.Name);
                                    iTime.setDayOfTimeSet(inputDay + "/" + inputMonth + "/" + "2024");

                                    // Điều chỉnh currentTime dựa trên xTime đã tích lũy
                                    TimeSpan xtimeToAdd = TimeSpan.FromSeconds(xTime);
                                    currentTime = currentTime.Add(xtimeToAdd);
                                    // Tạo timeStart cho New
                                    DateTime iTimeStart = new DateTime(
                                        iCalendarDay.Calendar.getYear(),
                                        iCalendarDay.Calendar.getMonth(),
                                        iCalendarDay.getDay()
                                    ).Add(currentTime);

                                    iTime.setTimeStart(iTimeStart);
                                    this.listTime.Add(iTime);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public class BanTinList
    {
        public BanTinList()
        {
        }

        public List<BanTin> Bantins { get; set; } = new List<BanTin>();

        public string SerializeToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }

        // Phương thức để deserialize chuỗi JSON thành đối tượng BanTinList
        public static BanTinList DeserializeFromJson(string json)
        {
            return JsonSerializer.Deserialize<BanTinList>(json);
        }

        public void Add(BanTin banTin)
        {
            Bantins.Add(banTin);
        }
    }

}
