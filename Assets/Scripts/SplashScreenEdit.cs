using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SplashScreenEdit : MonoBehaviour
{
    public GameObject music;
    private static bool played = false;
    IEnumerator Start()
    {
        if (!Application.isEditor && !played)
        {
            played = true;
            music.GetComponent<AudioSource>().Play();
            Time.timeScale = 0.05f;
            SplashScreen.Begin();
            while (!SplashScreen.isFinished)
            {
                SplashScreen.Draw();
                yield return null;

            }
            Time.timeScale = 1;
        }
    }
}