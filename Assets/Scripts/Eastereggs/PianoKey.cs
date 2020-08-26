using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PianoKey : MonoBehaviour
{
    public PianoMinigame Piano;
    public int ID;
    public float pitch;

    public void Press()
    {
        if (Piano.canPlay)
        {
            PlayAudio();
            Piano.SubmitKey(ID);
        }
    }
    public void PlayAudio()
    {
        GetComponent<AudioSource>().pitch = Mathf.Pow(2f, pitch / 12f);
        GetComponent<AudioSource>().Play();
    }
}

