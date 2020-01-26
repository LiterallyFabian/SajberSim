using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonClass : MonoBehaviour
{

    public class Character
    {
        public string name;       //"Sam"
        public string cutenick;   //"Sammy"
        public string favthing;   //"phone"
        public string haircolor;  //brown
        public string favcolor;   //blue
        public int age;           //17



        public Sprite sad;  //"characters/samsad.png"
        public Sprite neutral;
        public Sprite happy;

        public Character(string name, string cutenick, string favthing, string haircolor, string favcolor, int age, Sprite sadpath, Sprite neutralpath, Sprite happypath)
        {
            this.name = name;
            this.cutenick = cutenick;
            this.favthing = favthing;
            this.haircolor = haircolor;
            this.favcolor = favcolor;
            this.age = age;

            this.sad = sadpath;
            this.neutral = neutralpath;
            this.happy = happypath;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
