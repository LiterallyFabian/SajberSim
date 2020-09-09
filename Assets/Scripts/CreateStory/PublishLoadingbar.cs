using SajberSim.Colors;
using SajberSim.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PublishLoadingbar : MonoBehaviour
{
    [SerializeField] private Image Loadingfield;
    [SerializeField] private Image Emptyfield;
    [SerializeField] private Text ProgressNumber;

    private void Update()
    {
        if (Workshop.publishProgress != 0) UpdateBar(Workshop.publishProgress);
        if (Workshop.publishProgress > 1) Workshop.publishProgress = 0;
    }
    public void UpdateBar(float value)
    {
         Loadingfield.rectTransform.sizeDelta = new Vector2(value * 275, 8);
        if (value == 1) Loadingfield.color = Colors.SfwGreen;
        ProgressNumber.text = Math.Round(value * 100, 2) + "%";
        if (value == 0)
        {
            ProgressNumber.text = "";
            Emptyfield.rectTransform.sizeDelta = new Vector2(0, 8);
        }
        else Emptyfield.rectTransform.sizeDelta = new Vector2(275, 8);
    }
}