using UnityEngine;
using System.Collections;
using OdinSerializer;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CoreMaterialConfiguration", menuName = "Configuration/CoreGame/CoreMaterialConfiguration/CoreMaterialConfiguration", order = 1)]
    public class CoreMaterialConfiguration : SerializedScriptableObject
    {
        public Material UnlitVertexColor;
        
        [Header("Selection Materials")]
        public Mesh ForwardPlane;
        public Material SelectionDoticonMaterial;
        public Texture SelectionDotIconTexture;
        public Texture SelectionDotSwitchIconTexture;

        [Header("Circle Progression Material")]
        public Material CircleProgressionMaterial;

        [Header("InteractionRing Materials")]
        public Material InteractionRingMaterial;
    }
}
