using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VKGroupManagerRequests
{
    public class Post
    {
        public int id;
        public string text;
        public DateTime date;

        [JsonProperty("id")]
        public int Id 
        { 
            get { return id; }
            set { id = value; }
        }

        [JsonProperty("text")]
        public string Text 
        { 
            get { return text; }
            set 
            {
                text = value;
                int stringCount = 1;
                for(int i=60; i < text.Length; i = stringCount * 60)
                {
                    while (i < text.Length && text[i] != ' ')
                        i++;
                    if (i < text.Length-1)
                        text = text.Insert(i, "\n");
                    stringCount++;
                }
            }
        }

        [JsonProperty("date")]
        public int Date
        {
            set { date = UnixTimeToDateTime(value); }
        }

        public string mediaFilePath { get; set; }

        public int commentsCount { get; set; }

        private DateTime UnixTimeToDateTime(DateTime value)
        {
            throw new NotImplementedException();
        }

        public int likesCount { get; set; }

        public int viewsCount { get; set; }

        public int repostsCount { get; set; }

        public List<Attachement> attachements { get; set; }

        DateTime UnixTimeToDateTime(long unixTime)
        {
            // Unix Time начинается с 1 января 1970 года, в UTC
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime).ToLocalTime(); // Преобразование в локальное время
        }

        public override string ToString()
        {
            //return "\tТекст: " + text.Replace("\n", "\n\t\t") + "\n" +
            //     "\tКомментарии: " + commentsCount + "" +
            //      "\tЛайки: " + likesCount + "" +
            //       "\tПросмотры: " + viewsCount + "" +
            //       "\tРепосты: " + repostsCount + 
            //       "\tДата: " + date + "\n";
            return "Текст: " + text + "\n" +
                "Число вложений: " + attachements.Count + "\n" +
                  "Комментарии: " + commentsCount + "" +
                  "\t\tЛайки: " + likesCount + "" +
                   "\tПросмотры: " + viewsCount + "" +
                   "\tРепосты: " + repostsCount +
                   "\tДата: " + date + "\n";
        }
    }
}
