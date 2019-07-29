using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public interface IRenderBoundRetrievable
    {
        ExtendedBounds GetAverageModelBoundLocalSpace();
    }

    public static class IRenderBoundRetrievableStatic
    {
        public static Vector3 GetLineRenderPointLocalOffset(IRenderBoundRetrievable iRenderBoundRetrievable)
        {
            return (Vector3.up * iRenderBoundRetrievable.GetAverageModelBoundLocalSpace().Bounds.max.y);
        }

        public static Vector3 GetDisarmProgressBarLocalOffset(IRenderBoundRetrievable iRenderBoundRetrievable)
        {
            return (Vector3.up * (iRenderBoundRetrievable.GetAverageModelBoundLocalSpace().Bounds.max.y + 1));
        }

        public static Vector3 GetRepelLineRenderPointLocalOffset(IRenderBoundRetrievable iRenderBoundRetrievable)
        {
            return (Vector3.up * iRenderBoundRetrievable.GetAverageModelBoundLocalSpace().Bounds.max.y * 0.15f);
        }

        public static MonoBehaviour FromIRenderBoundRetrievable(IRenderBoundRetrievable iRenderBoundRetrievable)
        {
            return (MonoBehaviour)iRenderBoundRetrievable;
        }
    }
}
