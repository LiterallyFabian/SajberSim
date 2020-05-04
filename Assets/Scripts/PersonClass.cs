using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SajberSim.Chararcter
{

    public class Character
    {
        public string name;       //"Sam"
        public string nick;   //"Sammy"
        public int ID;

        public Character(string name, string cutenick, int charID)
        {
            this.name = name;
            this.nick = cutenick;
            ID = charID; //unused?
        }
    }
}
