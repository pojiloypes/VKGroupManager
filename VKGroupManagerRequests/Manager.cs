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
        string server;
        string accesTocken;
        string appId;

        public async Task auth()
        {
            if (!File.Exists("accesToken.txt"))
                File.WriteAllText("accesToken.txt", "");

            client = new HttpClient();
            string readToken = TokenEncryptor.Decrypt(File.ReadAllText("accesToken.txt"));

            HttpResponseMessage responce = await client.GetAsync(server + "account.getProfileInfo?access_token=" + readToken + "&v=5.199");
            string responceText = await responce.Content.ReadAsStringAsync();
            
            while(responceText.Contains("error"))
            {
                Console.WriteLine("[X] Для получения доступа перейдите по данной ссылке и введите ссылку на странице после авторизации:\n" +
                    "https://oauth.vk.com/authorize?client_id=51772080&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=wall,groups,photos,video&response_type=token&v=5.131");

                readToken = Console.ReadLine(); 
                readToken = readToken.Split("access_token=")[1].Split("&expires_in")[0];
                responce = await client.GetAsync(server + "account.getProfileInfo?access_token=" + readToken + "&v=5.199");
                responceText = await responce.Content.ReadAsStringAsync();
            }

            accesTocken = readToken;
            ServerLoader.run(server, accesTocken);
            jsonParser = new JsonParser();
            File.WriteAllText("accesToken.txt", TokenEncryptor.Encrypt(accesTocken));

        }
        public Manager()
        {
            server = "https://api.vk.com/method/";
            appId = "51772080";
        }

        public async Task<User> getProfileInfo() // Считать информаицю профиля
        {
            HttpResponseMessage responce = await client.GetAsync(server + "account.getProfileInfo?access_token=" + accesTocken + "&v=5.199");
            string responceText = await responce.Content.ReadAsStringAsync();
            jsonParser.responceText = responceText;

            User user = jsonParser.parseToGetUserProfileInfo();// = JObject.Parse(responceText)["response"].ToObject<User>();

            return user;
        }

        public async Task<List<Group>> getGroupsList(int userId)  // Получить список групп под управлением пользователя
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

        public async Task<List<Post>> getWallPosts(int userId)  // Получить список постов группы
        {
            HttpResponseMessage responce = await client.GetAsync(server + "wall.get?owner_id=-" + userId + "&access_token=" + accesTocken + "&count=500&extended=0&v=5.199");
            string responceText = await responce.Content.ReadAsStringAsync();

            jsonParser.responceText = responceText;
            List<Post> posts = await jsonParser.parseToGgetWallPosts();

            return posts;
        }

        public async Task<List<Post>> getWallPosts(int userId, int offset)  // Получить список постов группы со смещением
        {
            HttpResponseMessage responce = await client.GetAsync(server + "wall.get?owner_id=-" + userId + "&access_token=" + accesTocken + "&offset" + offset + "&count=500&extended=0&v=5.199");
            string responceText = await responce.Content.ReadAsStringAsync();

            jsonParser.responceText = responceText;
            List<Post> posts = await jsonParser.parseToGgetWallPosts();

            return posts;
        }

        public async Task createPost(int groupId, string message)  // Создать пост без вложений
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

        private async Task<string> inputPostAttachments(int groupId)  // Ввести инфу о вложениях для поста
        {
            string fileIdent, attachments = "";
            List<string> fileIdents = new List<string>();

            Console.Write("[X] Хотите ли вы добавить файл?(Да\\Нет): ");
            string addFilesAns = Console.ReadLine();

            while (addFilesAns == "Да" || (addFilesAns != "Да" && addFilesAns != "Нет"))
            {
                if (addFilesAns != "Да" && addFilesAns != "Нет") Console.WriteLine("Неверный ответ");
                else
                {
                    Console.Write("[X] Введите ссылку на файл: ");
                    string filePath = Console.ReadLine().Replace("\\", "//");
                    if (FilePathHandler.getFileType(filePath) != "unsupported")
                    {
                        fileIdent = await ServerLoader.getFileIdent(groupId, filePath);
                        fileIdents.Add(fileIdent);
                    }
                    else
                        Console.WriteLine("Неподдерживаемый формат файла");
                }
                Console.Write("[X] Хотите ли вы добавить файл?(Да\\Нет): ");
                addFilesAns = Console.ReadLine();
            }


            for (int i = 0; i < fileIdents.Count; i++)
            {
                if (i == 0) attachments += fileIdents[i];
                else attachments += "," + fileIdents[i];
            }

            return attachments;
        }

        public async Task createPostWithFile(int groupId) // Создать пост с вложением
        {
            Console.Write("[X] Введите текст сообщения: ");
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
        }

        public async Task deletePost(int groupId, int postId) // Удалить пост
        {
            var result = await client.GetAsync(server + "wall.delete?owner_id=-" + groupId + "&post_id=" + postId + "&access_token=" + accesTocken + "&extended=0&v=5.199");
        }

        public async Task saveMediaObjectOnLocalMachine() // Сохранить влоежния поста на устройство пользователя
        {
            string path = "C://Users//tyver//source//repos//VKGroupManagerRequests//VKGroupManagerRequests//currentSessionMeadiaObjects";
            string newName = "//media.jpg";
            Stream fileStream = await client.GetStreamAsync("https://sun9-22.userapi.com/impg/ltFhdirVTh9Dfh4q-PydRbnGC11CYAlF5x5RJw/H5gB0Jjc_I8.jpg?size=510x510&quality=96&sign=1a805546b7e951b426ab353bf1863e32&c_uniq_tag=8PTQUHlN7NZdMYqGYUKbBLEdzs00vcSekNoMD_Z-iYM&type=album");
            using (FileStream outputFileStream = new FileStream(path + newName, FileMode.CreateNew))
            {
                await fileStream.CopyToAsync(outputFileStream);
            }
        }

        ~Manager()
        {
            ServerLoader.Clear();
        }
    }
}
