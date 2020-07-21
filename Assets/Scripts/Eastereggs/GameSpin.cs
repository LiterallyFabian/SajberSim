﻿using SajberSim.Steam;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpin : MonoBehaviour
{
    public AudioSource music;
    public GameObject character;
    private bool running = false;


    public void StartSpin()
    {
        if(!running) StartCoroutine(Run());
    }
    private void Update()
    {
        if (running) Camera.main.transform.Rotate(0, 0, Time.deltaTime * 25f);
    }
    private IEnumerator Run()
    {
        Achievements.Grant(Achievements.List.ACHIEVEMENT_menuspin);
        running = true;
        StartCoroutine(ChangeAudio(1, 0.7f, 5));
        StartCoroutine(ChangeCharacter(1, 3, 13f));
        GameObject.Find("Canvas/logo").GetComponent<BeatPulse>().size = 1.5f;
        GameObject.Find("Canvas/logo").GetComponent<BeatPulse>().BPM /= 2;
        yield return new WaitForSeconds(6);
        StartCoroutine(ChangeAudio(0.7f, 1, 9f));
        yield return new WaitForSeconds(7);
        StartCoroutine(ChangeCharacter(3, 1, 0.7f));
        yield return new WaitForSeconds(1.4f);
        Camera.main.transform.rotation = Quaternion.identity;
        GameObject.Find("Canvas/logo").GetComponent<BeatPulse>().size = 1.055f;
        GameObject.Find("Canvas/logo").GetComponent<BeatPulse>().BPM *= 2;
        Destroy(this);
    }
    public IEnumerator ChangeAudio(float oldValue, float newValue, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            music.pitch = Mathf.Lerp(oldValue, newValue, t / duration);
            yield return null;
        }
        music.pitch = newValue;
    }
    public IEnumerator ChangeCharacter(float oldValue, float newValue, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            character.transform.localScale = new Vector3(Mathf.Lerp(oldValue, newValue, t / duration), 1, 1);
            yield return null;
        }
        character.transform.localScale = new Vector3(newValue, 1, 1);
    }
}
