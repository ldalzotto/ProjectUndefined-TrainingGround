using System.Collections.Generic;

namespace InteractiveObjectsAnimatorPlayable
{
    internal struct AnimationLayer
    {
        public int ID;

        public AnimationLayer(int id)
        {
            ID = id;
        }
    }

    internal enum AnimationLayerID
    {
        LocomotionLayer,
        ContextActionLayer
    }

    internal static class AnimationLayerStatic
    {
        internal static Dictionary<AnimationLayerID, AnimationLayer> AnimationLayers = new Dictionary<AnimationLayerID, AnimationLayer>()
        {
            {AnimationLayerID.LocomotionLayer, new AnimationLayer(0)},
            {AnimationLayerID.ContextActionLayer, new AnimationLayer(10)}
        };
    }
}