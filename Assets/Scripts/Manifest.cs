using Newtonsoft.Json;
using SajberSim.Colors;
using System;
using System.Collections.Generic;
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
}
/// <summary>
/// Visual Novel design file
/// </summary>
public class StoryDesign
{
    public string textcolor = ColorUtility.ToHtmlStringRGB(Colors.DarkPurple); 
    public string questioncolor = ColorUtility.ToHtmlStringRGB(Colors.IngameBlue); 
    public string questiontextcolor = ColorUtility.ToHtmlStringRGB(Colors.UnityGray);
}
