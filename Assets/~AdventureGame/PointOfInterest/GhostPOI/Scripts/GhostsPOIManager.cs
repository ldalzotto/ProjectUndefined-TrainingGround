using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdventureGame
{
    /// <summary>
    /// A <see cref="GhostsPOIManager"/> represent the persistance of internal state of a <see cref="PointOfInterestManager"/>.
    /// While <see cref="GhostsPOIManager"/> persist across the game, a <see cref="PointOfInterestManager"/> is the physical entity in the scene.
    /// Every state change occurs at the <see cref="GhostPOI"/> that store and pass the information to <see cref="PointOfInterestManager"/>.
    /// When a <see cref="PointOfInterestManager"/> is created, it first sync to his associated <see cref="GhostPOI"/> to be in sync with the current state.
    /// </summary>
    public class GhostsPOIManager : AGhostPOIManager
    {

        private Dictionary<PointOfInterestId, GhostPOI> ghostPOIs = null;
        private GhostPOIManagerPersister GhostPOIManagerPersister;

        public override void Init()
        {
            this.GhostPOIManagerPersister = new GhostPOIManagerPersister();

            if (this.ghostPOIs == null)
            {
                var loadedGhostPOIS = this.GhostPOIManagerPersister.Load();
                if (loadedGhostPOIS == null)
                {
                    ghostPOIs = new Dictionary<PointOfInterestId, GhostPOI>();
                    var poiIds = Enum.GetValues(typeof(PointOfInterestId)).Cast<PointOfInterestId>();
                    foreach (var poiId in poiIds)
                    {
                        ghostPOIs.Add(poiId, new GhostPOI());
                    }
                }
                else
                {
                    this.ghostPOIs = loadedGhostPOIS;
                }
            }

            foreach (var ghostPOI in this.ghostPOIs.Values)
            {
                ghostPOI.Init(this);
            }
        }

        public GhostPOI GetGhostPOI(PointOfInterestId pointOfInterestId)
        {
            return this.ghostPOIs[pointOfInterestId];
        }

        public override List<PointOfInterestDefinitionID> GetAllPOIIdElligibleToBeDynamicallyInstanciated(List<LevelZoneChunkID> levelZoneChunkIDs)
        {
            var returnList = new List<PointOfInterestDefinitionID>();
            foreach (var ghostPOIEntry in this.ghostPOIs)
            {
                var currentGhostPOI = ghostPOIEntry.Value;
                if (currentGhostPOI.PointOfInterestLevelPositioningState != null && currentGhostPOI.PointOfInterestLevelPositioningState.LevelZoneChunkID != LevelZoneChunkID.NONE
                    && levelZoneChunkIDs.Contains(currentGhostPOI.PointOfInterestLevelPositioningState.LevelZoneChunkID) &&
                    currentGhostPOI.PointOfInterestModelState != null && !currentGhostPOI.PointOfInterestModelState.IsDisabled)
                {
                    returnList.Add(currentGhostPOI.PointOfInterestIdentificationState.PointOfInterestDefinitionID);
                }
            }
            return returnList;
        }

        #region External Events
        public override void OnPOICreated(APointOfInterestType pointOfInterestType)
        {
            ((PointOfInterestType)pointOfInterestType).SyncPOIFromGhostPOI(this.ghostPOIs[((PointOfInterestType)pointOfInterestType).PointOfInterestId]);
            ((PointOfInterestType)pointOfInterestType).SynchIndentificationStateToGhostPOI(this.ghostPOIs[((PointOfInterestType)pointOfInterestType).PointOfInterestId]);
            this.OnGhostPOIChanged();
        }

        public override void OnPOIDisabled(APointOfInterestType pointOfInterestType)
        {
            ((PointOfInterestType)pointOfInterestType).SyncPointOfInterestModelStateToGhostPOI(this.ghostPOIs[((PointOfInterestType)pointOfInterestType).PointOfInterestId]);
            this.OnGhostPOIChanged();
        }

        public virtual void OnPOIEnabled(PointOfInterestId pointOfInterestId)
        {

        }


        public virtual void OnGhostPOIChanged()
        {
            this.GhostPOIManagerPersister.SaveAsync(this.ghostPOIs);
        }
        #endregion
    }


    [System.Serializable]
    public class GhostPOI
    {
        [SerializeField]
        private PointOfInterestIdentificationState pointOfInterestIdentificationState;
        [SerializeField]
        private PointOfInterestScenarioState PointOfInterestScenarioState;
        [SerializeField]
        private ContextActionSynchronizerManager contextActionSynchronizerManager;
        [SerializeField]
        private PointOfInterestModelState pointOfInterestModelState;
        [SerializeField]
        private PointOfInterestAnimationPositioningState pointOfInterestAnimationPositioningState;
        [SerializeField]
        private PointOfInterestLevelPositioningState pointOfInterestLevelPositioningState;

        public PointOfInterestScenarioState PointOfInterestScenarioState1 { get => PointOfInterestScenarioState; }
        internal ContextActionSynchronizerManager ContextActionSynchronizerManager { get => contextActionSynchronizerManager; }
        public PointOfInterestModelState PointOfInterestModelState { get => pointOfInterestModelState; set => pointOfInterestModelState = value; }
        public PointOfInterestAnimationPositioningState PointOfInterestAnimationPositioningState { get => pointOfInterestAnimationPositioningState; set => pointOfInterestAnimationPositioningState = value; }
        public PointOfInterestLevelPositioningState PointOfInterestLevelPositioningState { get => pointOfInterestLevelPositioningState; set => pointOfInterestLevelPositioningState = value; }
        public PointOfInterestIdentificationState PointOfInterestIdentificationState { get => pointOfInterestIdentificationState; }

        [NonSerialized]
        private GhostsPOIManager ghostsPOIManagerRef;

        public GhostPOI()
        {
            pointOfInterestIdentificationState = new PointOfInterestIdentificationState();
            PointOfInterestScenarioState = new PointOfInterestScenarioState();
            contextActionSynchronizerManager = new ContextActionSynchronizerManager();
            PointOfInterestModelState = new PointOfInterestModelState();
            pointOfInterestAnimationPositioningState = new PointOfInterestAnimationPositioningState();
            pointOfInterestLevelPositioningState = new PointOfInterestLevelPositioningState();
        }

        public void Init(GhostsPOIManager ghostsPOIManagerRef)
        {
            this.ghostsPOIManagerRef = ghostsPOIManagerRef;
        }

        #region External Events
        public void OnContextActionAdd(AContextAction contextActionToAdd)
        {
            contextActionSynchronizerManager.OnContextActionAdd(contextActionToAdd);
            this.OnGhostPOIChanged();
        }
        public void OnItemRelatedContextActionAdd(ItemID item, AContextAction contextActionToAdd)
        {
            contextActionSynchronizerManager.OnContextActionAdd(item, contextActionToAdd);
            this.OnGhostPOIChanged();
        }

        public void OnGrabbableItemRemove(ItemID itemId)
        {
            contextActionSynchronizerManager.OnContextActionremove(itemId);
            this.OnGhostPOIChanged();
        }

        public void OnReceivableItemAdd(ItemID itemID)
        {
            if (PointOfInterestScenarioState.ReceivableItemsComponent == null)
            {
                PointOfInterestScenarioState.ReceivableItemsComponent = new ReceivableItemsComponent();
            }
            PointOfInterestScenarioState.ReceivableItemsComponent.Add(itemID);
            this.OnGhostPOIChanged();
        }

        public void OnReceivableItemRemove(ItemID itemID)
        {
            if (PointOfInterestScenarioState.ReceivableItemsComponent != null)
            {
                PointOfInterestScenarioState.ReceivableItemsComponent.Remove(itemID);
                this.OnGhostPOIChanged();
            }
        }
        public void OnDiscussionTreeAdd(DiscussionTreeId discussionTreeId, AContextAction contextActionToAdd)
        {
            contextActionSynchronizerManager.OnContextActionAdd(discussionTreeId, contextActionToAdd);
            this.OnGhostPOIChanged();
        }
        public void OnDiscussionTreeRemove(DiscussionTreeId discussionTreeId)
        {
            contextActionSynchronizerManager.OnContextActionremove(discussionTreeId);
            this.OnGhostPOIChanged();
        }
        public void OnInteractableItemAdd(ItemID itemID)
        {
            if (PointOfInterestScenarioState.InteractableItemsComponent == null)
            {
                PointOfInterestScenarioState.InteractableItemsComponent = new InteractableItemsComponent();
            }
            PointOfInterestScenarioState.InteractableItemsComponent.Add(itemID);
            this.OnGhostPOIChanged();
        }

        public void OnInteractableItemRemove(ItemID itemID)
        {
            if (PointOfInterestScenarioState.InteractableItemsComponent != null)
            {
                PointOfInterestScenarioState.InteractableItemsComponent.Remove(itemID);
                this.OnGhostPOIChanged();
            }
        }

        public void OnLevelZoneTransitionAdd(LevelZonesID levelZonesID, AContextAction contextActionToAdd)
        {
            contextActionSynchronizerManager.OnContextActionAdd(contextActionToAdd);
            this.OnGhostPOIChanged();
        }

        public void OnDisablePOI()
        {
            this.PointOfInterestModelState.OnPOIDisabled();
            this.OnGhostPOIChanged();
        }

        public void OnEnablePOI()
        {
            this.PointOfInterestModelState.OnPOIEnabled();
            this.OnGhostPOIChanged();
        }

        public void OnAnimationPositioningPlayed(AnimationID lastAnimation)
        {
            if (this.pointOfInterestAnimationPositioningState == null)
            {
                this.pointOfInterestAnimationPositioningState = new PointOfInterestAnimationPositioningState();
            }
            this.pointOfInterestAnimationPositioningState.LastPlayedAnimation = lastAnimation;
            this.OnGhostPOIChanged();
        }

        public void OnPositionChanged(Transform position, LevelZoneChunkID levelZoneChunkID)
        {
            if (this.pointOfInterestLevelPositioningState == null)
            {
                this.pointOfInterestLevelPositioningState = new PointOfInterestLevelPositioningState();
            }
            this.pointOfInterestLevelPositioningState.LevelZoneChunkID = levelZoneChunkID;
            this.pointOfInterestLevelPositioningState.TransformBinarry = new TransformBinarry(position);
            this.OnGhostPOIChanged();
        }

        #endregion

        private void OnGhostPOIChanged()
        {
            this.ghostsPOIManagerRef.OnGhostPOIChanged();
        }


    }

    #region Context Action Synchronizer
    [System.Serializable]
    class ContextActionSynchronizerManager
    {
        [SerializeField]
        private Dictionary<string, List<AContextAction>> contextActions = new Dictionary<string, List<AContextAction>>();

        public List<AContextAction> ContextActions
        {
            get
            {
                return contextActions.Values.ToList().SelectMany(a => a).ToList();
            }
        }

        public void OnContextActionAdd(AContextAction contextActionToAdd)
        {
            var key = contextActionToAdd.GetType().Name;
            ContextActionAddSilently(contextActionToAdd, key);
        }

        public void OnContextActionAdd(Enum key, AContextAction contextActionToAdd)
        {
            ContextActionAddSilently(contextActionToAdd, key.ToString());
        }

        public void OnContextActionremove(Enum key)
        {
            contextActions.Remove(key.ToString());
        }

        private void ContextActionAddSilently(AContextAction contextActionToAdd, string key)
        {
            if (!contextActions.ContainsKey(key))
            {
                contextActions.Add(key, new List<AContextAction>());
            }
            contextActions[key].Add(contextActionToAdd);
        }

    }
    #endregion

    #region GhostPOI peristance
    class GhostPOIManagerPersister : AbstractGamePersister<Dictionary<PointOfInterestId, GhostPOI>>
    {
        public GhostPOIManagerPersister() : base("GhostPOIManager", ".poi", "POI")
        {
        }
    }
    #endregion
}