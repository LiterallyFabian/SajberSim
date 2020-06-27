using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#pragma warning disable CS0618 // Type or member is obsolete
class LogoSpin : MonoBehaviour
{
    private ParticleSystem stars;
    private bool spinning = false;
    private void Start()
    {
        stars = GameObject.Find("ParticleHolder/logo").GetComponent<ParticleSystem>();
    }
    public void Spin()
    {
        if (!spinning) StartCoroutine(StartSpin());
    }
    IEnumerator StartSpin()
    {
        spinning = true;
        GetComponent<Animator>().Play("logo spin", 0, 0);
        yield return new WaitForSeconds(0.8f);
        if (UnityEngine.Random.value > 0.9f) stars.emissionRate = 5000;
        stars.startDelay = 0;
        stars.Play();
        yield return new WaitForSeconds(0.2f);
        ResetParticles();
    }
    private void ResetParticles()
    {
        spinning = false;
        stars.startDelay = 1.1f;
        stars.emissionRate = 500;
    }
}
