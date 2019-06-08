using UnityEngine;

namespace RTPuzzle
{
    public class BoxGroundEffectManager : AbstractGroundEffectManager<BoxRangeType>
    {
        public BoxGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData) : base(rangeTypeInherentConfigurationData)
        {
        }

        public BoxRangeBufferData ToBoxBuffer()
        {
            var boxRangeBufferData = new BoxRangeBufferData();
            boxRangeBufferData.Forward = this.associatedRange.transform.forward;
            boxRangeBufferData.Up = this.associatedRange.transform.up;
            boxRangeBufferData.Right = this.associatedRange.transform.right;
            boxRangeBufferData.Center = this.associatedRange.WorldPositionCenter;
            boxRangeBufferData.LocalSize = this.associatedRange.LocalSize;

            if (this.rangeTypeInherentConfigurationData.RangeColorProvider != null)
            {
                boxRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            }
            else
            {
                boxRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeBaseColor;
            }
            boxRangeBufferData.AuraTextureAlbedoBoost = 0.2f;
            boxRangeBufferData.AuraAnimationSpeed = 20f;

            return boxRangeBufferData;
        }

    }

    public struct BoxRangeBufferData
    {
        public Vector3 Forward;
        public Vector3 Up;
        public Vector3 Right;
        public Vector3 Center;
        public Vector3 LocalSize;

        public Vector4 AuraColor;
        public float AuraTextureAlbedoBoost;
        public float AuraAnimationSpeed;

        public static int GetByteSize()
        {
            return ((5 * 3) + 4 + 1 + 1) * sizeof(float);
        }
    }

}
