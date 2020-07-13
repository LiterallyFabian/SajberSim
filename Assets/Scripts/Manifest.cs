using Newtonsoft.Json;
using SajberSim.CardMenu;
using SajberSim.Colors;
using SajberSim.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SajberSim.Helper.Helper;

/// <summary>
/// Visual Novel manifest containing all metadata
/// </summary>
public class Manifest
{
#pragma warning disable CS0649
    //Metadata
    public string name = "Default name";
    public string description = "Default description";
    public string language = "US";
    public bool nsfw = false;
    public int playtime = 0;
    public int publishdate = 19700101;
    public string[] tags = new string[0];
    public string genre = "other";
    public string rating = "everyone";

    //IDs
    public string author = "Default author";
    public string authorid = "-1";
    public string id = "0";
    
    //Designs
    public string overlaycolor = "FFFFFF";
    public string textcolor = "323232";
    

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
    /// <summary>
    /// Returns paths to all story manifest files if they exist
    /// </summary>
    public static string[] GetAll(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
    {
        if (!loggedin && where != StorySearchPaths.Own) where = StorySearchPaths.NoWorkshop;
        List<string> manifestPaths = new List<string>();
        foreach (string story in Stories.GetAllStoryPaths(args, nsfw, searchTerm, where))
        {
            if (!File.Exists($"{story}/manifest.json"))
                UnityEngine.Debug.LogError($"Helper/GetAllManifests: Tried getting manifest for {story} which does not exist.");
            else if (Manifest.Get($"{story}/manifest.json") != null)
                manifestPaths.Add($"{story}/manifest.json");
        }
        return manifestPaths.ToArray();
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

