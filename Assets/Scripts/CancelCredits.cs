using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CancelCredits : MonoBehaviour
{
    public GameObject fadeimage;
    public bool started;
    public AudioSource music;
    void Update()
    {
        if (Input.GetKeyDown("escape"))
            StartCoroutine(LeaveCredits());
    }
    public IEnumerator LeaveCredits()
    {
        if (!started)
        {
            StartCoroutine(AudioFadeOut.FadeOut(music, 1.6f));
            started = true;
            fadeimage.SetActive(true); //Open image that will fade (starts at opacity 0%)

            for (float i = 0; i <= 1; i += Time.deltaTime / 1.5f) //Starts fade, load scene when done
            {
                fadeimage.GetComponent<Image>().color = new Color(0, 0, 0, i);
                yield return null;
            }
            SceneManager.LoadScene("menu");
        }
    }
}
