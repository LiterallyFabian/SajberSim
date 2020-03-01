using UnityEngine;
using UnityEngine.UI;

public class BeatPulse : MonoBehaviour
{

    public float BPM = 100f;

    // Update is called once per frame
    void Update()
    {
        var baseValue = Mathf.Cos(((Time.time * Mathf.PI) * (BPM / 60f)) % Mathf.PI);
        this.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1), new Vector3(1.04f,1.04f,1), baseValue);
    }
}