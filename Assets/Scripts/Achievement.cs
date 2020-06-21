using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace SajberSim.Steam
{
    public class Achievements : MonoBehaviour
    {
        public enum List
        {
            ACHIEVEMENT_findblush,
            ACHIEVEMENT_play1,
            ACHIEVEMENT_finish1,
            ACHIEVEMENT_finish5,
            ACHIEVEMENT_finish15,
            ACHIEVEMENT_finish13,
            ACHIEVEMENT_finish100,
            ACHIEVEMENT_download,
            ACHIEVEMENT_create,
            ACHIEVEMENT_publish1,
            ACHIEVEMENT_publish10
        }
        public static void Grant(List achievement)
        {
            Achievement ach = new Achievement(achievement.ToString());
            ach.Trigger(true);
            Debug.Log($"{SteamClient.Name} achieved \"{ach.Name}\"! Congrats :3");
        }
    }
}
