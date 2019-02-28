using System;
using UnityEngine;

public abstract class TriggerableEffect : MonoBehaviour
{

    public abstract void TriggerEffect(Action onEffectEnd);

}
