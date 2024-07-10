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
using System.Diagnostics;

namespace VKGroupManagerRequests
{


    class Program
    {
        static void printHelp(int code)
        {
            switch(code)
            {
                case 1:
                    Console.WriteLine("\n0 - Справка \n1 - Посмотреть список групп \n2 - Работать с группой \n-1 - Выход");
                    break;
                case 2:
                    Console.WriteLine("\n0 - Справка \n1 - Посмотреть посты \n2 - Сменить группу \n3 - Создать пост \n4 - Удалить пост \n5 - Загрузить еще посты \n6 - Работа с постом\n-1 - Назад");
                    break;
                case 3:
                    Console.WriteLine("\n0 - Справка \n1 - Посмотреть пост \n2 - Сменить пост \n3 - Удалить пост \n-1 - Назад");
                    break;
            }
        }

        static void printGroups(List<Group> myGroups)
        {
            int idx = 1;
            foreach (Group group in myGroups)
            {
                Console.WriteLine("\n[Группа №" + idx++ + "]\n" + group.ToString()); 
            }
        }
        static void printPosts(List<Post> posts)
        {
            int idx = 1;
            foreach (Post post in posts)
            {
                Console.WriteLine("\n[Пост №" + idx++ + "]\n" + post.ToString());
            }
        }
        static async Task printAttachements(Post post)
        {
            int counter = 1;
            foreach (Attachement att in post.attachements)
            {
                if (att.filePath == "")
                {
                    Console.Write("Загрузка файла №" + counter++ + "...   ");
                    att.filePath = await ServerLoader.saveMediaObjectOnLocalMachine(att.fileUrl, att.type);
                    Console.Write("OK\n");
                }
            }

            if (post.attachements.Count > 0)
                Console.WriteLine("Вложения: ");

            counter = 1;
            foreach (Attachement att in post.attachements)
            {
                Console.WriteLine("Файл №" + counter++ + ": " + att.filePath);
            }
        }

        static async Task printFullPost(Post post, int idx)
        {
            Console.WriteLine("\n[Пост №" + idx + "]\n" + post.ToString());
            await printAttachements(post);
        }

        static int inputIdx(int left, int right, string numberOf)
        {
            int idx = left - 1;
            while (idx < left || idx >= right)
            {
                Console.Write("[X] Введите номер " + numberOf + ": ");
                idx = int.Parse(Console.ReadLine()) - 1;
            }
            return idx;
        }

        static async Task deletePost(Manager man, int groupId, int postId)
        {
            await man.deletePost(groupId, postId);
        }

        static async Task postWork(Manager man, List<Post> posts, int groupId)
        {
            int idx = inputIdx(0, posts.Count, "поста");

            await printFullPost(posts[idx], idx);

            bool endFlag = false;
            int actCode;

            printHelp(3);
            while (!endFlag)
            {
                Console.Write("\n[X] Введите номер команды: ");
                actCode = int.Parse(Console.ReadLine());

                switch (actCode)
                {
                    case 0: // Справка
                        printHelp(3);
                        break;
                    case 1: // Вывести инфу о посте вместе c сложениями
                        await printFullPost(posts[idx], idx);
                        break;
                    case 2: // Поменять номер рабочего поста
                        idx = inputIdx(0, posts.Count, "поста");
                        break;
                    case 3: // Удалить рабочий пост
                        await deletePost(man, groupId, posts[idx].id);
                        posts = await man.getWallPosts(groupId);
                        printPosts(posts);
                        idx = inputIdx(0, posts.Count, "поста");
                        break;
                    case -1:  // Назад
                        endFlag = true;
                        break;
                }
            }
        }
        static async Task groupWork(List<Group> myGroups, Manager man)
        {
            int idx = inputIdx(0, myGroups.Count, "группы");

            List<Post> posts = await man.getWallPosts(myGroups[idx].id);
            bool endFlag = false;
            int actCode;

            printHelp(2);
            while (!endFlag)
            {
                Console.Write("\n[X] Введите номер команды: ");
                actCode = int.Parse(Console.ReadLine());

                switch (actCode)
                {
                    case 0:  // Справка
                        printHelp(2);
                        break;
                    case 1: // Посмотреть список постов
                        printPosts(posts);
                        break;
                    case 2: // Сменить группу для работы
                        idx = inputIdx(0, myGroups.Count, "группы");
                        posts = await man.getWallPosts(myGroups[idx].id);
                        break;
                    case 3: // Создать пост в группе
                        await man.createPostWithFile(myGroups[idx].id);
                        posts = await man.getWallPosts(myGroups[idx].id);
                        printHelp(2);
                        break;
                    case 4: // Удалить пост в группе
                        await deletePost(man, myGroups[idx].id, posts[inputIdx(0, posts.Count, "поста")].id);
                        posts = await man.getWallPosts(myGroups[idx].id);
                        printPosts(posts);
                        break;
                    case 5:
                        List<Post> offsetPosts = await man.getWallPosts(myGroups[idx].id, posts.Count);
                        if (offsetPosts.Count == 0)
                            Console.WriteLine("Незагруженных постов больше нет");
                        else
                            posts.AddRange(offsetPosts);
                        break;
                    case 6: // Работа с постом
                        await postWork(man, posts, myGroups[idx].id);
                        printHelp(2);
                        break;
                    case -1:  // Назад
                        endFlag = true;
                        break;
                    default:
                        Console.WriteLine("Нераспознанная команда");
                        break;
                }
            }
        }

    static async Task Main(string[] args)
        {
            Manager man = new Manager();
            await man.auth();

            User user = await man.getProfileInfo();
            List<Group> myGroups = await man.getGroupsList(user.id);

            bool endFlag = false;
            int actCode;

            printHelp(1);
            while (!endFlag)
            {
                Console.Write("\n[X] Введите номер команды: ");
                actCode = int.Parse(Console.ReadLine());

                switch(actCode)
                {
                    case 0: // Справка
                        printHelp(1);
                        break;
                    case 1: // Вывести список групп
                        printGroups(myGroups);
                        break;
                    case 2: // Работа с группой
                        await groupWork(myGroups, man);
                        printHelp(1);
                        break;
                    case -1: // Выход
                        endFlag = true;
                        break;
                    default:
                        Console.WriteLine("Нераспознанная команда");
                        break;
                }
            }
            ////"C:\Users\tyver\source\repos\VKGroupManagerRequests\VKGroupManagerRequests\3dchelik.pdf"
        }
    }
}
