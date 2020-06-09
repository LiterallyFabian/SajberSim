using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// This example shows how you could draw the splash screen at the start of a Scene. This is a good way to integrate the splash screen with your own or add extras such as Audio.
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
            Time.timeScale = 0.07f;
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