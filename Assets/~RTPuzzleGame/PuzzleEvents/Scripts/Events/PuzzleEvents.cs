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

        public ThrowProjectileActionStartEvent(Transform throwerTransform, float maxRange)
        {
            this.throwerTransform = throwerTransform;
            this.maxRange = maxRange;
        }

        public Transform ThrowerTransform { get => throwerTransform; }
        public float MaxRange { get => maxRange; }
    }

    public class ProjectileThrowedEvent : PuzzleEvent { }
}
