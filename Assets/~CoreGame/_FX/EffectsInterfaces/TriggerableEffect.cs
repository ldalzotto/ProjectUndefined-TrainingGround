using System;
using UnityEngine;

public abstract class TriggerableEffect : MonoBehaviour
{

    public virtual void Init() { }

    public abstract void TriggerEffect(Action onEffectEnd);

}
