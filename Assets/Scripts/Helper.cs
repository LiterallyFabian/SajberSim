using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SajberSim.Story;
using UnityEngine;

namespace SajberSim.Helper
{
    class Helper : MonoBehaviour
    {

        /// <summary>
        /// Positions for all 6 icons in the story menu
        /// </summary>
        public static Dictionary<int, Vector2> storyPositions = new Dictionary<int, Vector2>();
        private void Start()
        {
            storyPositions.Add(0, new Vector2(0, 0));
            storyPositions.Add(1, new Vector2(330, 0));
            storyPositions.Add(2, new Vector2(660, 0));
            storyPositions.Add(3, new Vector2(0, -230));
            storyPositions.Add(4, new Vector2(330, -230));
            storyPositions.Add(5, new Vector2(660, -230));
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
                assetPaths.AddRange(Directory.GetFiles($"{story}/{Char.ToUpper(folder[0]) + folder.Remove(0, 1)}", extension));
            }
            return assetPaths.ToArray();
        }
        /// <summary>
        /// Returns paths to all story manifest files
        /// </summary>
        public string[] GetAllManifests()
        {
            return Directory.GetFiles($"{Application.dataPath}/Story/", "manifest.json");
        }
        /// <summary>
        /// Returns paths to all story folders, eg app/Story/OpenHouse
        /// </summary>
        public string[] GetAllStoryPaths()
        {
            return Directory.GetDirectories($"{Application.dataPath}/Story/");
        }
        /// <summary>
        /// Returns names of all story folders
        /// </summary>
        /// <returns></returns>
        public string[] GetAllStoryNames()
        {
            List<string> nameList = new List<string>();
            foreach (string path in GetAllStoryPaths())
                nameList.Add(path.Replace($"{Application.dataPath}/Story/", ""));

            return nameList.ToArray();
        }
        

    }
}
