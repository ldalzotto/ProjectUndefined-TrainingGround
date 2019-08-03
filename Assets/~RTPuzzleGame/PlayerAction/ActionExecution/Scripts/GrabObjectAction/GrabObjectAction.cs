using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class GrabObjectAction : RTPPlayerAction
    {
        private PlayerActionManager PlayerActionManager;

        public GrabObjectAction(GrabActionInherentData GrabActionInherentData) : base(GrabActionInherentData)
        {
        }

        public override bool FinishedCondition()
        {
            return true;
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            GrabActionInherentData GrabActionInherentData = (GrabActionInherentData)this.playerActionInherentData;
            this.PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            this.PlayerActionManager.IncreaseActionsRemainingExecutionAmount(GrabActionInherentData.PlayerActionToIncrement, 1);

            var InteractiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            InteractiveObjectContainer.OnInteractiveObjectDestroyed(InteractiveObjectContainer.GrabObjectModules[GrabActionInherentData.GrabObjectID].ParentInteractiveObject);
        }
        
        public override void Tick(float d)
        {
        }

        public override void GizmoTick() { }
        public override void GUITick() { }
        public override void LateTick(float d) { }
    }
}