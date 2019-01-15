using System;
using UnityEngine;

public class ConeSmokeEffectV2 : TriggerableEffect
{
    public ParticleSystem smokePartciles;

    public override void TriggerEffect(Action onEffectEnd)
    {
        smokePartciles.Play();
    }
}
