using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonClass : MonoBehaviour
{

    public class Character
    {
        public string name;       //"Sam"
        public string nick;   //"Sammy"


        public Character(string name, string cutenick)
        {
            this.name = name;
            this.nick = cutenick;
        }
    }
}
