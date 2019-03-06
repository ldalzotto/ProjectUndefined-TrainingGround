﻿using UnityEngine;


namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ProjectileInherentData", menuName = "Configuration/PuzzleGame/ProjectileConfiguration/ProjectileInherentData", order = 1)]
    public class ProjectileInherentData : ScriptableObject
    {
        [SerializeField]
        private float effectRange;

        [SerializeField]
        private float escapeSemiAngle;

        [SerializeField]
        private float travelDistancePerSeconds;

        public float EffectRange { get => effectRange; }
        public float EscapeSemiAngle { get => escapeSemiAngle; }
        public float TravelDistancePerSeconds { get => travelDistancePerSeconds; }

        #region Debug purposes
        public void SetTravelDistanceDebug(float value)
        {
            travelDistancePerSeconds = value;
        }
        #endregion
    }
}
