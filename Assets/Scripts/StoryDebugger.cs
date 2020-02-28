using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class StoryDebugger : MonoBehaviour
{
    public static void DebugStory()
    {
        using (StreamWriter write = new StreamWriter($@"{Application.dataPath}/debug.txt"))
        {
            string fixedpath = $@"{Application.dataPath}/Dialogues/".Replace("/", "\\");
            string audioPath = $@"{Application.dataPath}/Audio/".Replace("/", "\\");
            string[] storyPaths = Directory.GetFiles(fixedpath, "*.txt");
            string[] audioPaths = Directory.GetFiles(audioPath, "*.ogg");

            List<string> allmusic = new List<string>(); //list with all music
            List<string> allstories = new List<string>(); //list with all stories

            for (int i = 0; i < storyPaths.Length; i++)
                allstories.Add(storyPaths[i].Replace(fixedpath, "").Replace(".txt", ""));
            
            for (int i = 0; i < audioPaths.Length; i++)
                allmusic.Add(audioPaths[i].Replace(audioPath, "").Replace(".ogg", ""));

            foreach (string file in Directory.GetFiles(fixedpath, "*.txt"))
            {
                write.WriteLine($"{file} \n--------------------------------------\n");
                string[] story = File.ReadAllLines(file);

                for (int pos = 0; pos < story.Length; pos++)
                {
                    string[] line = story[pos].Split('|');

                    if (line[0] == "0") //textbox write.WriteLine($"Rad {pos + 1}: \"\n{story[pos]}\n");
                    {
                        if (!int.TryParse(line[1], out int i))
                        {
                            write.WriteLine($"Rad {pos + 1}: Person \"{line[1]}\" är inte en giltig person, kom ihåg att använda personens ID.\n{story[pos]}\n");
                        }
                        if (line.Length != 3)
                        {
                            write.WriteLine($"Rad {pos + 1}: Denna rad verkar vara för kort, förväntad längd: 3\n{story[pos]}\n");
                        }
                    }
                    else if (line[0] == "1") //new background
                    {
                        bool success = false;
                        string path = $@"{Application.dataPath}/Backgrounds/".Replace("/", "\\");
                        foreach (string img in Directory.EnumerateFiles(path, "*.png"))
                        {
                            if (img == $"{path}{line[1]}.png")
                            {
                                success = true;
                            }
                        }
                        if (!success) write.WriteLine($"Rad {pos + 1}: Bakgrunden \"{line[1]}\" verkar inte finnas.\n{story[pos]}\n");
                    }

                    else if (line[0] == "2") //move or create character
                    {
                        if (!double.TryParse(line[1], out double xd))
                            write.WriteLine($"Rad {pos + 1}: Person \"{line[1]}\" är inte en giltig person, kom ihåg att använda personens ID.\n{story[pos]}\n");
                        if (!double.TryParse(line[3], out double j))
                            write.WriteLine($"Rad {pos + 1}: {line[3]} är inte en giltig koordinat\n{story[pos]}\n");
                        if (!double.TryParse(line[4], out double g))
                            write.WriteLine($"Rad {pos + 1}: {line[4]} är inte en giltig koordinat\n{story[pos]}\n");
                    }
                    else if (line[0] == "3") //question
                    {
                        if (line.Length != 6)
                            write.WriteLine($"Rad {pos+1}: Denna rad verkar vara för kort, förväntad längd: 3\n{story[pos]}\n");
                        if (!allstories.Contains(line[3]))
                            write.WriteLine($"Rad {pos + 1}: Alternativet \"{line[3]}\" har ingen story\n{story[pos]}\n");
                        if (!allstories.Contains(line[5]))
                            write.WriteLine($"Rad {pos + 1}: Alternativet \"{line[5]}\" har ingen story\n{story[pos]}\n");

                    }
                    else if (line[0] == "4") //open new story (no question)
                    {
                        if (!allstories.Contains(line[1]))
                            write.WriteLine($"Rad {pos + 1}: Storyn \"{line[3]}\" existerar inte\n{story[pos]}\n");
                    }
                    else if (line[0] == "5") //general box
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
                    else if (!story[pos].StartsWith("//") && story[pos] != "")
                        write.WriteLine($"Rad {pos + 1}: Denna rad verkar ogiltig. Du kan kommentera genom att börja en rad med //\n{story[pos]}\n");
                }
            }
        }
        Process.Start($@"{Application.dataPath}/debug.txt");
    }
}
