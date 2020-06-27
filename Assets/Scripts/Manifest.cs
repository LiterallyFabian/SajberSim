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
    public string name;
    public string author;
    public string description;
    public string id;
    public string language;
    public string overlaycolor;
    public string textcolor;
    public bool nsfw;
    public int playtime;
    public int publishdate;
    public string[] tags;
    public string genre;
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
