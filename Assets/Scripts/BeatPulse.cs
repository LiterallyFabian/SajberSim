using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeatPulse : MonoBehaviour
{

    public float BPM = 100f;
    private bool ready = false;

    // Update is called once per frame
    private void Start()
    {
        StartCoroutine(Wait1s());
    }
    void Update()
    {
        var baseValue = Mathf.Cos(((Time.time * Mathf.PI) * (BPM / 60f)) % Mathf.PI);
        if(ready)
        this.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1), new Vector3(1.04f,1.04f,1), baseValue);
    }
    public IEnumerator Wait1s()
    {
        yield return new WaitForSeconds(1f);
        ready = true;
    }
}