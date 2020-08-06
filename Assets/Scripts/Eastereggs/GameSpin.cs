using SajberSim.Helper;
using SajberSim.Steam;
using SajberSim.Translation;
using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GameSpin : MonoBehaviour
{
    public AudioSource music;
    public UnityEngine.UI.Image profilepic;
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
    private IEnumerator Run() //time: 14.4s
    {
        running = true;
        Achievements.Grant(Achievements.List.ACHIEVEMENT_menuspin);

        StartCoroutine(ChangeAudio(1, 0.5f, 5));
        StartCoroutine(ChangeCharacter(1, 3, 13f));
        GameObject.Find("Canvas/logo").GetComponent<BeatPulse>().size = 1.5f;
        GameObject.Find("Canvas/logo").GetComponent<BeatPulse>().BPM /= 2;
        GameObject.Find("Canvas/logo").GetComponent<Animator>().Play("logo away");
        ParticleSystem.EmissionModule emission = GameObject.Find("ParticleHolder/Blossom").GetComponent<ParticleSystem>().emission;
        ParticleSystem.ForceOverLifetimeModule force = GameObject.Find("ParticleHolder/Blossom").GetComponent<ParticleSystem>().forceOverLifetime;
        emission.rateOverTime = 30;
        force.x = -50;
        yield return new WaitForSeconds(1);
        GameObject.Find("Canvas/ButtonQuit").GetComponent<Button>().interactable = false;
        string help = string.Join(" ", Translate.Get("help").ToLower().ToCharArray());
        GameObject.Find("Canvas/ButtonPlay").GetComponent<Text>().text = help;
        profilepic.transform.localScale = new Vector3(1, -1, 1);
        yield return new WaitForSeconds(1);
        GameObject.Find("Canvas/ButtonLoad").GetComponent<Text>().text = help;
        string username = GameObject.Find("Canvas/Username").GetComponent<Text>().text;
        GameObject.Find("Canvas/Username").GetComponent<Text>().text = string.Format(Translate.Get("goodbyeuser"), Helper.UsernameCache());
        yield return new WaitForSeconds(1);
        GameObject.Find("Canvas/ButtonFind").GetComponent<Text>().text = help;
        yield return new WaitForSeconds(1);
        GameObject.Find("Canvas/ButtonSettings").GetComponent<Text>().text = help;
        yield return new WaitForSeconds(1);
        GameObject.Find("Canvas/ButtonQuit").GetComponent<Text>().text = "h e l p m e";
        yield return new WaitForSeconds(1);
        StartCoroutine(ChangeAudio(0.5f, 1, 8.4f));

        yield return new WaitForSeconds(7);

        StartCoroutine(ChangeCharacter(3, 1, 0.7f));
        GameObject.Find("Canvas/ButtonPlay").GetComponent<Text>().text = Translate.Get("play");
        GameObject.Find("Canvas/ButtonFind").GetComponent<Text>().text = Translate.Get("novels");
        GameObject.Find("Canvas/ButtonLoad").GetComponent<Text>().text = Translate.Get("continue");
        GameObject.Find("Canvas/ButtonSettings").GetComponent<Text>().text = Translate.Get("settings");
        yield return new WaitForSeconds(1.4f);
        profilepic.transform.localScale = Vector3.one;
        GameObject.Find("Canvas/Username").GetComponent<Text>().text = username;
        GameObject.Find("Canvas/ButtonQuit").GetComponent<Text>().text = Translate.Get("quit");
        GameObject.Find("Canvas/ButtonQuit").GetComponent<Button>().interactable = true;
        Camera.main.transform.rotation = Quaternion.identity;
        GameObject.Find("Canvas/logo").GetComponent<BeatPulse>().size = 1.055f;
        GameObject.Find("Canvas/logo").GetComponent<BeatPulse>().BPM *= 2;
        emission.rateOverTime = 7;
        force.x = 0;
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
