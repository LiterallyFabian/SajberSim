using SajberSim.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DebugNovel : MonoBehaviour
{
    public Scrollbar D_scroll;
    public Text D_Stats;
    public Text D_ErrorList;
    public NovelDebugger Debugger;
    public void UpdateList()
    {
        Debugger.DebugNovel(CreateStory.currentlyEditingPath);
        D_scroll.value = 0.999f;
        D_Stats.text =
            $"{string.Format(Translate.Get("scripts"), Debugger.ScriptAmount)}\n" +
            $"{string.Format(Translate.Get("actions"), Debugger.ActionAmount)}\n\n" +
            $"{string.Format(Translate.Get("workingscripts"), $"{Debugger.WorkingScripts}/{Debugger.ScriptAmount}")}\n" +
            $"{string.Format(Translate.Get("workingactions"), $"{Debugger.WorkingActions}/{Debugger.ActionAmount}")}\n\n" +
            $"{string.Format(Translate.Get("errorscripts"), $"{Debugger.ErrorScripts}/{Debugger.ScriptAmount}")}\n" +
            $"{string.Format(Translate.Get("erroractions"), $"{Debugger.ErrorActions}/{Debugger.ActionAmount}")}\n";
        D_ErrorList.text = Debugger.ErrorList.ToString();
        D_ErrorList.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Debugger.ErrorList.ToString().Split('\n').Length * 21.5f + 200);
        if (Debugger.ErrorScripts == 0) D_ErrorList.text = Translate.Get("noerrors");


    }
}
