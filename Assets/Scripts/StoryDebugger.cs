using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class StoryDebugger : MonoBehaviour
{
        public static void DebugStory()
    { 
        NumberFormatInfo lang = new NumberFormatInfo();
        lang.NumberDecimalSeparator = ".";
        using (StreamWriter write = new StreamWriter($@"{Application.dataPath}/Modding/debug.txt"))
        {
            string fixedpath = $@"{Application.dataPath}/Modding/Dialogues/".Replace("/", "\\");
            string audioPath = $@"{Application.dataPath}/Modding/Audio/".Replace("/", "\\");
            string charPath = $@"{Application.dataPath}/Modding/Characters/".Replace("/", "\\");
            string[] storyPaths = Directory.GetFiles(fixedpath, "*.txt");
            string[] audioPaths = Directory.GetFiles(audioPath, "*.ogg");
            string[] charPaths = Directory.GetFiles(charPath, "*neutral.png");

            List<string> allmusic = new List<string>(); //list with all music
            List<string> allstories = new List<string>(); //list with all stories
            List<string> allchars = new List<string>(); //list with all characters

            for (int i = 0; i < storyPaths.Length; i++)
                allstories.Add(storyPaths[i].Replace(fixedpath, "").Replace(".txt", ""));
            
            for (int i = 0; i < audioPaths.Length; i++)
                allmusic.Add(audioPaths[i].Replace(audioPath, "").Replace(".ogg", ""));

            for (int i = 0; i < charPaths.Length; i++)
                allchars.Add(charPaths[i].Replace(charPath, "").Replace("neutral.png", ""));

            foreach (string file in Directory.GetFiles(fixedpath, "*.txt")) //kollar igenom alla FILER
            {
                write.WriteLine($"{file} \n--------------------------------------");
                string[] story = File.ReadAllLines(file);
                if (!story[story.Length - 1].StartsWith("FINISHGAME") && !story[story.Length - 1].StartsWith("QUESTION") && !story[story.Length - 1].StartsWith("LOADSTORY")) write.WriteLine("Denna story leder ingenstans. Ifall det är spelets sista del avslutar du med FINSIHGAME, annars en fråga\n"); else write.WriteLine("\n");

                for (int pos = 0; pos < story.Length; pos++) //kollar igenom alla RADER
                {
                    string[] line = story[pos].Split('|');

                    if (line[0] == "T") //textbox write.WriteLine($"Rad {pos + 1}: \"\n{story[pos]}\n");
                    {
                        if (!int.TryParse(line[1], out int i))
                        {
                            if(!allchars.Contains(line[1].ToLower())) write.WriteLine($"Rad {pos + 1}: Karaktär {line[1]} finns ej\n{story[pos]}\n");
                        }
                        if (line.Length < 3) write.WriteLine($"Rad {pos + 1}: Denna rad verkar vara för kort, förväntad längd: 3\n{story[pos]}\n");
                        else if (line.Length > 3) write.WriteLine($"Rad {pos + 1}: Denna rad verkar vara för lång, förväntad längd: 3\n{story[pos]}\n");
                    }
                    else if (line[0] == "BG") //new background
                    {
                        bool success = false;
                        string path = $@"{Application.dataPath}/Modding/Backgrounds/".Replace("/", "\\");
                        foreach (string img in Directory.EnumerateFiles(path, "*.png"))
                        {
                            if (img == $"{path}{line[1]}.png")
                            {
                                success = true;
                            }
                        }
                        if (!success) write.WriteLine($"Rad {pos + 1}: Bakgrunden \"{line[1]}\" verkar inte finnas.\n{story[pos]}\n");
                    }

                    else if (line[0] == "CHAR") //move or create character
                    {
                        bool success = false;
                        if (allchars.Contains(line[1].ToLower())) success = true;
                        if (!double.TryParse(line[1], NumberStyles.Number, lang, out double xd) && !success)
                            write.WriteLine($"Rad {pos + 1}: Person \"{line[1]}\" verkar inte finnas\n{story[pos]}\n");
                        if (!double.TryParse(line[3], NumberStyles.Number, lang, out double j))
                            write.WriteLine($"Rad {pos + 1}: {line[3]} är inte en giltig koordinat\n{story[pos]}\n");
                        if (!double.TryParse(line[4], NumberStyles.Number, lang, out double g))
                            write.WriteLine($"Rad {pos + 1}: {line[4]} är inte en giltig koordinat\n{story[pos]}\n");
                    }
                    else if (line[0] == "QUESTION") //question
                    {
                        for (int i = 3; i < line.Length; i = i+2)
                        {
                            if(!allstories.Contains(line[i]))
                                write.WriteLine($"Rad {pos + 1}: Alternativet \"{line[i]}\" har ingen story\n{story[pos]}\n");
                        }
                            

                    }
                    else if (line[0] == "LOADSTORY") //open new story (no question)
                    {
                        if (!allstories.Contains(line[1]))
                            write.WriteLine($"Rad {pos + 1}: Storyn \"{line[3]}\" existerar inte\n{story[pos]}\n");
                    }
                    else if (line[0] == "ALERT") //general box
                    {
                        if (line.Length == 1)
                            write.WriteLine($"Rad {pos + 1}: Denna rad saknar text\n{story[pos]}\n");
                    }
                    else if (line[0] == "WAIT") //delay
                    {
                        if (line[1].Contains(","))
                            write.WriteLine($"Rad {pos + 1}: Du måste använda punkter för decimaler, inte kommatecken. (6.9 istället för 6,9 etc)\n{story[pos]}\n");
                    }
                    else if (line[0] == "PLAYMUSIC" || line[0] == "PLAYSFX")
                    {
                        if (!allmusic.Contains(line[1]))
                            write.WriteLine($"Rad {pos + 1}: Det verkar som ljudet \"{line[1]}\" inte existerar\n{story[pos]}\n");
                    }
                    else if(line[0] == "DEL")
                    {
                        if (!allchars.Contains(line[1].ToLower()) && !int.TryParse(line[1], out int i))
                            write.WriteLine($"Rad {pos + 1}: Karaktär {line[1]} finns ej\n{story[pos]}\n");
                    }
                    else if (!story[pos].StartsWith("//") && story[pos] != "" && !story[pos].StartsWith("OPENSCENE|") && !story[pos].StartsWith("FINISHGAME"))
                        write.WriteLine($"Rad {pos + 1}: Denna rad verkar ogiltig. Du kan kommentera genom att börja en rad med //\n{story[pos]}\n");
                }
            }
        }
        Process.Start($@"{Application.dataPath}/Modding/debug.txt");
    }
}
