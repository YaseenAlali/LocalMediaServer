using System.Net.Sockets;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting.Server;
using System.Web;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.AccessControl;
using System;
using System.Diagnostics;
using System.Threading;
using static System.Net.WebRequestMethods;
using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Y2Sharp;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static WebAppTest1.AudioController;
using static WebAppTest1.AudioStreamer;
using System.Xml.Linq;


namespace WebAppTest1
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelperFunctions : ControllerBase
    {
        public class General { 
            public static List<string> SongsList = new List<string>();
            public static List<string> SongsListPath = new List<string>();
            
            public static string TempDownloadFile = string.Empty;
            public static bool admin = false;
            
            public static string GetLocalIPAddress(string port = "50000")
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return $"http://{localIP}:{port}";
        }
            public static string GetPublicIPAddress(string port = "50000")
            {
                HttpClient client = new HttpClient();
                string publicIp = client.GetStringAsync("https://api.ipify.org").Result;
                return $"http://{publicIp}:{port}";
            }
            public static  void Run_Process(string args)
        {
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.CreateNoWindow = false;
                myProcess.StartInfo.FileName = "cmd";
                myProcess.StartInfo.Arguments = args;
                myProcess.Start();
                myProcess.WaitForExit();
            }
        }
            public static bool isAdmin(string password)
            {
                //deprecated
                using (var sha1 = SHA1.Create())
                {
                    password = BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(password)));
                }
                return password == "59-48-72-97-4E-06-55-57-71-8A-CD-2A-85-19-81-10-84-B9-54-F0";
            }

            public static void ParseGetLogIn(HttpContext con)
            {
                if (con.Request.Query.Keys.Count > 0)
                {
                    if (con.Request.Query.ContainsKey("logout"))
                    {
                        if (con.Request.Query["logout"] == "true") { 
                            admin = false;
                            Security.generateOTP(1, 5);
                        }
                    }
                    else { 
                        if (con.Request.Query.ContainsKey("password")) { 
                            admin = Security.checkPassword(con.Request.Query["password"]);
                            if (!admin)
                            {
                                Security.oneTimePad = 0;
                                Security.generateOTP(1, 5);
                            }
						}
					}
                }
            }
        }


        public class FileSystem
        {
            public static string SongIndex = "";
			public static void GetFilesFromDirectory(string DirPath, bool api = false)
            {
                 
                string[] musicExtensions = { ".opus", ".m4a", ".mp3", ".mkv"}; //".mp4"
                DirectoryInfo Dir = new DirectoryInfo(DirPath);
                FileInfo[] FileList = Dir.GetFiles("*.*", SearchOption.AllDirectories);
    
                foreach (FileInfo FI in FileList)
                    foreach (var ext in musicExtensions)
                        if (ext == FI.Extension)
                        {
                            if (!api) {
                                General.SongsList.Add(FI.Name.Split('.')[0]);
                                General.SongsListPath.Add(FI.FullName);
                            }
                            else
                            {
                                General.TempDownloadFile = FI.FullName;
                            }
                            break;
                        }

            }
            public static void ParseGetDL(HttpContext con)
            {
                if (con.Request.Query.Keys.Count > 0)
                {
                    if (con.Request.Query.ContainsKey("logout")) return;

                    if (con.Request.Query.ContainsKey("refreshSongs"))
                    {
                        RefreshFileList();
                        return;
                    }
                    if (con.Request.Query.Keys.ToArray()[0].Split('_').Length > 1) { 
                        SongIndex = con.Request.Query.Keys.ToArray()[0].Split('_')[1];
                        ServeFile(con, General.SongsListPath[Convert.ToInt16(SongIndex)]);
                        SongIndex = "";
                    }
                }
            }
            public static long GetFileSize(string path)
            {
                FileInfo file = new FileInfo(path);
                return file.Length;
            }
            public static void ServeFile(HttpContext parContext, string path)
            {
                parContext.Response.Headers.Append("Content-Type", "audio/mpeg");
                try
                {
                    parContext.Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{Path.GetFileName(path)}\"");
                }
                catch (InvalidOperationException)
                {
                    parContext.Response.Headers.Append("Content-Disposition", $"attachment; filename=Music{Path.GetExtension(path)}");
                }
                
                using (FileStream fs = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    var buffer = new byte[4096];
                    while ((bs.Read(buffer,0,buffer.Length) != 0))
                    {
                        parContext.Response.Body.WriteAsync(buffer);
                    }
                    buffer = null;
                    bs.Close(); 
                }
                return;

            }
            public static void ParseGetYT(HttpContext con)
            {
                if (con.Request.Query.Keys.Count > 0)
                {
                    string URL = con.Request.Query["URL"];
                    string Name = con.Request.Query["NAME"];
                    //string audio = con.Request.Query["audio"];
                    //bool aud = true ? audio == "true" : false;

                    if (Name != "")
                    {
                        URL = YtSearch(Name);
                    }
                
                    Y2MateDownload(con,extractIdFromUrl(URL.Trim()), @"D:\stuff\music\Downloads\");
                }
                
            }
            public static void YtDlpDownload(HttpContext con, string URL, bool audio, string name="")
            {
                //deprecated
                string command = @"/C yt-dlp.exe --paths temp/";
                if (audio) 
                       command += " --extract-audio ";
                if (name != "")
                    command += $"-o \"{name}\" ";
                command += URL;
                General.Run_Process(command);
                GetFilesFromDirectory("temp" , true);
                ServeFile(con,General.TempDownloadFile);
                
                if (System.IO.File.Exists(General.TempDownloadFile))
                {
                    System.IO.File.Delete(General.TempDownloadFile);
                }
                
            }
            public static void Y2MateDownload(HttpContext con,string id, string path = "")
            {

                Y2Sharp.Youtube.Video.GetInfo(id).Wait();
                var video = new Y2Sharp.Youtube.Video();
                
               
                path += "download.mp3";
                video.DownloadAsync(path).Wait();
                


                GetFilesFromDirectory(@"D:\stuff\music\Downloads", true);
                ServeFile(con, General.TempDownloadFile);
                if (System.IO.File.Exists(General.TempDownloadFile))
                {
                    System.IO.File.Delete(General.TempDownloadFile);
                }
            }

            public static string extractIdFromUrl(string url)
            {
                string id = string.Empty;
                if (url.Contains("https://www.youtube.com/watch?v=")) //https://www.youtube.com/watch?v=lr5eStQ88cc
                {
                    id = url.Split('=')[1];
                }
                else if (url.Contains("https://youtu.be/"))
                {
                    id = url.Split("https://youtu.be/")[1];
                }
                return id;
                
            }
            public static string YtSearch(string name)
            {
                string command = "/C python3 YTsearch.py \"" + name + '"';
                General.Run_Process(command);
                

                return System.IO.File.ReadAllText("search.txt");
                
            }

            public static void RefreshFileList()
            {
                General.SongsList.Clear();
                General.SongsListPath.Clear();
                GetFilesFromDirectory(@"D:\stuff\music");
            }
        }


        public class StreamingService {
            public static int StreamSongIndex = -1;
            public static void ParseGetStreamSong(HttpContext con)
            {
                
                if (con.Request.Query.Keys.Count > 0)
                {
                    if (con.Request.Query.Keys.ToArray()[0].Split('-').Length  > 1) { 
                        StreamSongIndex = Convert.ToInt16(con.Request.Query.Keys.ToArray()[0].Split('-')[1]);
                        
                    }
                }
            }
        }

        public class Security
        {
            public static int oneTimePad = 0;
            public static int subValue = 0;
            public static int modValue = 0;
            public static void generateOTP(int min, int max)
            {
                Random rnd = new Random();
                for (int i = 0; i < 3; i++) {
                    oneTimePad += rnd.Next(min,max);
                }
                subValue = rnd.Next(1, 4);
                if (rnd.Next(0, 2) % 2 == 0) modValue = 5;
                else modValue = 10;
            }
            public static int autisticNumberGenerator(int n, int x, int m)
            {
                return 2*((n - x) % m);
            }
            public static bool checkPassword(string answer)
            {
                try
                {
                    int number = Convert.ToInt16(answer);
                    
                    if (number == autisticNumberGenerator(oneTimePad,subValue, modValue)) return true;
                }
                catch (Exception e)
                {
                    return false;
                }
                return false;
            }
        }

        public class TorrentService
        {
            public static string title = "";
            public static string seeders = "";
            public static string catogry = "";
            
            public static void parseTorrentGet(HttpContext con)
            {
                if (con.Request.Query.Keys.Count > 0)
                {
                    
                    if (con.Request.Query.ContainsKey("searchTitle"))
                    {
                        if (con.Request.Query["searchTitle"] != "") { 
						string searchtitle = "";
						searchtitle = con.Request.Query["searchTitle"];
                        if (con.Request.Query.ContainsKey("seachCatogry"))
                        {
                            catogry = con.Request.Query["seachCatogry"];
                            catogry = catogry.Trim();
                        }
                        else catogry = "";
                        string command = "/C python3 TorrentSearcher.py \"" + searchtitle + '"';
                        if (catogry == "movies" || catogry == "anime" || catogry == "tv") command += " " + catogry;
                        //title = command;
                        General.Run_Process(command);
                        setTitleAndSeeders();
                    }
                    if (con.Request.Query.ContainsKey("DownloadTorrent")){
                        string command = "/C python3 TorrentDownloader.py";
                        General.Run_Process(command);
                        }
					}

				}
            }
            public static void setTitleAndSeeders()
            {
                string[] lines = System.IO.File.ReadAllText("torrentTitle.txt").Split("\n");
                title = lines[0];
                seeders = lines[1];
            }

        }

    }

}
