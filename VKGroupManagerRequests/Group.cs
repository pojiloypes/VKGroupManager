using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace VKGroupManagerRequests
{
    public class Group
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("members_count")]
        public int membersCount { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("photo_50")]
        public string photo_50 { get; set; }

        [JsonProperty("photo_100")]
        public string photo_100 { get; set; }

        [JsonProperty("photo_200")]
        public string photo_200 { get; set; }

        [JsonProperty("screen_name")]
        public string screenName { get; set; }

        public override string ToString()
        {
            return "Название: " + name + "\n" +
                 "Тэг: " + screenName + "\n" +
                  "Подписчики: " + membersCount + "\n";
        }
    }
}
