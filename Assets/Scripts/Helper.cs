using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SajberSim.Story;
using SajberSim.Translation;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Compression;
using UnityEngine.SocialPlatforms;

namespace SajberSim.Helper
{
    public class Helper : MonoBehaviour
    {
        // genres translated
        public static string[] genres;
        // genres as how they are in the steamworks backend
        public static string[] genresSteam = new string[] { "Adventure", "Action", "Comedy", "Drama", "Fantasy", "Horror", "Magic", "Mystery", "Romance", "Sci-fi", "Slice of life", "Supernatural", "Other" };
        // genres in manifest
        public static string[] genresid = new string[] { "adventure", "action", "comedy", "drama", "fantasy", "horror", "magic", "mystery", "romance", "scifi", "sliceoflife", "supernatural", "other" };
        public static string[] audience = new string[] { "Everyone", "Questionable", "Mature" };
        public const uint AppID = 1353530;
        public static bool loggedin = false;
        public static int id = -1;
        public static string localPath = "";
        public static string customPath = "";
        public static string steamPath = "";
        public static string currentStoryPath = "";
        public static string currentStoryName = "";

        public static AS_AccountInfo acc;


        /// <summary>
        /// Positions for all 6 icons in the story menu
        /// </summary>
        public static Dictionary<int, Vector3> CardPositions = new Dictionary<int, Vector3>()
        {
            {0, new Vector3(0, 0, 1)},
            {1, new Vector3(330, 0, 1)},
            {2, new Vector3(660, 0, 1)},
            {3, new Vector3(0, -230, 1)},
            {4, new Vector3(330, -230, 1)},
            {5, new Vector3(660, -230, 1)}
        };
        public enum StorySearchArgs
        {
            Alphabetical,
            ReverseAlphabetical,
            LongestFirst,
            ShortestFirst,
            Newest,
            Oldest,
            Author,
            LastModified,
            ID
        }
        public enum StorySearchPaths
        {
            All, //all stories
            Workshop, //only workshop stories
            NoWorkshop, //no workshop stories
            Local, //only official stories
            Own //only custom stories
        }
        public static void Alert(string text)
        {
            string size = "Small";
            if (text.Length > 150) size = "Medium";
            if (text.Length > 400) size = "Large";
            GameObject alert = Instantiate(Resources.Load($"Prefabs/Alert{size}", typeof(GameObject)), Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas").GetComponent<Transform>()) as GameObject;
            alert.transform.Find("AlertText").GetComponent<Text>().text = text;
            alert.transform.localPosition = Vector3.zero;
        }

        private static bool _filledlist = false;
        private void Start()
        {
            if (!_filledlist)
            {
                _filledlist = true;
                genres = new string[] { Translate.Get("action"), 
                    Translate.Get("adventure"), 
                    Translate.Get("comedy"), 
                    Translate.Get("drama"), 
                    Translate.Get("fantasy"), 
                    Translate.Get("horror"), 
                    Translate.Get("magic"), 
                    Translate.Get("mystery"), 
                    Translate.Get("romance"), 
                    Translate.Get("scifi"), 
                    Translate.Get("sliceoflife"), 
                    Translate.Get("supernatural"), 
                    Translate.Get("other") };
                customPath = Application.dataPath + "/MyStories/";
                localPath = Application.dataPath + "/Story/";
                if(loggedin)
                steamPath = SteamApps.AppInstallDir().Replace(@"common\SajberSim", $@"workshop\content\{AppID}\");
                UnityEngine.Debug.Log($"Loaded all static data. Found {genres.Length} genres: {string.Join(", ", genres)}");
            }
            Directory.CreateDirectory($"{Application.dataPath}/Story"); //to avoid errors when booting after build
            Directory.CreateDirectory($"{Application.dataPath}/MyStories");
            AudioListener.volume = PlayerPrefs.GetFloat("volume", 1f); //sets volume to player value
        }
        /// <summary>
        /// Checks if input is an int or not
        /// </summary>
        public static bool IsNum(string input)
        {
            if (int.TryParse(input, out int n)) return true;
            else return false;
        }
       
        //public static string GetManifestPath(string storyID, StorySearchPaths where)
        //{
        //    string dir;
        //    switch (where)
        //    {
        //        case StorySearchPaths.Local:
        //            dir = localPath;
        //            break;
        //        case StorySearchPaths.Workshop:
        //            if (loggedin) dir = steamPath;
        //            else dir = localPath;
        //            break;
        //        case StorySearchPaths.Own:
        //            dir = customPath;
        //            break;
        //        default:
        //            dir = localPath;
        //            break;
        //    }
        //    string path = $"{dir}/{storyID}";
        //    if (File.Exists(path))
        //        return path;
        //    else
        //    {
        //        UnityEngine.Debug.LogError($"Helper: Tried getting manifest path {storyID} which does not exist ({path}, search argument {where.ToString()})");
        //        return null;
        //    }
        //}

        public static string[] ReverseArray(string[] arr)
        {
            Array.Reverse(arr);
            return arr;
        }
        public static List<string> ReverseList(List<string> list)
        {
            list.Reverse();
            return list;
        }
        public static void CreateLogfile()
        {
            if (Application.isEditor) return;
            DateTime now = DateTime.Now;
            string sourceFile = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/../LocalLow/LiterallyFabian/SajberSim/Player.log".Replace("/", "\\");
            string destFile = $@"{Application.dataPath}/Logs/SajberSim {now:yyyy.MM.dd - HH.mm.ss}.txt".Replace("/", "\\");
            System.IO.Directory.CreateDirectory($@"{Application.dataPath}\Logs");
            System.IO.File.Copy(sourceFile, destFile, true);
        }
        /// <summary>
        /// Appends argument on game directory (*/SajberSim_Data/)
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFolderFromGame(string path)
        {
            string fullpath = $@"{Application.dataPath}/{path}";
            if (Directory.Exists(fullpath))
                Process.Start(fullpath);
            else
                UnityEngine.Debug.LogError($"Helper/Folder: Tried to open path {fullpath} which does not exist");
        }

        /// <param name="dateTime">Date in the past to count from</param>
        /// <returns>Simplified string, eg "about 30 days ago", "yesterday"</returns>
        public static string TimeAgo(DateTime dateTime)
        {
            string result = string.Empty;
            TimeSpan timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format(Translate.Get("abouta"), timeSpan.Seconds, Translate.Get("seconds"));
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    string.Format(Translate.Get("about"), timeSpan.Minutes, Translate.Get("minutes")) : string.Format(Translate.Get("abouta"), Translate.Get("minute"));
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    string.Format(Translate.Get("about"), timeSpan.Hours, Translate.Get("hours")) : string.Format(Translate.Get("abouta"), Translate.Get("hour"));
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    string.Format(Translate.Get("about"), timeSpan.Days, Translate.Get("days")) : Translate.Get("yesterday");
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    string.Format(Translate.Get("about"), timeSpan.Days / 30, Translate.Get("months")) : string.Format(Translate.Get("abouta"), Translate.Get("month"));
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    string.Format(Translate.Get("about"), timeSpan.Days / 365, Translate.Get("years")) : string.Format(Translate.Get("abouta"), Translate.Get("year"));
            }

            return result;
        }
        /// <summary>
        /// Converts an int to a bool
        /// </summary>
        public static bool ToBool(int n)
        {
            if (n == 1) return true;
            else return false;
        }
        /// <summary>
        /// Converts a bool to 0 or 1
        /// </summary>
        public static int ToNum(bool n)
        {
            return n ? 1 : 0;
        }
        /// <summary>
        /// Checks if input is a float value
        /// </summary>
        public static bool IsFloat(string n)
        {
            try
            {
                float x = (float)Convert.ToDouble(n, Language.Format);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool GetBoolFromPrefs(string key, int def)
        {
            if (PlayerPrefs.GetInt(key, def) == 1) return true;
            else return false;
        }
        public static void SetPrefsFromBool(string key, bool b)
        {
            if (b) PlayerPrefs.SetInt(key, 1);
            else PlayerPrefs.SetInt(key, 0);
        }
        /// <summary>
        /// Modifies the brightness of a color
        /// </summary>
        /// <param name="alphaChange">Amount to multiply the darkness with</param>
        public static Color ModifyColor(Color color, float alphaChange)
        {
            color = color * alphaChange;
            color.a = 1;
            return color;
        }
        /// <summary>
        /// Tries to zip a directory and put it in the origin folder (Folder -> Folder/Folder.zip)
        /// </summary>
        /// <param name="origin"></param>
        /// <returns>True for successful compressions, otherwise false</returns>
        //public static bool ZipDirectory(string origin)
        //{
        //    if (!Directory.Exists(origin))
        //    {
        //        UnityEngine.Debug.LogError($"Helper: Tried to zip directory {origin} which does not exist.");
        //        return false;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            string result = new DirectoryInfo(origin).Name + ".zip";
        //            ZipFile.CreateFromDirectory(origin, result);
        //            return true;
        //        }
        //        catch (Exception e)
        //        {
        //            UnityEngine.Debug.LogError($"Helper: Could not zip directory {origin}.\n{e}");
        //            return false;
        //        }
        //    }
        //}
        public byte[] ConvertZipToData(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }
        public static string UwUTranslator(string text)
        {
            if (PlayerPrefs.GetInt("uwu", 0) == 0) return text;
            else
            {
                text = text.Replace('l', 'w');
                text = text.Replace('r', 'w');
                text = text.Replace(" f", " f-f");
                text = text.Replace('L', 'W');
                text = text.Replace('R', 'W');
                text = text.Replace(" F", " F-F");
                if (UnityEngine.Random.Range(0, 10) == 0) text = text + " :3";
            }
            return text;
        }
    }
}
/// <summary>
/// Used to store path and corresponding search pattern for Helper.SortArrayBy.
/// </summary>
class StorySort
{
    public string thepath;
    public string argstring;
    public int argint;
    public long arglong;
    public StorySort(string path, string arg)
    {
        this.thepath = path;
        this.argstring = arg;
    }
    public StorySort(string path, int arg)
    {
        this.thepath = path;
        this.argint = arg;
    }
    // only giving a path will set argint to the latest modification date 
    public StorySort(string path)
    {
        thepath = path;
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        if (Directory.Exists(path))
        {
            TimeSpan diff = Directory.GetLastWriteTime(path).ToUniversalTime() - origin;
            this.argint = (int)(diff.TotalSeconds);
        }
        else
        {
            arglong = 0;
        }
    }
}
