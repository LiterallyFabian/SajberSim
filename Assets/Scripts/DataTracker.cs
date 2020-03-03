using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class DataTracker : MonoBehaviour
{
    public static void ReportQuestion(string question, int choice)
    {
        Analytics.CustomEvent("question_answered", new Dictionary<string, object>
        {
            { "question", question },
            { "answer", choice }
        });
    }
}
