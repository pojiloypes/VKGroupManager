using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VKGroupManagerRequests
{
    [Serializable]
    public class User
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("photo_200")]
        public string photo { get; set; }

        [JsonProperty("bdate")]
        public string bdate { get; set; }

        [JsonProperty("first_name")]
        public string first_name { get; set; }

        [JsonProperty("last_name")]
        public string last_name { get; set; }

        [JsonProperty("phone")]
        public string phone { get; set; }

        [JsonProperty("screen_name")]
        public string screen_name { get; set; }

        [JsonProperty("sex")]
        public string sex { get; set; }

        public override string ToString()
        {
            string res = "";
            res += "Имя Фамилия: " + first_name + " " + last_name + "\n" +
                 "Статус: " + status + "\n" +
                  "Телефон: " + phone + "\n" +
                   "Тэг: " + screen_name + "\n" +
                    "Дата рождения: " + bdate + "\n"
                    + "Пол: ";
            if (sex == "2") res += "мужской\n";
            else if (sex == "1") res += "женский\n";
            else res += "не указан\n";

            return res;
        }
    }
}
