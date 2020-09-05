using SajberSim.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class _CharacterConfig
{
    public string name;
    public int customCharacters;
    public bool success;
    public static _CharacterConfig TryGetNameFromLine(string line)
    {
        _CharacterConfig CC = new _CharacterConfig();
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