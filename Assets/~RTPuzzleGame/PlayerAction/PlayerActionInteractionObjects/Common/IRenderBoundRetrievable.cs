using UnityEngine;

namespace RTPuzzle
{
    public interface IRenderBoundRetrievable
    {
        Bounds GetAverageModelBoundLocalSpace();
    }

    public static class IRenderBoundRetrievableStatic
    {
        public const float LineYPositionBoundsFactor = 1f;
        public static Vector3 GetLineRenderPointLocalOffset(IRenderBoundRetrievable iRenderBoundRetrievable)
        {
            return (Vector3.up * iRenderBoundRetrievable.GetAverageModelBoundLocalSpace().max.y * LineYPositionBoundsFactor);// iRenderBoundRetrievable.GetAverageModelBoundLocalSpace();
        }

        public static MonoBehaviour FromIRenderBoundRetrievable(IRenderBoundRetrievable iRenderBoundRetrievable)
        {
            return (MonoBehaviour)iRenderBoundRetrievable;
        }
    }
}
