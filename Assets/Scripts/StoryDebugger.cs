using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StoryDebugger : MonoBehaviour
{
    void Start()
    {
        using (StreamWriter write = new StreamWriter($@"{Application.dataPath}/debug.txt"))
        {
            string fixedpath = $@"{Application.dataPath}/Dialogues/".Replace("/", "\\");
            foreach (string file in Directory.GetFiles(fixedpath, "*.txt"))
            {
                write.WriteLine($"{file} \n--------------------------------------\n");
                string[] story = File.ReadAllLines(file);


                for (int pos = 0; pos < story.Length; pos++)
                {
                    string[] line = story[pos].Split('|');

                    if (line[0] == "0") //textbox write.WriteLine($"Rad {pos}: \"\n{story[pos]}\n");
                    {
                        if (!int.TryParse(line[1], out int i))
                        {
                            write.WriteLine($"Rad {pos}: Person \"{line[1]}\" är inte en gilltig person, kom ihåg att använda personens ID.\n{story[pos]}\n");
                        }
                        if(line.Length != 3)
                        {
                            write.WriteLine($"Rad {pos}: Denna rad verkar vara för kort, förväntad längd: 3\n{story[pos]}\n");
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
                        if(!success) write.WriteLine($"Rad {pos}: Bakgrunden \"{line[1]}\" verkar inte finnas.\n{story[pos]}\n");
                    }

                    else if (line[0] == "2") //move or create character
                    {
                        if(!int.TryParse(line[1], out int i))
                            write.WriteLine($"Rad {pos}: Person \"{line[1]}\" är inte en gilltig person, kom ihåg att använda personens ID.\n{story[pos]}\n");
                        if (!int.TryParse(line[3], out int j))
                            write.WriteLine($"Rad {pos}: {line[3]} är inte en gilltig koordinat\n{story[pos]}\n");
                        if (!int.TryParse(line[4], out int g))
                            write.WriteLine($"Rad {pos}: {line[4]} är inte en gilltig koordinat\n{story[pos]}\n");
                    }
                    else if (line[0] == "3") //question
                    {
                        ready = false;
                        string quest = line[1];
                        string alt1 = line[2];
                        story1 = line[3];
                        string alt2 = line[4];
                        story2 = line[5];
                        Question(quest, alt1, alt2);
                    }
                    else if (line[0] == "4") //open new story (no question)
                    {
                        ToggleTextbox(false, 3);
                        story = LoadStory(line[1]);
                        pos = 0; //återställ positionen - ny story!
                        if (line.Length > 2)
                            RemoveCharacters();
                    }
                    else if (line[0] == "5") //general box
                    {
                        string text = line[1].Replace("#", ",");
                        Debug.Log($"Alert: {text}");
                        ready = false;
                        StartCoroutine(SpawnAlert(UwUTranslator(text)));
                    }
                    else if (line[0] == "WAIT") //delay
                    {
                        ToggleTextbox(false, 3);
                        ready = false;
                        StartCoroutine(Delay(float.Parse(line[1])));
                    }
                    else if (line[0] == "PLAYMUSIC")
                    {
                        ToggleTextbox(false, 3);
                        StartCoroutine(PlayMusic(line[1]));
                        pos++;
                    }
                    else if (line[0] == "STOPSOUNDS")
                    {
                        StopSounds();
                        pos++;
                    }
                    else if (line[0] == "PLAYSFX")
                    {
                        ToggleTextbox(false, 3);
                        StartCoroutine(PlaySoundEffect(line[1]));
                        pos++;
                    }
                }
            }
        }
    }
}
