using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;


namespace VKGroupManagerRequests
{
    public static class ServerLoader
    {
        static string server;
        static string accesTocken;
        static HttpClient client;

        public static void run(string newServer, string newAccesTocken)
        {
            client = new HttpClient();
            server = newServer;
            accesTocken = newAccesTocken;
        }

        public static async Task<string> saveMediaObjectOnLocalMachine(string url, string postType)
        {
            var bytes = new byte[8];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            string hashFileName = BitConverter.ToString(bytes).Replace("-", "").ToLower();

            string newName, path = "currentSessionMeadiaObjects//";

            newName = postType + "_" + hashFileName + Path.GetExtension(url).Split("?")[0];

            Stream fileStream = await client.GetStreamAsync(url);
            using (FileStream outputFileStream = new FileStream(path + newName, FileMode.CreateNew))
            {
                await fileStream.CopyToAsync(outputFileStream);
            }

            return path + newName;
        }

        private static async Task<string> getServerToUploadPicture(int groupId)
        {
            HttpResponseMessage responce = await client.GetAsync(server + "photos.getWallUploadServer?access_token=" + accesTocken + "&group_id=" + groupId + "&v=5.199");
            string responceText = await responce.Content.ReadAsStringAsync();

            return responceText.Split("\"upload_url\":\"")[1].Split("\",\"user_id\"")[0];
        }


        private static async Task<string> getServerToUploadVideo(int groupId)
        {
            HttpResponseMessage responce = await client.GetAsync(server + "video.save?access_token=" + accesTocken + "&group_id=" + groupId + "&v=5.199");
            string responceText = await responce.Content.ReadAsStringAsync();

            return responceText.Split("\"upload_url\":\"")[1].Split("\",\"video_id\"")[0];
        }


        private static async Task<string> getServerToUploadObject(int groupId, string dataType)
        {
            string address = "";
            switch (dataType)
            {
                case "photo":
                    address = await getServerToUploadPicture(groupId);
                    break;
                case "video_file":
                    address = await getServerToUploadVideo(groupId);
                    break;
            }

            return address;
        }


        private static async Task<string> uploadObjectToServer(int groupId, string filePath)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            byte[] byteOfImage = System.IO.File.ReadAllBytes(filePath);
            string fileType = FilePathHandler.getFileType(filePath);

            content.Add(new StreamContent(new MemoryStream(byteOfImage)), fileType, FilePathHandler.getFileName(filePath));

            string serverAdress = (await getServerToUploadObject(groupId, fileType)).Replace("\\/", "/");

            var response = await client.PostAsync(serverAdress, content);
            string responceText;

            using (var sr = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                responceText = sr.ReadToEnd();
            }

            return responceText;
        }

        private static async Task<MultipartFormDataContent> getContentForSavePicture(int groupId, string filePath)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            string responceFromServer = await uploadObjectToServer(groupId, filePath);

            content.Add(new StringContent(groupId.ToString()), "group_id");
            content.Add(new StringContent(accesTocken), "access_token");
            content.Add(new StringContent("5.131"), "v");

            content.Add(new StringContent(responceFromServer.Split("\"server\":")[1].Split(",\"photo")[0]), "server");
            content.Add(new StringContent(responceFromServer.Split("\"photo\":\"")[1].Split("\",\"hash\"")[0].Replace("\\", "")), "photo");
            content.Add(new StringContent(responceFromServer.Split("\"hash\":\"")[1].Split("\"")[0]), "hash");

            return content;
        }

        private static async Task<string> savePictureOnServer(int groupId, string filePath)
        {
            MultipartFormDataContent content = await getContentForSavePicture(groupId, filePath);

            var response = await client.PostAsync(server + "photos.saveWallPhoto", content);
            string responceText = await response.Content.ReadAsStringAsync();

            string id = responceText.Split("\"id\":")[1].Split(",\"owner_id\"")[0];
            string owner_id = responceText.Split(",\"owner_id\":")[1].Split(",\"access_key\"")[0];
            string fileIdent = "photo" + owner_id + "_" + id;

            return fileIdent;
        }

        private static async Task<string> saveVideoOnServer(int groupId, string filePath)
        {
            string fileIdent;
            string responceFromServer = await uploadObjectToServer(groupId, filePath);

            string video_id = responceFromServer.Split("\"video_id\":")[1];
            string owner_id = responceFromServer.Split(",\"owner_id\":")[1].Split(",\"video_id\":")[0];

            fileIdent = "video" + owner_id + "_" + video_id;
            return fileIdent;
        }

        public static async Task<string> getFileIdent(int groupId, string filePath)
        {
            string fileIdent = "";

            switch (FilePathHandler.getFileType(filePath))
            {
                case "photo":
                    fileIdent = await savePictureOnServer(groupId, filePath);
                    break;
                case "video_file":
                    fileIdent = await saveVideoOnServer(groupId, filePath);
                    break;
            }

            return fileIdent;
        }

        public static void Clear()
        {
            DirectoryInfo dirInfo = new DirectoryInfo("currentSessionMeadiaObjects//");

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                file.Delete();
            }
        }
    }


}
