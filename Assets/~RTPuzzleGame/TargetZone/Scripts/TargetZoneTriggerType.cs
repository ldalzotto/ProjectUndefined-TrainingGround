using UnityEngine;

public class TargetZoneTriggerType : MonoBehaviour
{
    private Collider targetZoneTriggerCollider;

    public Collider TargetZoneTriggerCollider { get => targetZoneTriggerCollider; }

    public void Init()
    {
        this.targetZoneTriggerCollider = GetComponent<Collider>();
    }
}
