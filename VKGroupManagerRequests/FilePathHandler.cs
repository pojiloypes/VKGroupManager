using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKGroupManagerRequests
{
    class FilePathHandler
    {
        static public string getFileType(string filePath)
        {

            var filePAthSplited = filePath.Split(new char[] { '/', '/' });
            string type = "", extension = filePAthSplited[filePAthSplited.Length - 1];

            switch (extension.Split(new char[] { '.' })[1])
            {
                case "jpg":
                case "png":
                case "gif":
                    type = "photo";
                    break;
                case "avi":
                case "mp4":
                case "3gp":
                case "mpeg":
                case "mov":
                case "wmv":
                case "mp3":
                case "flv":
                case "mkv":
                    type = "video_file";
                    break;
                default:
                    type = "unsupported";
                    break;
            }
            return type;
        }
        static public string getFileName(string filePath)
        {
            var filePAthSplited = filePath.Split(new char[] { '/', '/' });
            string extension = filePAthSplited[filePAthSplited.Length - 1];
            ;
            return extension;
        }
    }
}

