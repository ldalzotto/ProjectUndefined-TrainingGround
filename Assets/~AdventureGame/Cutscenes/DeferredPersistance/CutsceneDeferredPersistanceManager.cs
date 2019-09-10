using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;

namespace AdventureGame
{
    public class CutsceneDeferredPersistanceManager
    {
        #region External Dependencies
        private GhostsPOIManager GhostsPOIManager;
        #endregion

        private List<CutsceneDeferredPOIpersistanceInput> WaitingToPersistInputs;

        public CutsceneDeferredPersistanceManager()
        {
            this.GhostsPOIManager = (GhostsPOIManager)CoreGameSingletonInstances.AGhostPOIManager;
            this.WaitingToPersistInputs = new List<CutsceneDeferredPOIpersistanceInput>();
        }

        #region External Event
        public void PushDeferredPersistance(CutsceneDeferredPOIpersistanceInput cutsceneDeferredPOIpersistanceInput)
        {
            this.WaitingToPersistInputs.Add(cutsceneDeferredPOIpersistanceInput);
        }

        public void OnCutsceneEnd()
        {
            foreach (var WaitingToPersistInput in WaitingToPersistInputs)
            {
                WaitingToPersistInput.DoPersist(this.GhostsPOIManager);
            }
        }
        #endregion
    }

    public class CutsceneDeferredPOIpersistanceInput
    {
        public PointOfInterestId pointOfInterestId;
        public TransformBinarry worldPosition;
        public AnimationID poseAnimationID;
        public LevelZoneChunkID currentLevelZoneChunkID;

        public CutsceneDeferredPOIpersistanceInput(PointOfInterestId pointOfInterestId, TransformBinarry worldPosition, AnimationID poseAnimationID, LevelZoneChunkID currentLevelZoneChunkID)
        {
            this.pointOfInterestId = pointOfInterestId;
            this.worldPosition = worldPosition;
            this.poseAnimationID = poseAnimationID;
            this.currentLevelZoneChunkID = currentLevelZoneChunkID;
        }

        public void DoPersist(GhostsPOIManager GhostsPOIManager)
        {
            var ghostPOI = GhostsPOIManager.GetGhostPOI(this.pointOfInterestId);
            if (ghostPOI != null)
            {
                ghostPOI.OnPositionChanged(this.worldPosition, this.currentLevelZoneChunkID);

                if (this.poseAnimationID != AnimationID.NONE)
                {
                    ghostPOI.OnAnimationPositioningPlayed(this.poseAnimationID);
                }
            }
        }
    }
}
