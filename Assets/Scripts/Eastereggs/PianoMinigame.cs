using SajberSim.Steam;
using SajberSim.Translation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;

public class PianoMinigame : MonoBehaviour
{
    private bool usePremade = false;
    private int preMadeNo;
    public bool canPlay = true;
    private float delay = 0.8f;
    public GameObject ButtonRetry;
    public GameObject LossText;
    public List<int> sequence = new List<int>();
    public GameObject[] keys = new GameObject[9];
    private int currentPos = 0;
    public Text stats;

    public int[][] premadeSequences = new int[][]
    {
        new int[] {5,3,4,3,4,3,4,5,7,3,2,8,7,8,7,6,7,6}, // crossing field
        new int[] {1,1,8,5,4,4,3,1,3,4,0,0,8,5,4,4,3,1,3,4,0,0,7,5,4,4,3,1,3,4} // megalovania
    };


    private void Start()
    {
        GameObject.Find("Canvas/logo").GetComponent<AudioSource>().volume = 0.1f;
        SetStats();
        PlayGame();
    }

    public void Close()
    {
        GameObject.Find("Canvas/logo").GetComponent<AudioSource>().volume = 1;
        Destroy(this.gameObject);
    }

    public void SubmitKey(int id)
    {
        if (id - 1 != sequence[currentPos])
        {
            StartCoroutine(Fail());
        }
        else
        {
            currentPos++;
            SetStats();
        }
        if (sequence.Count == currentPos)
        {
            if (PlayerPrefs.GetInt("pianostreak", 0) < currentPos) PlayerPrefs.SetInt("pianostreak", currentPos);
            if (currentPos == 20) Achievements.Grant(Achievements.List.ACHIEVEMENT_20piano);
            StartCoroutine(RunSequence());
        }
    }

    public void PlayGame()
    {
        if(Random.Range(0, 100) > 70)
        {
            usePremade = true;
            preMadeNo = Random.Range(0, premadeSequences.Length);
        }
        else
        {
            usePremade = false;
        }
        StartCoroutine(RunSequence());
        LossText.GetComponent<Animator>().Play("New State");
        LossText.transform.localPosition = new Vector3(1000, 0, 0);
        foreach (GameObject key in keys)
        {
            key.GetComponent<Image>().color = Color.white;
        }
        sequence = new List<int>();
        delay = 0.8f;
    }

    private IEnumerator Fail()
    {
        LossText.GetComponent<Animator>().Play("lose text");
        foreach (GameObject key in keys)
        {
            key.GetComponent<AudioSource>().pitch -= 0.8f;
            key.GetComponent<Button>().interactable = false;
            key.GetComponent<Image>().color = Color.red;
        }
        yield return new WaitForSeconds(2);
        ButtonRetry.SetActive(true);
    }

    private IEnumerator RunSequence()
    {
        yield return new WaitForSeconds(0.2f);
        currentPos = 0;
        if (delay > 0.3f) delay -= 0.02f;
        if (!usePremade) sequence.Add(UnityEngine.Random.Range(0, 9));
        else
        {
            if (premadeSequences[preMadeNo].Length > sequence.Count) sequence.Add(premadeSequences[preMadeNo][sequence.Count]);
            else sequence.Add(UnityEngine.Random.Range(0, 9));
        }
        SetStats();
        ToggleKeys(false);
        yield return new WaitForSeconds(0.9f);
        foreach (int key in sequence)
        {
            keys[key].GetComponent<PianoKey>().PlayAudio();
            keys[key].GetComponent<Image>().color = new Color(1, 0.01415092f, 0.5780373f, 1f);
            yield return new WaitForSeconds(delay / 2);
            keys[key].GetComponent<Image>().color = Color.white;
            yield return new WaitForSeconds(delay / 2.5f);
        }
        yield return new WaitForSeconds(0.5f);
        ToggleKeys(true);
    }

    private void ToggleKeys(bool letUserPlay)
    {
        canPlay = letUserPlay;
        foreach (GameObject key in keys)
        {
            key.GetComponent<Button>().interactable = letUserPlay;
        }
    }

    private void SetStats()
    {
        stats.text = string.Format(Translate.Get("hiscore"), PlayerPrefs.GetInt("pianostreak", 0)) + "\n" + string.Format(Translate.Get("streak"), sequence.Count - 1) + "\n" + string.Format(Translate.Get("keysleft"), sequence.Count - currentPos);
    }
}