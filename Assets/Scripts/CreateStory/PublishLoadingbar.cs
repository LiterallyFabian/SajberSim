using SajberSim.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PublishLoadingbar : MonoBehaviour
{
    private static Image Loadingfield;
    [SerializeField] private Image LoadingbarLocal;

    private static Image Emptyfield;
    [SerializeField] private Image EmptyfieldLocal;

    private static Text Progress;
    [SerializeField] private Text ProgressLocal;

    
    private void Start()
    {
        Loadingfield = LoadingbarLocal;
        Progress = ProgressLocal;
        Emptyfield = EmptyfieldLocal;
    }
    public static void UpdateBar(float value)
    {
        Loadingfield.rectTransform.sizeDelta = new Vector2(value * 275, 8);
        if (value == 1) Loadingfield.color = Colors.SfwGreen;
        Progress.text = Math.Round(value * 100, 2) + "%";
        if (value == 0)
        {
            Progress.text = "";
            Emptyfield.rectTransform.sizeDelta = new Vector2(0, 8);
        }
        else Emptyfield.rectTransform.sizeDelta = new Vector2(275, 8);
    }
}