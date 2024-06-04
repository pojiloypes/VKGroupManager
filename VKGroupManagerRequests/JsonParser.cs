using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace VKGroupManagerRequests
{
    class JsonParser
    {
        public string responceText { get; set; }
        JObject parsedResponce { get; set; }
        JToken jToken { get; set; }

        ServerLoader serverLoader;

        public JsonParser(ServerLoader server)
        {
            serverLoader = server;
        }

        public User parseToGetUserProfileInfo() =>
            JObject.Parse(responceText)["response"].ToObject<User>();


        public List<Group> parseToGetGroupsList()
        {
            List<Group> groups = new List<Group>();
            JObject parsedRsponce = JObject.Parse(responceText);
            List<JToken> results = parsedRsponce["response"]["items"].Children().ToList();

            foreach (JToken result in results)
            {
                Group newgroup = result.ToObject<Group>();
                groups.Add(newgroup);
            }

            return groups;
        }

        private async Task parseJsonToFillNewAttachement(Attachement attachement)
        {
            string type = jToken["type"].ToString();
            attachement.type = type;
            switch (type)
            {
                case "photo":
                    attachement.fileUrl = jToken[type]["sizes"][4]["url"].ToString();
                    break;
                case "audio":
                    attachement.fileUrl = jToken[type]["url"].ToString();
                    break;
            }
            attachement.filePath = await serverLoader.saveMediaObjectOnLocalMachine(attachement.fileUrl, type);
        }

        private async Task parseJsonToFillNewPost(Post newPost)
        {
            //newPost = jToken.ToObject<Post>();
            newPost.attachements = new List<Attachement>();
            newPost.likesCount = int.Parse(jToken["likes"]["count"].ToString());
            newPost.commentsCount = int.Parse(jToken["comments"]["count"].ToString());
            newPost.viewsCount = int.Parse(jToken["views"]["count"].ToString());
            newPost.repostsCount = int.Parse(jToken["reposts"]["count"].ToString());

            var attaches = jToken["attachments"].Children().ToList();
            foreach (JToken attach in attaches)
            {
                jToken = attach;
                Attachement attachement = new Attachement();
                await parseJsonToFillNewAttachement(attachement);
                newPost.attachements.Add(attachement);
            }
        }

        public async Task<List<Post>> parseToGgetWallPosts()
        {
            List<Post> posts = new List<Post>();
            parsedResponce = JObject.Parse(responceText);

            var results = parsedResponce["response"]["items"].Children().ToList();
            foreach (JToken result in results)
            {
                Post newPost = new Post();
                jToken = result;
                await parseJsonToFillNewPost(newPost);
                posts.Add(newPost);
            }

            return posts;
        }
    }
}
