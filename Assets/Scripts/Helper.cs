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
        /*
        /// <summary>
        /// Returns paths to all story folders, eg base/Story/OpenHouse
        /// </summary>
        public string[] GetAllStoryPaths()
        {

        }
        /// <summary>
        /// Returns names of all story folders
        /// </summary>
        /// <returns></returns>
        public string[] GetAllStoryNames()
        {

        }
        */
    }
}
