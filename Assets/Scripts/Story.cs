using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Story
    {
        public string name;
        public string path;
        public int length;
        public string firstAction;
        public string[] alltext;

        public Story(string filename)
        {
            name = filename;
            path = $"{Application.dataPath}/Modding/Dialogues/{filename}.txt";
            if (!File.Exists(path))
            {
                Debug.LogError($"Tried to create story \"{filename}\" which does not exist. (PATH: {path})");
                return;
            }
            alltext = File.ReadAllLines($"{Application.dataPath}/Modding/Dialogues/{filename}.txt");
            length = alltext.Length;

            foreach(string line in alltext)
            {
                if (GetActionFromText(line) != "COMMENT" && GetActionFromText(line) != "COMMENT")
                {
                    firstAction = GetActionFromText(line);
                    break;
                }   
            }
        }
        public string[] GetLine(int line)
        {
            return alltext[line].Split('|');
        }
        public string GetActionFromLine(int line)
        {
            return GetActionFromText(alltext[line]);
        }
        public string GetActionFromText(string line)
        {
            if (line.StartsWith("//")) return "COMMENT";
            if (line == "") return "BLANK";
            else return line.Split('|')[0];
        }

    }
}
