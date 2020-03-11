using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeatPulse : MonoBehaviour
{

    public float BPM = 100f;
    private bool ready = false;
    public GameObject ray;

    // Update is called once per frame
    private void Start()
    {
        StartCoroutine(Wait1s());
    }
    void Update()
    {
        var baseValue = Mathf.Cos(Time.time * Mathf.PI * (BPM / 60f) % Mathf.PI);
        ray.transform.Rotate(0, 0, 30 * Time.deltaTime);
        if (!ready) return;
        this.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1), new Vector3(1.04f,1.04f,1), baseValue);
        ray.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1), new Vector3(1.08f, 1.08f, 1), baseValue);


    }
    public IEnumerator Wait1s()
    {
        yield return new WaitForSeconds(1f);
        ready = true;
    }
}