using Steamworks;
using System.Collections;
using System.Collections.Generic;
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
            novelspublished
        }
        public static void Add(List stat, int value = 1)
        {
            SteamUserStats.AddStat(stat.ToString(), value);
        }
        public static int GetInt(List stat)
        {
            return SteamUserStats.GetStatInt(stat.ToString());
        }
        public static float GetFloat(List stat)
        {
            return SteamUserStats.GetStatInt(stat.ToString());
        }
    }
}