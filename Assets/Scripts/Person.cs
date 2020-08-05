using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Holds a character name and nick
/// </summary>
namespace SajberSim.Chararcter
{

    public class Person
    {
        public new string name;   //"Sam"
        public string nick;   //"Sammy"
        public int ID;        //i don't think this is used

        public Person(string name, string cutenick, int charID)
        {
            this.name = name;
            this.nick = cutenick;
            ID = charID;
        }
    }
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
