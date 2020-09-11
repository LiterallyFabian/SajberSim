using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SajberSim.Chararcter
{
    /// <summary>
    /// Container for character name and nick loaded from character configs
    /// </summary>
    public class Person
    {
        public string name;
        public string nick;
        public int ID; //ID they will get assigned
        public static Person[] people = new Person[4];

        public Person(string name, string cutenick, int charID)
        {
            this.name = name;
            this.nick = cutenick;
            this.ID = charID;
        }
        /// <summary>
        /// Reads character config for currently running story and assigns everyone an ID
        /// </summary>
        /// <returns>List of characters.</returns>
        public static Person[] Assign()
        {
            //Fix path to config if existing
            System.Random rnd = new System.Random();
            string configPath = Path.Combine(Helper.Helper.currentStoryPath, "Characters", "characterconfig.txt");
            if (!File.Exists(configPath)) return new Person[0];
            string[] config = File.ReadAllLines(configPath);

            people = new Person[config.Length]; //change size to amount of ppl
            PlayerPrefs.SetInt("characters", config.Length); //amount of characters

            for (int i = 0; i < config.Length; i++) //fill array from file
                people[i] = new Person(config[i].Split(',')[0], config[i].Split(',')[1], i);

            //Randomize and return
            people = people.OrderBy(x => rnd.Next()).ToArray();
            return people;
        }
    }
    /// <summary>
    /// Save data for characters including full position and scale
    /// </summary>
    public class PersonSave
    {
        public string name;
        public string mood;
        public float x;
        public float y;
        public float size;
        public bool flipped;
    }
}
