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
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("text")]
        public string text { get; set; }

        public string mediaFilePath { get; set; }

        public int commentsCount { get; set; }

        public int likesCount { get; set; }

        public int viewsCount { get; set; }

        public int repostsCount { get; set; }

        public List<Attachement> attachements { get; set; }

        public override string ToString()
        {
            if (text == null)
                text = "";

            return "\tТекст: " + text.Replace("\n", "\n\t\t") + "\n" +
                 "\tКомментарии: " + commentsCount + "" +
                  "\tЛайки: " + likesCount + "" +
                   "\tПросмотры: " + viewsCount + "" +
                   "\tРепосты: " + repostsCount + "\n";
        }
    }
}
