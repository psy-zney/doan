using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doan
{
    public class Channel
    {
        private string name;
        private int limitNews;
        private List<New> sang;
        private List<New> toi;
        private string anchorName;

        public static List<Channel> channels = new List<Channel>();

        internal List<New> Sang { get { return sang; } set { sang = value; } }
        internal List<New> Toi { get { return toi; } set { toi = value; } }

        public Channel(string name, int limitNews)
        {
            anchorName = "Chưa có nguười dẫn chương trình";
            this.name = name;
            if (limitNews < 10 && limitNews >= 3)
                this.limitNews = limitNews;
            else
            {
                Console.WriteLine("So luong ban tin toi da cho 1 kenh la 10 va toi thieu la 3 ");
                return;
            }

            channels.Add(this);
            this.toi = new List<New>();
            this.sang = new List<New>();
        }

        public List<New> getListPeriod(string iPeriod)
        {
            if (iPeriod == "sang")
                return this.sang;
            else if (iPeriod == "toi")
                return this.toi;
            return null;
        }

        public int getLimitNews() { return limitNews; }
        public string getName() { return name; }


        public void setAnchor(string nameAnchor)
        {
            anchorName = nameAnchor;
        }

        public static List<Channel> getChanels()
        {
            return channels;
        }

        // thêm bản tin vào channel
        public void channelAddBanTin(string banTinName, string period)
        {
            New foundBanTin = null;
            foreach (New banTin in BanTin.listBanTins)
            {
                if (banTin.getName() == banTinName)
                {
                    foundBanTin = banTin;
                    break;
                }
            }
            if (foundBanTin != null)
            {
                if (period == "sang")
                {
                    sang.Add(foundBanTin);
                    foundBanTin.getListChannels().Add(this.name);
                    foundBanTin.setChanelName(this.name);
                }

                else if (period == "toi")
                {
                    toi.Add(foundBanTin);
                    foundBanTin.getListChannels().Add(this.name);
                    foundBanTin.setChanelName(this.name);
                }
                else
                {
                    Console.WriteLine("Lam on nhap lai");
                    return;
                }
            }

        }
        public void setTimeAndAddBanTin(New inew, string period, int inputDay, int inputMonth)
        {
            inew.setTime(period, this.getName(), inputDay, inputMonth);
            this.channelAddBanTin(inew.getName(), period);
        }
        public void printAll()
        {
            Console.WriteLine("Kenh " + name + " gom cac ban tin :");
            Console.WriteLine("Ban tin buoi sang: ");
            if (sang.Count == 0)
                Console.WriteLine("khong co ban tin buoi sang ");
            else
            {
                int i = 1;
                foreach (New banTin in sang)
                    Console.WriteLine(i + ". " + banTin.getName());
                i++;
            }

            Console.WriteLine("Ban tin buoi toi: ");
            if (toi.Count == 0)
                Console.WriteLine("khong co ban tin buoi toi ");
            else
            {
                int i = 1;
                foreach (New banTin in toi)
                    Console.WriteLine(i + ". " + banTin.getName());
                i++;
            }
        }

    }
}
