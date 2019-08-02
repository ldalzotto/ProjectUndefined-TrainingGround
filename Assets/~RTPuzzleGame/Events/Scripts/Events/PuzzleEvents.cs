using GameConfigurationID;
using System;
using UnityEngine;

namespace RTPuzzle
{

    public class ThrowProjectileActionStartEvent
    {
        private Transform throwerTransform;
        private float maxRange;
        private Func<Vector3> currentCursorPositionRetriever;
        private LaunchProjectileID projectileInvolved;

        public ThrowProjectileActionStartEvent(Transform throwerTransform, float maxRange, Func<Vector3> currentCursorPositionRetriever, LaunchProjectileID projectileInvolved)
        {
            this.throwerTransform = throwerTransform;
            this.maxRange = maxRange;
            this.currentCursorPositionRetriever = currentCursorPositionRetriever;
            this.projectileInvolved = projectileInvolved;
        }

        public Transform ThrowerTransform { get => throwerTransform; }
        public float MaxRange { get => maxRange; }
        public Func<Vector3> CurrentCursorPositionRetriever { get => currentCursorPositionRetriever; }
        public LaunchProjectileID ProjectileInvolved { get => projectileInvolved; }
    }
    
}
