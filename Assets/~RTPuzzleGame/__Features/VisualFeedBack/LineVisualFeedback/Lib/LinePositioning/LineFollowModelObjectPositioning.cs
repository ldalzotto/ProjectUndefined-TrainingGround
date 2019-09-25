using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class LineFollowModelObjectPositioning : ILinePositioning
    {
        private ModelObjectModule ModelObjectModuleToFollow;
        private Vector3 targetWorldPositionOffset;

        public LineFollowModelObjectPositioning(ModelObjectModule ModelObjectModuleToFollow)
        {
            this.ModelObjectModuleToFollow = ModelObjectModuleToFollow;
            this.targetWorldPositionOffset = IRenderBoundRetrievableStatic.GetLineRenderPointLocalOffset(ModelObjectModuleToFollow);
        }

        public Vector3 GetEndPosition(Vector3 startPosition)
        {
            return this.ModelObjectModuleToFollow.transform.position + this.targetWorldPositionOffset;
        }
    }

}
