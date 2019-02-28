using System;
using UnityEngine;

public abstract class AnimatedLayoutCell : MonoBehaviour
{

    private Vector3 oldPosition;
    private Vector3 targetPosition;

    protected abstract Func<Vector3, Vector3> inAnimation { get; }
    protected abstract Func<Vector3, Vector3> outAnimation { get; }

    public bool AnimationTick(float d, float speed)
    {
        this.oldPosition = transform.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, d * speed);
        bool hasReachedTarget = (Vector3.Distance(transform.position, targetPosition) < 0.1f);
        if (hasReachedTarget)
        {
            transform.position = targetPosition;
        }
        return hasReachedTarget;
    }
    
    public void UpdateOldPosition()
    {
        this.oldPosition = transform.position;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    public void SetCurrentPositionAsOld()
    {
        transform.position = oldPosition;
    }

    public void SetCurrentPositionAsTarget()
    {
        transform.position = targetPosition;
    }

    public void AdjustCurrentPositionForInAniamtion()
    {
        if (inAnimation != null)
        {
            transform.position = inAnimation(transform.position);
        }
        UpdateOldPosition();
    }

    public void AdjustTargetPositionForOutAnimation()
    {
        if (outAnimation != null)
        {
            targetPosition = outAnimation(transform.position);
        }
    }
}