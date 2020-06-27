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
        public static string[] genres;
        // genres as how they are in the steamworks backend
        public static string[] genresid = new string[] { "Adventure", "Action", "Comedy", "Drama", "Fantasy", "Horror", "Magic", "Mystery", "Romance", "Sci-fi", "Slice of life", "Supernatural", "Other" };
        public static uint AppID = 1353530;
        public static bool loggedin = false;
        public static int id = -1;
        public static string localPath = "";
        public static string customPath = "";
        public static string steamPath = "";
        public static string currentStoryPath = "";

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
            GameObject alert = Instantiate(Resources.Load("Prefabs/Alert", typeof(GameObject)), Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas").GetComponent<Transform>()) as GameObject;
            alert.transform.Find("title").GetComponent<Text>().text = text;
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
        /// <summary>
        /// Returns paths to all assets of specified type collected from all stories
        /// </summary>
        /// <param name="folder">Foldertype, eg. Characters</param>
        /// <returns>Array with all specified assets</returns>
        public static string[] GetAllStoryAssetPaths(string folder)
        {
            string[] validPaths = { "audio", "backgrounds", "characters", "dialogues" };
            List<string> assetPaths = new List<string>();
            folder = folder.ToLower();

            if (!validPaths.Contains(folder)) return new string[0];

            string extension = "*.png";

            switch (folder)
            {
                case "audio":
                    extension = "*.ogg";
                    break;
                case "dialogues":
                    extension = "*.txt";
                    break;
            }
            foreach (string story in GetAllStoryPaths())
            {
                string path = $"{story}/{Char.ToUpper(folder[0]) + folder.Remove(0, 1)}";
                if (Directory.Exists(path))
                    assetPaths.AddRange(Directory.GetFiles(path, extension));
            }
            return assetPaths.ToArray();
        }
        /// <summary>
        /// Returns paths to all story manifest files
        /// </summary>
        public static string[] GetAllManifests(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!loggedin) where = StorySearchPaths.NoWorkshop;
            List<string> manifestPaths = new List<string>();
            foreach (string story in GetAllStoryPaths(args, nsfw, searchTerm, where))
            {
                if (!File.Exists($"{story}/manifest.json"))
                    UnityEngine.Debug.LogError($"Helper/GetAllManifests: Tried getting manifest for {story} which does not exist.");
                else
                    manifestPaths.Add($"{story}/manifest.json");
            }
            return manifestPaths.ToArray();
        }
        /// <summary>
        /// Returns paths to all story folders, eg app/Story/OpenHouse. Main method for most stuff here
        /// </summary>
        /// <param name="args">Search arguments</param>
        /// <param name="nsfw">Include NSFW</param>
        /// <returns>Array with paths to all local story folders</returns>
        public static string[] GetAllStoryPaths(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!loggedin) where = StorySearchPaths.NoWorkshop;

            List<string> storyPaths = new List<string>();
            //This is what I call "The tired" ~
            //update, apparently the 5 line method i had here before wasn't the problem. oh well goodnight
            if (where == StorySearchPaths.Workshop)
            {
                storyPaths = Directory.GetDirectories(steamPath).ToList();
            }
            else if (where == StorySearchPaths.Local)
            {
                storyPaths = Directory.GetDirectories(localPath).ToList();
            }
            else if (where == StorySearchPaths.Own)
            {
                storyPaths = Directory.GetDirectories(customPath).ToList();
            }
            else if (where == StorySearchPaths.NoWorkshop)
            {
                storyPaths.AddRange(Directory.GetDirectories(customPath).ToList());
                storyPaths.AddRange(Directory.GetDirectories(localPath).ToList());
            }
            else if (where == StorySearchPaths.All)
            {
                storyPaths.AddRange(Directory.GetDirectories(customPath).ToList());
                storyPaths.AddRange(Directory.GetDirectories(localPath).ToList());
                storyPaths.AddRange(Directory.GetDirectories(steamPath).ToList());
            }

                //Fix the list
                string[] fixedPaths = SortArrayBy(storyPaths, args);

            if (!nsfw) //remove nsfw if needed
                fixedPaths = FilterNSFWFromCardPaths(fixedPaths.ToList());
            if (searchTerm != "")
                fixedPaths = FilterSearchFromCardPaths(fixedPaths.ToList(), searchTerm);
            return fixedPaths;
        }
        /// <summary>
        /// Takes array of story paths and sorts it by data in manifest
        /// </summary>
        private static string[] SortArrayBy(List<string> storyPaths, StorySearchArgs args)
        {
            if (args == StorySearchArgs.ID) return storyPaths.ToArray();

            bool reverse = false;
            if (args == StorySearchArgs.ReverseAlphabetical || args == StorySearchArgs.Newest || args == StorySearchArgs.LongestFirst) reverse = true;
            List<StorySort> itemList = new List<StorySort>();

            //Add everything to a list
            foreach (string path in storyPaths)
            {
                if (File.Exists($"{path}/manifest.json"))
                {
                    Manifest storydata = GetManifest($"{path}/manifest.json");
                    if (args == StorySearchArgs.Alphabetical || args == StorySearchArgs.ReverseAlphabetical)
                        itemList.Add(new StorySort(path, storydata.name));
                    else if (args == StorySearchArgs.LongestFirst || args == StorySearchArgs.ShortestFirst)
                        itemList.Add(new StorySort(path, storydata.playtime));
                    else if (args == StorySearchArgs.Author)
                        itemList.Add(new StorySort(path, storydata.author));
                    else if (args == StorySearchArgs.Newest || args == StorySearchArgs.Oldest)
                        itemList.Add(new StorySort(path, storydata.publishdate));
                }
                else
                {
                    UnityEngine.Debug.LogError($"{path} does not have a manifest");
                }
            }

            //Start sorting
            if (args == StorySearchArgs.LongestFirst || args == StorySearchArgs.Newest || args == StorySearchArgs.Oldest || args == StorySearchArgs.ShortestFirst) //playtime
                itemList = itemList.OrderBy(c => c.argint).ToList();
            if (args == StorySearchArgs.Alphabetical || args == StorySearchArgs.Author || args == StorySearchArgs.ReverseAlphabetical) //name || author
                itemList = itemList.OrderBy(c => c.argstring).ToList();

            //Done, now add paths
            List<string> sortedList = new List<string>();
            foreach (StorySort story in itemList)
            {
                sortedList.Add(story.thepath);
            }
            if (reverse) return ReverseArray(sortedList.ToArray());
            else return sortedList.ToArray();

        }
        private static string[] FilterNSFWFromCardPaths(List<string> storyPaths, bool remove = true) // https://i.imgur.com/Dw1l9YI.png
        {
            foreach (string path in storyPaths.ToList())
            {
                Manifest storydata = GetManifest($"{path}/manifest.json");
                if (storydata.nsfw && remove) storyPaths.Remove(path);
                else if (!storydata.nsfw && !remove) storyPaths.Remove(path);
            }
            return storyPaths.ToArray();
        }
        private static string[] FilterSearchFromCardPaths(List<string> storyPaths, string searchTerm)
        {
            searchTerm.ToLower();
            if (searchTerm == "nsfw") return FilterNSFWFromCardPaths(storyPaths, false);
            foreach (string path in storyPaths.ToList())
            {
                Manifest storydata = GetManifest($"{path}/manifest.json");

                if (!storydata.name.ToLower().Contains(searchTerm) && !storydata.tags.Contains(searchTerm) && !storydata.description.ToLower().Contains(searchTerm) && !storydata.author.ToLower().Contains(searchTerm) && !storydata.genre.ToLower().Contains(searchTerm))
                    storyPaths.Remove(path);
            }
            return storyPaths.ToArray();
        }
        /// <summary>
        /// Removes all story paths where the logged in owner not is the author
        /// </summary>
        public static string[] FilterNonOwnedFromCardPaths(List<string> storyPaths)
        {
            foreach(string path in storyPaths)
            {
                Manifest storydata = GetManifest($"{path}/manifest.json");
                if (storydata.author != SteamClient.Name) storyPaths.Remove(path);
            }
            return storyPaths.ToArray();
        }
        public static string GetManifestPath(string storyID, StorySearchPaths where)
        {
            string dir;
            switch (where)
            {
                case StorySearchPaths.Local:
                    dir = localPath;
                    break;
                case StorySearchPaths.Workshop:
                    if (loggedin) dir = steamPath;
                    else dir = localPath;
                    break;
                case StorySearchPaths.Own:
                    dir = customPath;
                    break;
                default:
                    dir = localPath;
                    break;
            }
            string path = $"{dir}/{storyID}";
            if (File.Exists(path))
                return path;
            else
            {
                UnityEngine.Debug.LogError($"Helper: Tried getting manifest path {storyID} which does not exist ({path}, search argument {where.ToString()})");
                return null;
            }
        }
        /// <summary>
        /// Returns names of all story folders
        /// </summary>
        public static string[] GetAllStoryNames(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!loggedin) where = StorySearchPaths.NoWorkshop;
            List<string> nameList = new List<string>();
            foreach (string path in GetAllStoryPaths(args, nsfw, searchTerm, where))
                nameList.Add(path.Replace(steamPath, ""));

            return nameList.ToArray();
        }
        /// <summary>
        /// Returns amount of pages needed for the preview card menu, in the correct order
        /// </summary>
        public static int GetCardPages(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!loggedin) where = StorySearchPaths.NoWorkshop;
            int length = GetAllManifests(args, nsfw, searchTerm, where).Length;

            if (length <= 6) return 0;
            return (length - (length % 6)) / 6;
        }
        /// <summary>
        /// Returns amount of cards that should be on the last page
        /// </summary>
        public static int GetLeftoverCardAmount(bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!loggedin) where = StorySearchPaths.NoWorkshop;
            int n = GetAllManifests(StorySearchArgs.ID, nsfw, searchTerm, where).Length % 6;
            if (n == 0) return 6; //there shouldn't be 0 cards on the last page
            else return n;
        }
        public static Manifest GetManifest(string path)
        {
            if (!path.Contains(".json"))
            {
                UnityEngine.Debug.LogError($"Helper: Tried getting manifest for path \"{path}\" which does not exist");
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<Manifest>(File.ReadAllText(path));
            }
            catch
            {
                UnityEngine.Debug.LogError($"Helper/GetManifest: Something went wrong when converting manifest \"{path}\". Is it setup correctly?");
                return null;
            }
        }
        public static Manifest GetManifestFromName(string name, StorySearchPaths where = StorySearchPaths.Local)
        {
            if (!loggedin) where = StorySearchPaths.NoWorkshop;
            string path = localPath;
            if (where == StorySearchPaths.Workshop) path = steamPath;
            else if(where == StorySearchPaths.All || where == StorySearchPaths.NoWorkshop)
            {
                UnityEngine.Debug.LogError($"Tried getting manifest for story {name} but search arguments were incorrect ({where}).");
                return null;
            }
            return GetManifest($"{path}{name}/manifest.json");
        }
        /// <summary>
        /// Returns amount of cards in total
        /// </summary>
        public static int GetTotalCardAmount(bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            return GetAllManifests(StorySearchArgs.ID, nsfw, searchTerm, where).Length;
        }
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
            if (n) return 1;
            else return 0;
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
        //        catch(Exception e)
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
}
