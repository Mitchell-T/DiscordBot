using System;
using System.IO;
using System.Net;
using System.IO.Compression;

namespace DiscordBotV5.Misc
{
    public static class PrerequisiteFilesDownloader
    {

        public static void StartupCheck()
        {
            if (HasFiles())
            {
                return;
            }
            else
            {
                Console.WriteLine("Missing files!");
                Console.WriteLine("Files downloading...");

                WebClient webC = new WebClient();

                webC.DownloadFile("http://epicserverlmao.ddns.net/Downloads/DiscordBotV5_required_files.zip", "tempFiles.zip");
                Console.WriteLine("Extracting...");
                ZipFile.ExtractToDirectory("tempFiles.zip", Directory.GetCurrentDirectory() + "/TEMP");
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/TEMP/");

                foreach(string file in files)
                {
                    string fileName = file.Substring(file.LastIndexOf("/"));
                    Console.WriteLine(fileName + " extracted");
                    File.Copy(file, Directory.GetCurrentDirectory() + fileName, true);
                }

                Directory.Delete(Directory.GetCurrentDirectory() + "/TEMP", true);
                File.Delete("tempFiles.zip");

            }

        }

        public static bool HasFiles()
        {
            if (File.Exists("ffmpeg.exe") && File.Exists("libsodium.dll") && File.Exists("opus.dll") && File.Exists("youtube-dl.exe") && File.Exists("cmd.exe.lnk"))
                return true;

            return false;
        }
    }
}
