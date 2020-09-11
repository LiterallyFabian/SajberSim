using SajberSim.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class _CharacterHelper
{
    public string name;
    public int customCharacters;
    public string path;
    public bool success = false;
    public static _CharacterHelper TryGetNameFromLine(string line)
    {
        _CharacterHelper CC = new _CharacterHelper();
        ///Check character config & assign name
        CC.customCharacters = 0;
        string configPath = Path.Combine(Helper.currentStoryPath, "Characters", "characterconfig.txt");
        if (File.Exists(configPath)) CC.customCharacters = File.ReadAllLines(configPath).Length;
        if (Helper.IsNum(line))
            if (int.Parse(line) >= CC.customCharacters)
            {
                CC.name = "ERROR";
                CC.success = false;
                return CC;
            }
            else CC.name = GameManager.people[int.Parse(line)].name; //ID if possible, else name
        else CC.name = line;
        CC.success = true;
        return CC;
    }
    public static _CharacterHelper GetPath(string name, string mood)
    {
        _CharacterHelper CC = new _CharacterHelper();
        CC.name = name;
        CC.path = Path.Combine(Helper.currentStoryPath, "Characters", name + mood + ".png"); // root folder Characters/fabinahappy.png
        if (File.Exists(CC.path))
        {
            CC.success = true;
            return CC;
        }
        CC.path = Path.Combine(Helper.currentStoryPath, "Characters", name, mood + ".png"); // sub folder Characters/fabina/happy.png
        if (File.Exists(CC.path))
        {
            CC.success = true;
            return CC;
        }
        return CC; //no character found
    }
}