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
    public bool success;
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
}