using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedTorch : MonoBehaviour
{
    [Header("Sequence Settings")]
    public int torchIndex;          // order number in puzzle
    public float burnDuration = 10f; // seconds before timeout

    [Header("Events")]
    public UnityEvent onCorrectSequence;

    [Header("VFX / SFX")]
    public ParticleSystem flameFX;
    public AudioSource flameSfx;

    private static readonly List<int> sequence = new() { 2, 4, 1 };
    private static readonly List<TimedTorch> allTorches = new();
    private static int currentStep = 0;

    private bool isLit = false;
    private Coroutine burnTimer;

    private void Awake() { allTorches.Add(this); }
    private void OnDestroy() { allTorches.Remove(this); }

    public void OnDamageReceived(TakeDamageEffect hit)
    {
        if (isLit) return;
        if (hit.weaponElement != WeaponElement.Fire) return;

        if (torchIndex == sequence[currentStep])
        {
            LightTorch();
            currentStep++;

            if (currentStep >= sequence.Count)
            {
                onCorrectSequence?.Invoke();
                //ResetSequenceAll();
            }
            else
            {
                // restart shared timer
                if (burnTimer != null) StopCoroutine(burnTimer);
                burnTimer = StartCoroutine(Timeout());
            }
        }
        else
        {
            // wrong torch
            ResetSequenceAll();
        }
    }

    private void LightTorch()
    {
        isLit = true;
        if(flameFX) 
            flameFX.Play();
        if (flameSfx) 
            flameSfx.Play();
    }

    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(burnDuration);
        ResetSequenceAll();
    }

    private void Extinguish()
    {
        isLit = false;
        if (flameFX)
            flameFX.Stop();
        if (flameSfx)
            flameSfx.Stop();
    }

    private static void ResetSequenceAll()
    {
        currentStep = 0;

        foreach (TimedTorch t in allTorches)
        {
            if (t == null) continue;
            if (t.burnTimer != null)
            {
                t.StopCoroutine(t.burnTimer);
                t.burnTimer = null;
            }
            if (t.isLit) t.Extinguish();
        }
    }
}
