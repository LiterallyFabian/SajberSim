using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string[] GetAllManifests()
        {
            List<string> manifestPaths = new List<string>();
            foreach (string story in GetAllStoryPaths())
            {
                manifestPaths.AddRange(Directory.GetFiles($"{story}", "manifest.json"));
            }
            return manifestPaths.ToArray();
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
}
