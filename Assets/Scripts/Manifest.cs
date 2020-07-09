using Newtonsoft.Json;
using SajberSim.Colors;
using SajberSim.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Visual Novel manifest containing all metadata
/// </summary>
public class Manifest
{
#pragma warning disable CS0649
    public string name = "Default name";
    public string author = "Default author";
    public string authorid = "-1";
    public string description = "Default description";
    public string id = "0";
    public string language = "US";
    public string overlaycolor = "FFFFFF";
    public string textcolor = "323232";
    public bool nsfw = false;
    public int playtime = 0;
    public int publishdate = 19700101;
    public string[] tags = new string[0];
    public string genre = "other";
    public string rating = "everyone";

    public static Manifest Get(string path)
    {
        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogError($"Helper: Tried getting manifest for path \"{path}\" which does not exist");
            return null;
        }
        try
        {
            return JsonConvert.DeserializeObject<Manifest>(File.ReadAllText(path));
        }
        catch
        {
            UnityEngine.Debug.LogError($"Helper/GetManifest: Something went wrong when converting manifest \"{path}\". Is it setup correctly?");
            return null;
        }
    }
}
/// <summary>
/// Visual Novel design file
/// </summary>
public class StoryDesign
{
    public string textcolor = ColorUtility.ToHtmlStringRGB(Colors.DarkPurple); 
    public string questioncolor = ColorUtility.ToHtmlStringRGB(Colors.IngameBlue); 
    public string questiontextcolor = ColorUtility.ToHtmlStringRGB(Colors.UnityGray);
    public static StoryDesign Get()
    {
        string path = Helper.currentStoryPath + "/design.json";
        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogWarning($"{Helper.currentStoryPath} does not have a design manifest, continuing with default.");
            return new StoryDesign();
        }
        try
        {
            return JsonConvert.DeserializeObject<StoryDesign>(File.ReadAllText(path));
        }
        catch
        {
            UnityEngine.Debug.LogError($"Helper/GetDesign: Something went wrong when converting manifest \"{path}/design.json\". Is it setup correctly?");
            return null;
        }
    }
}

