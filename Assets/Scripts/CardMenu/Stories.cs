using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static SajberSim.Helper.Helper;

namespace SajberSim.CardMenu
{
    public class Stories
    {
        private static string[] storyPaths;

        //if the path list above needs to be updated or if it is up-to-date already
        public static bool pathUpdateNeeded = true;

        /// <summary>
        /// Returns paths to all story folders, eg app/Story/OpenHouse. Main method for most stuff here
        /// </summary>
        /// <param name="args">Search arguments</param>
        /// <param name="nsfw">Include NSFW</param>
        /// <param name="searchTerm">If the stories should match a search term</param>
        /// <param name="where">Location of stories</param>
        /// <param name="forceUpdate">Wether it should ignore the cache</param>
        /// <returns>Array with paths to all local story folders</returns>Helper.customPath
        public static string[] GetAllStoryPaths(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All, bool forceUpdate = false)
        {
            if (!pathUpdateNeeded && !forceUpdate) return Stories.storyPaths;
            if (!loggedin && where != StorySearchPaths.Own) where = StorySearchPaths.NoWorkshop;
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
            else if (where == StorySearchPaths.NoWorkshop)
            {
                storyPaths.AddRange(Directory.GetDirectories(customPath).ToList());
                storyPaths.AddRange(Directory.GetDirectories(localPath).ToList());
            }
            else if (where == StorySearchPaths.All)
            {
                storyPaths.AddRange(Directory.GetDirectories(customPath).ToList());
                storyPaths.AddRange(Directory.GetDirectories(localPath).ToList());
                if (loggedin)
                    storyPaths.AddRange(Directory.GetDirectories(steamPath).ToList());
            }
            else if (where == StorySearchPaths.Own)
            {
                storyPaths.AddRange(Directory.GetDirectories(customPath).ToList());
                storyPaths.AddRange(Directory.GetDirectories(localPath).ToList());
                if (loggedin)
                    storyPaths.AddRange(Directory.GetDirectories(steamPath).ToList());
            }

            //Sort the list
            string[] fixedPaths = SortArrayBy(storyPaths, args);

            if (!nsfw) //remove nsfw if needed
                fixedPaths = FilterNSFWFromCardPaths(fixedPaths.ToList());
            if (searchTerm != "")
                fixedPaths = FilterSearchFromCardPaths(fixedPaths.ToList(), searchTerm);
            if (where == StorySearchPaths.Own)
                fixedPaths = FilterNonOwnedFromCardPaths(fixedPaths.ToList());
            Stories.pathUpdateNeeded = false;
            Stories.storyPaths = fixedPaths;
            return fixedPaths;
        }

        /// <summary>
        /// Takes array of story paths and sorts it by data in manifest
        /// </summary>
        private static string[] SortArrayBy(List<string> storyPaths, StorySearchArgs args)
        {
            if (args == StorySearchArgs.ID) return storyPaths.ToArray();

            bool reverse = false;
            if (args == StorySearchArgs.ReverseAlphabetical || args == StorySearchArgs.Newest || args == StorySearchArgs.LongestFirst || args == StorySearchArgs.LastModified) reverse = true;
            List<StorySort> itemList = new List<StorySort>();

            //Add everything to a list
            foreach (string path in storyPaths)
            {
                if (File.Exists(Path.Combine(path, "manifest.json")))
                {
                    Manifest storydata = Manifest.Get(Path.Combine(path, "manifest.json"));
                    StoryStats storystats = StoryStats.Get(path);
                    if (storydata != null)
                    {
                        if (args == StorySearchArgs.Alphabetical || args == StorySearchArgs.ReverseAlphabetical)
                            itemList.Add(new StorySort(path, storydata.name));
                        else if (args == StorySearchArgs.LongestFirst || args == StorySearchArgs.ShortestFirst)
                            itemList.Add(new StorySort(path, storystats.words));
                        else if (args == StorySearchArgs.Author)
                            itemList.Add(new StorySort(path, storydata.author));
                        else if (args == StorySearchArgs.Newest || args == StorySearchArgs.Oldest)
                            itemList.Add(new StorySort(path, storydata.uploaddate.ToString("yyyymmdd")));
                        else if (args == StorySearchArgs.LastModified)
                            itemList.Add(new StorySort(path));
                    }
                }
            }

            //Start sorting
            if (args == StorySearchArgs.LongestFirst || args == StorySearchArgs.Newest || args == StorySearchArgs.Oldest || args == StorySearchArgs.ShortestFirst || args == StorySearchArgs.LastModified) //playtime
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
                Manifest storydata = Manifest.Get(Path.Combine(path, "manifest.json"));
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
                Manifest data = Manifest.Get(Path.Combine(path, "manifest.json"));
                string localizedGenre = genres[Array.IndexOf(genresid, data.genre)];
                string steamGenre = genresSteam[Array.IndexOf(genresid, data.genre)];

                // Remove story if it doesn't match any
                if (!data.name.ToLower().Contains(searchTerm) &&
                    !data.tags.Contains(searchTerm) &&
                    !data.description.ToLower().Contains(searchTerm) &&
                    !data.author.ToLower().Contains(searchTerm) &&
                    !data.genre.ToLower().Contains(searchTerm) &&
                    !localizedGenre.ToLower().Contains(searchTerm) &&
                    !steamGenre.ToLower().Contains(searchTerm))
                {
                    storyPaths.Remove(path);
                }
            }
            return storyPaths.ToArray();
        }

