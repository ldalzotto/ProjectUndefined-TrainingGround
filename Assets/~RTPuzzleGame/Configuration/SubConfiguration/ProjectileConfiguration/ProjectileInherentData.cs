﻿using ConfigurationEditor;
using UnityEngine;


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

        [DictionaryEnumSearch]
        public float EffectRange { get => effectRange; }
        [DictionaryEnumSearch]
        public float EscapeSemiAngle { get => escapeSemiAngle; }
        [DictionaryEnumSearch]
        public float TravelDistancePerSeconds { get => travelDistancePerSeconds; }

        public void Init(float effectRange, float escapeSemiAngle, float travelDistancePerSeconds)
        {
            this.effectRange = effectRange;
            this.escapeSemiAngle = escapeSemiAngle;
            this.travelDistancePerSeconds = travelDistancePerSeconds;
        }

        #region Debug purposes
        public void SetTravelDistanceDebug(float value)
        {
            travelDistancePerSeconds = value;
        }
        #endregion
    }
}
