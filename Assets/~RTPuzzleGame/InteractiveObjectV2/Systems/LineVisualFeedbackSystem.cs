using CoreGame;
using GameConfigurationID;
using RTPuzzle;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjectTest
{
    public class LineVisualFeedbackSystem
    {
        private Vector3 positionOffsetFromNPC;

        private List<CoreInteractiveObject> sourceTriggeringInteractiveObjects = new List<CoreInteractiveObject>();
        private List<ILinePositioning> linePositionings = new List<ILinePositioning>();
        private List<DottedLine> lines = new List<DottedLine>();

        private InteractiveGameObject InteractiveGameObjectRef;

        public LineVisualFeedbackSystem(InteractiveGameObject InteractiveGameObject)
        {
            this.InteractiveGameObjectRef = InteractiveGameObject;
            //position calculation
            this.positionOffsetFromNPC = IRenderBoundRetrievableStatic.GetLineRenderPointLocalOffset(InteractiveGameObject.AverageModelBounds);
        }

        public void TickAlways(float d)
        {
            for (var i = 0; i < this.lines.Count; i++)
            {
                var startPosition = this.InteractiveGameObjectRef.GetTransform().WorldPosition + this.positionOffsetFromNPC;
                this.lines[i].Tick(d, startPosition, this.linePositionings[i].GetEndPosition(startPosition));
            }
        }

        public void OnDestroy()
        {
            for (var i = 0; i < this.lines.Count; i++)
            {
                this.DestroyLine(this.sourceTriggeringInteractiveObjects[i]);
            }
        }

        #region External Events
        public void CreateLineFollowing(DottedLineID DottedLineID, CoreInteractiveObject TargetInteractiveGameObject)
        {
            this.sourceTriggeringInteractiveObjects.Add(TargetInteractiveGameObject);
            this.lines.Add(new DottedLine(DottedLineID));
            var targetGameObject = TargetInteractiveGameObject.InteractiveGameObject;
            this.linePositionings.Add(new LineFollowTransformPositioning(targetGameObject.InteractiveGameObjectParent.transform, targetGameObject.AverageModelBounds));
        }

        /*
        public void CreateLineDirectionPositioning(DottedLineID DottedLineID, CoreInteractiveObject sourceInteractiveObject)
        {
            this.sourceTriggeringInteractiveObjects.Add(sourceInteractiveObject);
            this.lines.Add(DottedLine.CreateInstance(DottedLineID, PuzzleGameSingletonInstances.PuzzleGameConfigurationManager));
            this.linePositionings.Add(new LineDirectionPositioning(this.ModelObjectModule, sourceTriggeringObject));
        }
        */

        public void DestroyLine(CoreInteractiveObject sourceInteractiveObject)
        {
            var index = this.sourceTriggeringInteractiveObjects.IndexOf(sourceInteractiveObject);
            if (index >= 0)
            {
                this.sourceTriggeringInteractiveObjects.RemoveAt(index);
                this.linePositionings.RemoveAt(index);
                this.lines[index].OnDestroy();
                this.lines.RemoveAt(index);
            }
        }
        #endregion
    }

}
