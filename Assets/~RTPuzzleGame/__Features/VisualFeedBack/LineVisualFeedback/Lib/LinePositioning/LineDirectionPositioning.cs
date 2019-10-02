using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class LineDirectionPositioning : ILinePositioning
    {
        private ModelObjectModule lineStartModelObject;
        private Component sourceTriggeringObject;

        public LineDirectionPositioning(ModelObjectModule lineStartModelObject, Component sourceTriggeringObject)
        {
            this.lineStartModelObject = lineStartModelObject;
            this.sourceTriggeringObject = sourceTriggeringObject;
        }

        public Vector3 GetEndPosition(Vector3 startPosition)
        {
            var startObjectBound = lineStartModelObject.GetAverageModelBoundLocalSpace();
            var maxRadius = Mathf.Max(startObjectBound.SideDistances.x, startObjectBound.SideDistances.z) * 0.5f;
            var triggeringObjectTransform = this.sourceTriggeringObject.transform;
            var projectedDirection = Vector3.ProjectOnPlane((triggeringObjectTransform.position - startPosition), lineStartModelObject.transform.up).normalized;
            return startPosition + (maxRadius * -3 * projectedDirection);
        }
    }
}
