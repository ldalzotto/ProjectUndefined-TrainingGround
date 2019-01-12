using System;
using UnityEngine;

public class ConeSmokeEffect : TriggerableEffect
{
    public MeshRenderer ConeMeshRenderer;
    public float speed;

    public float startUV = 0f;
    public float endUV = 0f;
    private Animator effectAnimator;
    private Action OnEffectEnd;

    private Animator EffectAnimator
    {
        get
        {
            if (effectAnimator == null) { }
            return effectAnimator;
        }
    }

    void Start()
    {
        effectAnimator = GetComponent<Animator>();
        startUV = 0f;
        endUV = 0f;
    }

    void Update()
    {
        ConeMeshRenderer.material.SetFloat("_TextureTranslationSpeed", -speed);
        ConeMeshRenderer.material.SetFloat("_StartUV", startUV);
        ConeMeshRenderer.material.SetFloat("_EndUV", endUV);
    }

    public void OnAnimationEnd()
    {
        this.OnEffectEnd.Invoke();
    }

    public override void TriggerEffect(Action onEffectEnd)
    {
        EffectAnimator.Play("EffectTriggered");
        this.OnEffectEnd = onEffectEnd;
    }
}
