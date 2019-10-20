using System;
using OdinSerializer;
using UnityEngine;

namespace SelectionWheel
{
    [Serializable]
    [CreateAssetMenu(fileName = "SelectionWheelGlobalConfiguration", menuName = "Configuration/CoreGame/SelectionWheelGlobalConfiguration/SelectionWheelGlobalConfiguration", order = 1)]
    public class SelectionWheelGlobalConfiguration : SerializedScriptableObject
    {
        public ActionWheelNodePositionManagerComponent ActionWheelNodePositionManagerComponent;
        public Material NonSelectedMaterial;
        public Material SelectedMaterial;
        public GameObject SelectionWheelPrefab;
    }

    [Serializable]
    public class ActionWheelNodePositionManagerComponent
    {
        public float DistanceFromCenter;
        public float RotationSpeed;
    }
}