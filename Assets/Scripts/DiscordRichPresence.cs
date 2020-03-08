using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;



public class DiscordRichPresence : MonoBehaviour
{

    public Discord.Discord discord;
    public static DiscordRichPresence discordPresence = new DiscordRichPresence();
    void Start()
    {
        DateTimeOffset dto = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        string smallimg;
        string smallimgname;

        discord = new Discord.Discord(684504383618285590, (System.UInt64)Discord.CreateFlags.Default);
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            Details = "Playing SajberSim",
            Assets = new ActivityAssets
            {
                LargeImage = "main",
                //   SmallImage = smallimg,

            },
            Timestamps = new ActivityTimestamps
            {
                Start = dto.ToUnixTimeSeconds(),
            }
        };
        activityManager.UpdateActivity(activity, (res) =>
        {
            if (res == Discord.Result.Ok)
            {
                Debug.Log($"Discord connected.");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        discord.RunCallbacks();
    }



}