using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;


namespace VKGroupManagerRequests
{
   
    class Program
    {
        static async Task Main(string[] args)
        {
            Manager man = new Manager();

            User user = await man.getProfileInfo();

            int idTrrp = 223100201;
            List<Post> posts = await man.getWallPosts(idTrrp);
            int counter = 1;
            foreach (Post post in posts)
            {
                Console.WriteLine("\tПост №" + counter++);
                Console.WriteLine(post.ToString());
                //foreach (Attachement att in post.attachements) Console.WriteLine(att.filePath);

            }
            //await man.createPostWithFile(idTrrp);

            //"C:\Users\tyver\source\repos\VKGroupManagerRequests\VKGroupManagerRequests\3dchelik.pdf"

            List<Group> myGroups = await man.getGroupsList(user.id);
            //foreach (Group group in myGroups)
            //{
            //    Console.WriteLine(group.ToString());

            //    int counter = 1;
            //    List<Post> posts = await man.getWallPosts(group.id);
            //    foreach (Post post in posts)
            //    {
            //        Console.WriteLine("\tПост №" + counter++);
            //        Console.WriteLine(post.ToString());
            //        //foreach (Attachement att in post.attachements) Console.WriteLine(att.filePath);

            //    }
            //}


            //await man.deletePost(client, "223100201", "4");
        }
    }
}
