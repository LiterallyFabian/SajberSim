using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SajberSim.Story;
using UnityEngine;
using UnityEngine.UI;

namespace SajberSim.Helper
{
    class Helper : MonoBehaviour
    {

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
        public string[] GetAllStoryAssetPaths(string folder)
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
            }
            return assetPaths.ToArray();
        }
        /// <summary>
        /// Returns paths to all story manifest files
        /// </summary>
        public string[] GetAllManifests(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true)
        {
            List<string> manifestPaths = new List<string>();
            foreach (string story in GetAllStoryPaths(args, nsfw))
            {
                if (!File.Exists($"{story}/manifest.json")) Debug.LogError($"Tried getting manifest for {story} which does not exist.");
                manifestPaths.Add($"{story}/manifest.json");
            }
            return manifestPaths.ToArray();
        }
        /// <summary>
        /// Returns paths to all story folders, eg app/Story/OpenHouse
        /// </summary>
        /// <param name="args">Search arguments</param>
        /// <param name="nsfw">Include NSFW</param>
        /// <returns></returns>
        public string[] GetAllStoryPaths(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true)
        {
            List<string> storyPaths = Directory.GetDirectories($"{Application.dataPath}/Story/").ToList();
            string[] fixedPaths;
            if (args == StorySearchArgs.ID) 
                fixedPaths = Directory.GetDirectories($"{Application.dataPath}/Story/");
            else 
                fixedPaths = SortArrayBy(storyPaths, args);

            if (!nsfw) 
                fixedPaths = RemoveNSFWFromCardPaths(fixedPaths.ToList<string>());

            return fixedPaths;
        }
        /// <summary>
        /// Takes array of story paths and sorts it by data in manifest
        /// </summary>
        private string[] SortArrayBy(List<string> storyPaths, StorySearchArgs args)
        {
            bool reverse = false;
            if (args == StorySearchArgs.ReverseAlphabetical || args == StorySearchArgs.Oldest || args == StorySearchArgs.ShortestFirst) reverse = true;
            List<StorySort> itemList = new List<StorySort>();

            //Add everything to a list
            foreach (string path in storyPaths)
            {
                Manifest storydata = JsonConvert.DeserializeObject<Manifest>(File.ReadAllText($"{path}/manifest.json"));
                if (args == StorySearchArgs.Alphabetical)
                    itemList.Add(new StorySort(path, storydata.name));
                else if (args == StorySearchArgs.LongestFirst)
                    itemList.Add(new StorySort(path, storydata.playtime));
                else if (args == StorySearchArgs.Author)
                    itemList.Add(new StorySort(path, storydata.author));
                else if (args == StorySearchArgs.Newest)
                    itemList.Add(new StorySort(path, storydata.publishdate));
            }

            //Start sorting
            if(args == StorySearchArgs.LongestFirst || args == StorySearchArgs.Newest) //playtime
                itemList.Sort((x, y) => y.argint.CompareTo(x.argint));
            if(args == StorySearchArgs.Alphabetical || args == StorySearchArgs.Author) //name || author
                itemList.Sort((x, y) => string.Compare(x.argstring, y.argstring));


            //Done, now add paths
            List<string> sortedList = new List<string>();
            foreach (StorySort story in itemList)
            {
                sortedList.Add(story.path);
            }
            if (reverse) sortedList.Reverse();
            return sortedList.ToArray();
            
        }
        private string[] RemoveNSFWFromCardPaths(List<string> storyPaths) // https://i.imgur.com/Dw1l9YI.png
        {
            foreach (string path in storyPaths.ToList())
            {
                Manifest storydata = JsonConvert.DeserializeObject<Manifest>(File.ReadAllText($"{path}/manifest.json"));
                if (storydata.nsfw) storyPaths.Remove(path);
            }
            return storyPaths.ToArray();
        }
         public string GetManifest(string storyID)
        {
            string path = $"{Application.dataPath}/Story/{storyID}";
            if (File.Exists(path))
                return path;
            else
            {
                Debug.LogError($"Tried getting manifest {storyID} which does not exist ({path})");
                return null;
            }    
        }
        /// <summary>
        /// Returns names of all story folders
        /// </summary>
        public string[] GetAllStoryNames()
        {
            List<string> nameList = new List<string>();
            foreach (string path in GetAllStoryPaths())
                nameList.Add(path.Replace($"{Application.dataPath}/Story/", ""));

            return nameList.ToArray();
        }
        /// <summary>
        /// Returns amount of pages needed for the preview card menu
        /// </summary>
        public int GetCardPages()
        {
            return (GetAllManifests().Length - (GetAllManifests().Length % 6)) / 6;
        }
    }
    /// <summary>
    /// Used to store path and corresponding search pattern for Helper.SortArrayBy.
    /// </summary>
    class StorySort
    {
        public string path;
        public string argstring;
        public int argint;
        public StorySort(string path, string arg)
        {
            this.path = path;
            this.argstring = arg;
        }
        public StorySort(string path, int arg)
        {
            this.path = path;
            this.argint = arg;
        }
    }
}
