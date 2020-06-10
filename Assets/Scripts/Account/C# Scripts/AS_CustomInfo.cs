/*
 * 		Account System - Custom Info
 * 
 * This class is used to store custom information for an account in a MySQL database.
 * 
 * You can use it to store anything from player preferences and progress,
 * to inventories, chat dialogues and any other custom classes, as long as
 * they are [Serializable].
 * 
 * The CustomInfo class is serialized and stored in XML format in the 'accounts'
 * table of your database, under the 'custominfo' field by AS_AccountManagement.UploadAccountInfoToDb
 * It is retrieved as a string and de-serialized by AS_AccountManagement.DownloadAccountInfoFromDb
 * 
 * All these are showcased in the demo scene. 
 * 
 */

using UnityEngine;
using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

public enum UserRoles { Admin, Moderator, Supporter, User, Banned }

/// <summary>
/// Use this class to easily store custom class instances to the database.
/// </summary>
[Serializable]
public class AS_CustomInfo
{
    public AS_CustomInfo() { }

    public UserRoles role = UserRoles.User;
    public string reddit = "";
    public string discord = "";
    public string github = "";
    public string twitter = "";
    public string website = "";
}

[Serializable]
public class AS_Socials
{

    // Required to be serializable
    public AS_Socials() { }

    public AS_Socials(string _key, string _value)
    { key = _key; value = _value; }

    public string key = "";
    public string value = "";


}

/*	
 * -------Alter any class you wish below here although note that-----------
 * 		altering these classes will cause the Demo script
 * 		named AS_AccountManagementGUI to stop working
 * 		(although you can probably edit it to suit your needs,
 *     	if you need an ingame editor for the Custom Info class)
 * 
 */
/*
[Serializable]
public class AS_SocialInfo
{
   public AS_SocialInfo() { }

   public bool show = false;
   public AS_SocialInfo(string _social, string _username)
   {
       social = _social;
       username = _username;
   }

   public string social = "";
   public string username = "";
}
*/
/*
public static class AS_CustomInfoMethods
{

    public static AS_CustomInfo CustomInfoOnGUI(this AS_CustomInfo customInfo)
    {

        if (customInfo == null)
            return customInfo;

        GUILayout.BeginVertical();

        // Title

        // LEVELS
        List<AS_LevelInfo> levels = customInfo.levels.ToList();

        GUILayout.Label("");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level Info", GUILayout.Width(100));


        if (GUILayout.Button("Add Level", GUILayout.Width(100)))
        {
            levels.Add(new AS_LevelInfo());
        }
        GUILayout.EndHorizontal();
        foreach (AS_LevelInfo level in levels)
        {

            GUILayout.BeginHorizontal();

            GUILayout.Label("Name: ", GUILayout.Width(50));
            level.name = GUILayout.TextField(level.name, GUILayout.MinWidth(100), GUILayout.MaxWidth(150));
            GUILayout.Label("", GUILayout.Width(25));
            GUILayout.Label("Score: ", GUILayout.Width(50));
            level.score = (int)GUILayout.HorizontalSlider(level.score, 0, 10, GUILayout.MinWidth(50), GUILayout.MaxWidth(75));
            try
            {
                level.score = Mathf.Clamp(
                                           Convert.ToInt32(
                                GUILayout.TextField(level.score.ToString(), GUILayout.MinWidth(50), GUILayout.MaxWidth(75))),
                                           0, 10);
            }
            catch { }

            if (GUILayout.Button("Remove", GUILayout.Width(75)))
            {

                levels.Remove(level);
                break;
            }

            GUILayout.EndHorizontal();

		}

        customInfo.levels = levels.ToArray();

        // ITEMS
        List<AS_StatInfo> stats = customInfo.stats.ToList();
        GUILayout.Label("");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Stat Info", GUILayout.Width(100));
        if (GUILayout.Button("Add Stat", GUILayout.Width(100)))
        {
            stats.Add(new AS_StatInfo());
        }
        GUILayout.EndHorizontal();
        foreach (AS_StatInfo stat in stats)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Name: ", GUILayout.Width(50));
            stat.name = GUILayout.TextField(stat.name, GUILayout.MinWidth(100), GUILayout.MaxWidth(150));
            GUILayout.Label("", GUILayout.Width(25));
            GUILayout.Label("Value: ", GUILayout.Width(50));
            stat.value = (int)GUILayout.HorizontalSlider(stat.value, 0, 100, GUILayout.MinWidth(50), GUILayout.MaxWidth(75));
            try
            {
                stat.value = Mathf.Clamp(
                                          Convert.ToInt32(GUILayout.TextField(stat.value.ToString(), GUILayout.MinWidth(50), GUILayout.MaxWidth(75))), 0, 100);
            }
            catch { }


            if (GUILayout.Button("Remove", GUILayout.Width(75)))
            {
                stats.Remove(stat);
                break;
            }

            GUILayout.EndHorizontal();


        }
        customInfo.stats = stats.ToArray();
        GUILayout.EndVertical();
        return customInfo;
    }
    */
