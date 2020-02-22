using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DevCtrl : MonoBehaviour
{
    public GameObject background;
    public GameObject music;
    public void ChangeStory(string input)
    {
        GameManager.story = File.ReadAllLines($"{Application.dataPath}/Dialogues/{input}.txt");
        GameManager.dialogdone = false;
        GameManager.ready = true;
        GameManager.dialogpos = 0;
        GameManager.RemoveCharacters();
        StopSounds();
        PlayerPrefs.SetString("tempstory", input);
    }
    public void StopSounds()
    {
        background.GetComponent<AudioSource>().Stop();
        music.GetComponent<AudioSource>().Stop();
    }
}
