using SajberSim.Translation;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace SajberSim.Steam
{
    public class Stats : MonoBehaviour
    {
        public enum List
        {
            novelsstarted,
            novelsfinished,
            novelscreated,
            novelspublished,
            decisionsmade
        }
        public static void Add(List stat, int value = 1)
        {
            if (!Helper.Helper.loggedin) return;
            try
            {
                SteamUserStats.AddStat(stat.ToString(), value);
            }
            catch(Exception e)
            {
                Debug.LogError($"Steam/Stats/Add: Could not modify stat {stat.ToString().ToUpper()}. Error:\n{e}");
            }
        }
        public static int GetInt(List stat)
        {
            if (!Helper.Helper.loggedin) return -1;
            return SteamUserStats.GetStatInt(stat.ToString());
        }
        public static float GetFloat(List stat)
        {
            if (!Helper.Helper.loggedin) return -1;
            return SteamUserStats.GetStatInt(stat.ToString());
        }
        public static void ShowAll()
        {
            if (!Helper.Helper.loggedin) return;
            foreach(List stat in (List[])Enum.GetValues(typeof(List)))
            {
                Debug.Log(string.Format(stat.Name(), GetInt(stat)));
            }
        }
    }
}