        /// <summary>
        /// Removes all story paths where the logged in owner not is the author
        /// </summary>
        public static string[] FilterNonOwnedFromCardPaths(List<string> storyPaths)
        {
            foreach (string path in storyPaths.ToList())
            {
                Manifest storydata = Manifest.Get(Path.Combine(path, "manifest.json"));
                if (storydata != null)
                {
                    if (storydata.author != SteamClient.Name && storydata.authorid != $"{SteamClient.SteamId}" && !path.Contains("MyStories")) storyPaths.Remove(path);
                }
                else
                    storyPaths.Remove(path);
            }
            return storyPaths.ToArray();
        }

        /// <summary>
        /// Returns names of all story folders
        /// </summary>
        public static string[] GetAllStoryNames(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!loggedin && where != StorySearchPaths.Own) where = StorySearchPaths.NoWorkshop;
            List<string> nameList = new List<string>();
            foreach (string path in GetAllStoryPaths(args, nsfw, searchTerm, where))
                nameList.Add(new DirectoryInfo(path).Name);

            return nameList.ToArray();
        }

        /// <summary>
        /// Returns amount of pages needed for the preview card menu
        /// </summary>
        public static int GetCardPages(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!loggedin && where != StorySearchPaths.Own) where = StorySearchPaths.NoWorkshop;
            int length = Manifest.GetAll(args, nsfw, searchTerm, where).Length;
            if (length <= 6) return 0;
            else if (length % 6 == 0) return length / 6 - 1;
            return (length - (length % 6)) / 6;
        }

        /// <summary>
        /// Returns amount of cards that should be on the last page (indexed at 0)
        /// </summary>
        public static int GetLeftoverCardAmount(bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!loggedin && where != StorySearchPaths.Own) where = StorySearchPaths.NoWorkshop;
            int n = Manifest.GetAll(StorySearchArgs.ID, nsfw, searchTerm, where).Length % 6;
            if (n == 0) return 6; //there shouldn't be 0 cards on the last page
            else return n;
        }

        /// <summary>
        /// Returns amount of cards in total
        /// </summary>
        public static int GetTotalCardAmount(bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            return Manifest.GetAll(StorySearchArgs.ID, nsfw, searchTerm, where).Length;
        }

        /// <summary>
        /// Returns paths to all assets of specified type collected from all stories
        /// </summary>
        /// <param name="folder">Foldertype, eg. Characters</param>
        /// <returns>Array with all specified assets</returns>
        public static string[] GetAllStoryAssetPaths(string folder)
        {
            string[] validPaths = { "audio", "backgrounds", "characters", "dialogues", "main", "ports" };
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

                case "main":
                    extension = "mainbg*.png";
                    break;

                case "ports":
                    extension = "*port.png";
                    break;
            }
            foreach (string story in GetAllStoryPaths())
            {
                if (folder == "main") folder = "backgrounds";
                if (folder == "ports") folder = "characters";
                string path = Path.Combine(story, folder.FirstCharToUpper());
                if (Directory.Exists(path))
                {
                    foreach (string subpath in Directory.GetDirectories(path))
                        assetPaths.AddRange(Directory.GetFiles(subpath, extension));
                    assetPaths.AddRange(Directory.GetFiles(path, extension));
                }
            }
            return assetPaths.ToArray();
        }

        public static string[] GetStoryAssetPaths(string folder, string path)
        {
            string[] validPaths = { "audio", "backgrounds", "characters", "dialogues", "ports" };
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

                case "ports":
                    extension = "*port.png";
                    break;
            }
            if (folder == "ports") folder = "characters";
            path = Path.Combine(path, folder.FirstCharToUpper());
            if (Directory.Exists(path))
                foreach (string subpath in Directory.GetDirectories(path))
                    assetPaths.AddRange(Directory.GetFiles(subpath, extension));
            if (Directory.Exists(path))
                assetPaths.AddRange(Directory.GetFiles(path, extension));
            return assetPaths.ToArray();
        }
    }

    /// <summary>
    /// Holds stats for a specific story
    /// </summary>
    public class StoryStats
    {
        public int words = 0;
        public string wordsK = "0k";
        public int decisions = 0;
        public int actions = 0;
        public int lines = 0;
        public int textboxes = 0;
        public int alerts = 0;
        public int scripts = 0;
        public int backgroundchanges = 0;
        public string filesize = "0 Mb";

        public int audioclips = 0;
        public int backgrounds = 0;
        public int charactersprites = 0;

        public bool hascredits = false;
        public int participants = 0;

        public static StoryStats Get(string path)
        {
            StoryStats stats = new StoryStats();
            string[] scriptPaths = Stories.GetStoryAssetPaths("dialogues", path);
            List<string> scriptLines = new List<string>();
            foreach (string scriptPath in scriptPaths)
            {
                if (File.Exists(scriptPath))
                {
                    scriptLines.AddRange(File.ReadAllLines(scriptPath));
                }
            }

            stats.scripts = scriptPaths.Length;
            stats.lines = scriptLines.Count();
            foreach (string line in scriptLines)
            {
                string action = line.Split('|')[0];
                switch (action)
                {
                    case "T":
                        stats.textboxes++;
                        if (line.Split('|').Length == 3)
                            stats.words += line.Split('|')[2].Count(f => f == ' ') + 1;
                        break;

                    case "BACKGROUND":
                        stats.backgroundchanges++;
                        break;

                    case "ALERT":
                        stats.alerts++;
                        if (line.Split('|').Length == 2)
                            stats.words += line.Split('|')[1].Count(f => f == ' ') + 1;
                        break;

                    case "QUESTION":
                        stats.decisions++;
                        if (line.Split('|').Length > 1)
                            stats.words += line.Split('|')[1].Count(f => f == ' ') + 1;
                        break;
                }
                if (line.Contains('|')) stats.actions++;
            }
            stats.filesize = $"{Math.Round(BytesTo(DirSize(new DirectoryInfo(path)), DataSize.Megabyte)),1} MB";
            stats.audioclips = Stories.GetStoryAssetPaths("audio", path).Length;
            stats.backgrounds = Stories.GetStoryAssetPaths("backgrounds", path).Length;
            stats.charactersprites = Stories.GetStoryAssetPaths("characters", path).Length;
            stats.wordsK = FormatNumber(stats.words);
            if (File.Exists(Path.Combine(path, "credits.txt")))
            {
                stats.hascredits = true;
                foreach (string line in File.ReadAllLines(Path.Combine(path, "credits.txt")))
                {
                    if (!line.Contains('|') && !line.StartsWith("-") && line != "") stats.participants++;
                }
            }
            return stats;
        }
    }
}