using System;
using System.Collections;
using UnityEngine;

public class TriggerableParticleSystemFX : TriggerableEffect
{
    public ParticleSystem ParticleSystem;
    public ParticleSystemFXDestructionType ParticleSystemFXDestructionType;

    public override void TriggerEffect(Action onEffectEnd)
    {
        this.ParticleSystem.Play();
        this.StartCoroutine(DestroyParticleSystem(onEffectEnd));
    }

    private IEnumerator DestroyParticleSystem(Action onEffectEnd)
    {
        switch (this.ParticleSystemFXDestructionType)
        {
            case ParticleSystemFXDestructionType.NO_MORE_EMISSION:
                yield return new WaitForParticlesToNotEmit(this.ParticleSystem);
                break;
        }
        onEffectEnd.Invoke();
    }
}

public enum ParticleSystemFXDestructionType
{
    NO_MORE_EMISSION
}
