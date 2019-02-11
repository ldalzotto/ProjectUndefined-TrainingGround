using System;
using UnityEngine;

namespace RTPuzzle
{
    public abstract class PuzzleEvent
    {
        public bool IsEventOfType<T>() where T : PuzzleEvent
        {
            return GetType() == typeof(T);
        }
    }

    public class ThrowProjectileActionStartEvent : PuzzleEvent
    {
        private Transform throwerTransform;
        private float maxRange;
        private Func<Nullable<Vector3>> currentCursorPositionRetriever;
        private LaunchProjectileId projectileInvolved;

        public ThrowProjectileActionStartEvent(Transform throwerTransform, float maxRange, Func<Nullable<Vector3>> currentCursorPositionRetriever, LaunchProjectileId projectileInvolved)
        {
            this.throwerTransform = throwerTransform;
            this.maxRange = maxRange;
            this.currentCursorPositionRetriever = currentCursorPositionRetriever;
            this.projectileInvolved = projectileInvolved;
        }

        public Transform ThrowerTransform { get => throwerTransform; }
        public float MaxRange { get => maxRange; }
        public Func<Vector3?> CurrentCursorPositionRetriever { get => currentCursorPositionRetriever; }
        public LaunchProjectileId ProjectileInvolved { get => projectileInvolved; }
    }

    public class ProjectileThrowedEvent : PuzzleEvent { }
}
