using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SajberSim.Story
{
    class Story
    {
        public string name;
        public string path;
        public int length;
        public string firstAction;
        public int firstActionPos;
        public string[] alltext;
        enum Folders
        {
            Audio,
            Backgrounds,
            Characters,
            Dialogues
        }
        public Story(string filename)
        {
           name = filename;
           path = $"{Application.dataPath}/Modding/Dialogues/{name}.txt";
            if (!File.Exists(path))
            {
                Debug.LogError($"Story/Constructor: Tried to create story \"{name}\" which does not exist. (PATH: {path})");
                return;
            }
            alltext = File.ReadAllLines(path);
            length = alltext.Length;
            int i = 0;
            foreach(string line in alltext)
            {
                if (GetActionFromText(line) != "COMMENT" && GetActionFromText(line) != "COMMENT")
                {
                    firstAction = GetActionFromText(line);
                    firstActionPos = i;
                    break;
                }
                i++;
            }
        }
        public string[] Line(int line)
        {
            string x = Application.dataPath;
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
