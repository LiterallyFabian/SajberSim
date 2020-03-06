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
		discord = new Discord.Discord(684504383618285590, (System.UInt64)Discord.CreateFlags.Default);
		var activityManager = discord.GetActivityManager();
		var activity = new Discord.Activity
		{
			State = "i'm just testing stuff y u reading dis",
			Details = "Playing SajberSim",
			

	};
		activityManager.UpdateActivity(activity, (res) =>
		{
			if (res == Discord.Result.Ok)
			{
				Debug.LogError("Everything is fine!");
			}
		});
	}

	// Update is called once per frame
	void Update()
	{
		discord.RunCallbacks();
		var activity = new Discord.Activity
		{
			State = "i'm just testing stuff y u reading dis",



		};
	}



}