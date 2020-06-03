using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SajberSim.Story;
using SajberSim.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace SajberSim.Helper
{
    class Helper : MonoBehaviour
    {
        public static string[] genres;

        /// <summary>
        /// Positions for all 6 icons in the story menu
        /// </summary>
        public static Dictionary<int, Vector3> CardPositions = new Dictionary<int, Vector3>();
        private static bool _filledlist = false;
        private void Start()
        {
            if (!_filledlist)
            {
                _filledlist = true;
                CardPositions.Add(0, new Vector3(0, 0, 1));
                CardPositions.Add(1, new Vector3(330, 0, 1));
                CardPositions.Add(2, new Vector3(660, 0, 1));
                CardPositions.Add(3, new Vector3(0, -230, 1));
                CardPositions.Add(4, new Vector3(330, -230, 1));
                CardPositions.Add(5, new Vector3(660, -230, 1));
                genres = new string[] { Translate.Get("action"), Translate.Get("adventure"), Translate.Get("comedy"), Translate.Get("drama"), Translate.Get("fantasy"), Translate.Get("horror"), Translate.Get("magic"), Translate.Get("mystery"), Translate.Get("scifi"), Translate.Get("sliceoflife"), Translate.Get("supernatural"), Translate.Get("other") };
                UnityEngine.Debug.Log($"Loaded all static data. Found {genres.Length} genres: {string.Join(", ", genres)}");
            }
            
        }
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
            foreach(string story in Directory.GetDirectories($"{Application.dataPath}/Story/"))
            {
                string path = $"{story}/{Char.ToUpper(folder[0]) + folder.Remove(0, 1)}";
                if (Directory.Exists(path))
                    assetPaths.AddRange(Directory.GetFiles(path, extension));
                //else Debug.LogError($"Tried getting files matching {extension} in folder {path}, but none were found");
            }
            return assetPaths.ToArray();
        }
        /// <summary>
        /// Returns paths to all story manifest files
        /// </summary>
        public static string[] GetAllManifests(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "")
        {
            List<string> manifestPaths = new List<string>();
            foreach (string story in GetAllStoryPaths(args, nsfw, searchTerm))
            {
                if (!File.Exists($"{story}/manifest.json"))
                    UnityEngine.Debug.LogError($"Tried getting manifest for {story} which does not exist.");
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
        public static string[] GetAllStoryPaths(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "")
        {
            List<string> storyPaths = Directory.GetDirectories($"{Application.dataPath}/Story/").ToList();
            string[] fixedPaths;
            if (args == StorySearchArgs.ID)
                fixedPaths = Directory.GetDirectories($"{Application.dataPath}/Story/");
            else
                fixedPaths = SortArrayBy(storyPaths, args);
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
            bool reverse = false;
            if (args == StorySearchArgs.ReverseAlphabetical || args == StorySearchArgs.Newest || args == StorySearchArgs.LongestFirst) reverse = true;
            List<StorySort> itemList = new List<StorySort>();

            //Add everything to a list
            foreach (string path in storyPaths)
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

            //Start sorting
            if(args == StorySearchArgs.LongestFirst || args == StorySearchArgs.Newest || args == StorySearchArgs.Oldest || args == StorySearchArgs.ShortestFirst) //playtime
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
         public static string GetManifestPath(string storyID)
        {
            string path = $"{Application.dataPath}/Story/{storyID}";
            if (File.Exists(path))
                return path;
            else
            {
                UnityEngine.Debug.LogError($"Tried getting manifest path {storyID} which does not exist ({path})");
                return null;
            }    
        }
        /// <summary>
        /// Returns names of all story folders
        /// </summary>
        public static string[] GetAllStoryNames(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "")
        {
            List<string> nameList = new List<string>();
            foreach (string path in GetAllStoryPaths(args, nsfw, searchTerm))
                nameList.Add(path.Replace($"{Application.dataPath}/Story/", ""));

            return nameList.ToArray();
        }
        /// <summary>
        /// Returns amount of pages needed for the preview card menu, in the correct order
        /// </summary>
        public static int GetCardPages(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "")
        {
            int length = GetAllManifests(args, nsfw, searchTerm).Length;

            if (length <= 6) return 0;
            return (length - (length % 6)) / 6;
        }
        /// <summary>
        /// Returns amount of cards that should be on the last page
        /// </summary>
        public static int GetLeftoverCardAmount(bool nsfw = true, string searchTerm = "")
        {
            int n = GetAllManifests(StorySearchArgs.ID, nsfw, searchTerm).Length % 6;
            if (n == 0) return 6; //there shouldn't be 0 cards on the last page
            else return n;
        }
        public static Manifest GetManifest(string path)
        {
            if (!path.Contains(".json"))
            {
                UnityEngine.Debug.LogError($"Tried getting manifest for path \"{path}\" which does not exist");
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<Manifest>(File.ReadAllText(path));
            }
            catch
            {
                UnityEngine.Debug.LogError($"Something went wrong when converting manifest \"{path}\". Is it setup correctly?");
                return null;
            }
        }
        /// <summary>
        /// Returns amount of cards in total
        /// </summary>
        public static int GetTotalCardAmount(bool nsfw = true, string searchTerm = "")
        {
            return GetAllManifests(StorySearchArgs.ID, nsfw, searchTerm).Length;
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
                Process.Start("explorer.exe", fullpath);
            else
                UnityEngine.Debug.LogError($"Tried to open path {fullpath} which does not exist");
        }

        /// <param name="dateTime">Date in the past to count from</param>
        /// <returns>Simplified string, eg "about 30 days ago", "yesterday"</returns>
        public static string TimeAgo(DateTime dateTime)
        {
            string result = string.Empty;
            TimeSpan timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = String.Format(Translate.Get("abouta"), timeSpan.Seconds, Translate.Get("seconds"));
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format(Translate.Get("about"), timeSpan.Minutes, Translate.Get("minutes")) : String.Format(Translate.Get("abouta"), Translate.Get("minute"));
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format(Translate.Get("about"), timeSpan.Hours, Translate.Get("hours")) : String.Format(Translate.Get("abouta"), Translate.Get("hour"));
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format(Translate.Get("about"), timeSpan.Days, Translate.Get("days")) : Translate.Get("yesterday");
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format(Translate.Get("about"), timeSpan.Days / 30, Translate.Get("months")) : String.Format(Translate.Get("abouta"), Translate.Get("month"));
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format(Translate.Get("about"), timeSpan.Days / 365, Translate.Get("years")) : String.Format(Translate.Get("abouta"), Translate.Get("year"));
            }

            return result;
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
}
