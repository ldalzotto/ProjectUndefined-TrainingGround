using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace RTPuzzle
{
    public class GrabObjectAction : RTPPlayerAction
    {
        private PlayerActionManager PlayerActionManager;

        public GrabObjectAction(GrabActionInherentData GrabActionInherentData) : base(GrabActionInherentData)
        {
        }

        #region Data Retrieval 
        public PlayerActionId GetPlayerActionToIncrementOrAdd()
        {
            return ((GrabActionInherentData)this.playerActionInherentData).PlayerActionToIncrementOrAdd;
        }
        #endregion

        public override bool FinishedCondition()
        {
            return true;
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            GrabActionInherentData GrabActionInherentData = (GrabActionInherentData)this.playerActionInherentData;
            this.PlayerActionManager = PuzzleGameSingletonInstances.PlayerActionManager;
            this.PlayerActionManager.IncreaseOrAddActionsRemainingExecutionAmount(GrabActionInherentData.PlayerActionToIncrementOrAdd, 1);
        }
        
        public override void Tick(float d)
        {
        }

        public override void GizmoTick() { }
        public override void GUITick() { }
        public override void LateTick(float d) { }
    }
}