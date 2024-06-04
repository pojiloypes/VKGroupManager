using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;

namespace VKGroupManagerRequests
{
    public class Manager
    {
        HttpClient client;
        JsonParser jsonParser;
        ServerLoader serverLoader;
        string server;
        string accesTocken;
        string appId;
        

        public Manager()
        {
            server = "https://api.vk.com/method/";
            accesTocken = "vk1.a.eZvNY22sx7SLIYRUffvmY4u5BPtdAuzEL6X7irbSX36wVfei9mjZpzgOQwwN0AlAiOLSV6lBxnEAbL-MHE8wBSO9I42Dkhc97sHtTva1YdF9XYx8cJqs1jYGNBehZ3DVmZ-dqiysd1-w1viQIUFmWwOCwqfZR-s3W-ludsdXKxLYehi6AfPdIfBXLobQWbv2qTBoR8JbRH2crGTcpwW1Ig&expires_in=86400&user_id=274020754";
            appId = "51772080";
            client = new HttpClient();
            serverLoader = new ServerLoader(server, accesTocken);
            jsonParser = new JsonParser(serverLoader);
        }

        public void auth()
        {
            //https://oauth.vk.com/authorize?client_id=51772080&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=wall,groups,photos,video&response_type=token&v=5.131
            //https://oauth.vk.com/authorize?client_id=51772080&redirect_uri=https://oauth.vk.com/blank.html&scope=account&display=page&responce_type=token

        }

        public async Task<User> getProfileInfo()
        {
            HttpResponseMessage responce = await client.GetAsync(server + "account.getProfileInfo?access_token=" + accesTocken + "&v=5.199");
            string responceText = await responce.Content.ReadAsStringAsync();
            jsonParser.responceText = responceText;

            User user = jsonParser.parseToGetUserProfileInfo();// = JObject.Parse(responceText)["response"].ToObject<User>();

            return user;
        }

        public async Task<List<Group>> getGroupsList(int userId)
        {
            HttpResponseMessage responce = await client.GetAsync(server + "groups.get?user_id=" + userId + "&access_token=" + accesTocken + "&extended=1&fields=membersCount&filter=admin&v=5.199");
            string responceText = await responce.Content.ReadAsStringAsync();

            jsonParser.responceText = responceText;
            List<Group> groups = jsonParser.parseToGetGroupsList();

            return groups;
        }

        private void addNewPostToPOstList(List<Post> posts, Post post)
        {
            posts.Add(post);
        }

        public async Task<List<Post>> getWallPosts(int userId)
        {
            HttpResponseMessage responce = await client.GetAsync(server + "wall.get?owner_id=-" + userId + "&access_token=" + accesTocken + "&extended=0&v=5.199");
            string responceText = await responce.Content.ReadAsStringAsync();

            jsonParser.responceText = responceText;
            List<Post> posts = await jsonParser.parseToGgetWallPosts();

            return posts;
        }

        public async Task createPost(int groupId, string message)
        {
            var parametrs = new Dictionary<string, string>
            {
                { "access_token", accesTocken },
                { "owner_id", "-" + groupId },
                { "from_group", "1" },
                { "message", message },
                { "v", "5.199" }
            };

            var response = await client.PostAsync(server + "wall.post", new FormUrlEncodedContent(parametrs));

            string responceText = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responceText);
        }

        private async Task<string> inputPostAttachments(int groupId)
        {
            string fileIdent, attachments = "";
            List<string> fileIdents = new List<string>();

            Console.Write("Хотите ли вы добавить файл?(Да\\Нет): ");
            string addFilesAns = Console.ReadLine();

            while (addFilesAns == "Да" || (addFilesAns != "Да" && addFilesAns != "Нет"))
            {
                if (addFilesAns != "Да" && addFilesAns != "Нет") Console.WriteLine("Неверный ответ");
                else
                {
                    Console.Write("Введите ссылку на файл: ");
                    string filePath = Console.ReadLine().Replace("\\", "//");
                    if (FilePathHandler.getFileType(filePath) != "unsupported")
                    {
                        fileIdent = await serverLoader.getFileIdent(groupId, filePath);
                        fileIdents.Add(fileIdent);
                    }
                }
                Console.Write("Хотите ли вы добавить файл?(Да\\Нет): ");
                addFilesAns = Console.ReadLine();
            }


            for (int i = 0; i < fileIdents.Count; i++)
            {
                if (i == 0) attachments += fileIdents[i];
                else attachments += "," + fileIdents[i];
            }

            return attachments;
        }

        public async Task createPostWithFile(int groupId)
        {
            Console.Write("Введите текст сообщения: ");
            string message = Console.ReadLine();


            var parametrs = new Dictionary<string, string>
            {
                { "access_token", accesTocken },
                { "owner_id", "-"+groupId },
                { "v", "5.199" },
                { "message", message }
            };
            string attachments = await inputPostAttachments(groupId);
            if (attachments.Length > 0) parametrs.Add("attachments", attachments);

            var response = await client.PostAsync(server + "wall.post", new FormUrlEncodedContent(parametrs));

            var responceText = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responceText);
        }

        public async Task deletePost(int groupId, int postId)
        {
            var result = await client.GetAsync(server + "wall.delete?owner_id=-" + groupId + "&post_id=" + postId + "&access_token=" + accesTocken + "&extended=0&v=5.199");
            //Console.WriteLine("https://" + server + "wall.get?owner_id=-223100201&access_token=" + accesTocken + "&extended=0&v=5.199");
            Console.WriteLine(await result.Content.ReadAsStringAsync());
        }

        public async Task saveMediaObjectOnLocalMachine()
        {
            string path = "C://Users//tyver//source//repos//VKGroupManagerRequests//VKGroupManagerRequests//currentSessionMeadiaObjects";
            string newName = "//media.jpg";
            Stream fileStream = await client.GetStreamAsync("https://sun9-22.userapi.com/impg/ltFhdirVTh9Dfh4q-PydRbnGC11CYAlF5x5RJw/H5gB0Jjc_I8.jpg?size=510x510&quality=96&sign=1a805546b7e951b426ab353bf1863e32&c_uniq_tag=8PTQUHlN7NZdMYqGYUKbBLEdzs00vcSekNoMD_Z-iYM&type=album");
            using (FileStream outputFileStream = new FileStream(path + newName, FileMode.CreateNew))
            {
                await fileStream.CopyToAsync(outputFileStream);
            }
        }
    }
}
