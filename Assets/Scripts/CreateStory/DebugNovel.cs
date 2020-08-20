using SajberSim.Helper;
using SajberSim.Translation;
using System;
using System.Collections.Generic;
using System.IO;
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
        string fixedList = FixLength(Debugger.ErrorList);
        D_ErrorList.text = fixedList;
        D_ErrorList.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fixedList.Split('\n').Length * 33.5f + 200);
        if (Debugger.ErrorScripts == 0) D_ErrorList.text = Translate.Get("noerrors");
    }
    private string FixLength(StringBuilder sb)
    {
        if (Debugger.ErrorScripts == 0)
        {
            return Translate.Get("noerrors");
        }
        string rawList = sb.ToString();
        string fixedList = rawList.Length > 10000 ? rawList.Substring(0, 10000) : rawList;
        if (rawList.Length > 10000)
            return fixedList + "\n\nNOTE: The novel contains more errors that could not fit in this list. Fix the errors above and run the debugger again, or generate a debug log to see them.";
        return fixedList;
    }
    public void GenerateLog()
    {
        if (Debugger.ErrorScripts == 0)
            File.WriteAllText(Helper.currentStoryPath + "/debug.log", Translate.Get("noerrors"));
        else
        {
            string text = Debugger.ErrorList.ToString();
            text.Replace("<b>", "");
            text.Replace("</b>", "");
            File.WriteAllText(Helper.currentStoryPath + "/debug.log", text);
        }
            
        System.Diagnostics.Process.Start(Helper.currentStoryPath + "/debug.log");
    }
}
