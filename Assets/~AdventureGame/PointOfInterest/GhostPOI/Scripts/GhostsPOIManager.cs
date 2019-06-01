using CoreGame;
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
    public class GhostsPOIManager : MonoBehaviour
    {

        private Dictionary<PointOfInterestId, GhostPOI> ghostPOIs = null;
        private GhostPOIManagerPersister GhostPOIManagerPersister;

        public Dictionary<PointOfInterestId, GhostPOI> GhostPOIs { get { return ghostPOIs; } }

        public void Init()
        {
            this.GhostPOIManagerPersister = new GhostPOIManagerPersister();
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
            foreach(var ghostPOI in this.ghostPOIs.Values)
            {
                ghostPOI.Init(this);
            }
        }

        public GhostPOI GetGhostPOI(PointOfInterestId pointOfInterestId)
        {
            return GhostPOIs[pointOfInterestId];
        }

        #region External Events
        public void OnScenePOICreated(PointOfInterestType pointOfInterestType)
        {
            pointOfInterestType.SyncCreateFromGhostPOI(GhostPOIs[pointOfInterestType.PointOfInterestId]);
            this.OnGhostPOIChanged();
        }
        public void OnScenePOIDestroyed(PointOfInterestType pointOfInterestType)
        {
            pointOfInterestType.SyncDestroyedFromGhostPOI(GhostPOIs[pointOfInterestType.PointOfInterestId]);
            this.OnGhostPOIChanged();
        }
        public void OnGhostPOIChanged()
        {
            this.GhostPOIManagerPersister.Save(this.ghostPOIs);
        }
        #endregion
    }

    [System.Serializable]
    public class GhostPOI
    {
        [SerializeField]
        private PointOfInterestScenarioState PointOfInterestScenarioState;
        [SerializeField]
        private ContextActionSynchronizerManager contextActionSynchronizerManager;
        [SerializeField]
        private PointOfInterestModelState pointOfInterestModelState;

        public PointOfInterestScenarioState PointOfInterestScenarioState1 { get => PointOfInterestScenarioState; }
        internal ContextActionSynchronizerManager ContextActionSynchronizerManager { get => contextActionSynchronizerManager; }
        public PointOfInterestModelState PointOfInterestModelState { get => pointOfInterestModelState; set => pointOfInterestModelState = value; }

        [NonSerialized]
        private GhostsPOIManager ghostsPOIManagerRef;

        public GhostPOI()
        {
            PointOfInterestScenarioState = new PointOfInterestScenarioState();
            contextActionSynchronizerManager = new ContextActionSynchronizerManager();
        }

        public void Init(GhostsPOIManager ghostsPOIManagerRef)
        {
            this.ghostsPOIManagerRef = ghostsPOIManagerRef;
        }

        public DiscussionTree GetAssociatedDiscussionTree()
        {
            return PointOfInterestScenarioState.DiscussionTree;
        }

        #region External Events
        public void OnContextActionAdd(ItemID itemID, AContextAction contextActionToAdd)
        {
            contextActionSynchronizerManager.OnContextActionAdd(itemID, contextActionToAdd);
            this.OnGhostPOIChanged();
        }
        public void OnGrabbableItemRemove(ItemID itemId)
        {
            contextActionSynchronizerManager.OnGrabbableItemRemoved(itemId);
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
        public void OnDiscussionTreeAdd(DiscussionTree discussionTree, AContextAction contextActionToAdd)
        {
            PointOfInterestScenarioState.DiscussionTree = discussionTree;
            contextActionSynchronizerManager.OnDiscussionTreeAdd(contextActionToAdd);
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
            contextActionSynchronizerManager.OnLevelTransitionAdd(contextActionToAdd);
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
        private Dictionary<string, AContextAction> contextActions = new Dictionary<string, AContextAction>();

        public List<AContextAction> ContextActions
        {
            get
            {
                return contextActions.Values.ToList();
            }
        }

        public void OnContextActionAdd(ItemID itemId, AContextAction contextActionToAdd)
        {
            var key = itemId.ToString();
            ContextActionAddSilently(contextActionToAdd, key);
        }

        public void OnGrabbableItemRemoved(ItemID itemID)
        {
            contextActions.Remove(itemID.ToString());
        }

        public void OnDiscussionTreeAdd(AContextAction contextActionToAdd)
        {
            var key = typeof(TalkAction).ToString();
            ContextActionAddSilently(contextActionToAdd, key);
        }

        public void OnLevelTransitionAdd(AContextAction contextActionToAdd)
        {
            var key = typeof(LevelZoneTransitionAction).ToString();
            ContextActionAddSilently(contextActionToAdd, key);
        }

        private void ContextActionAddSilently(AContextAction contextActionToAdd, string key)
        {
            if (contextActions.ContainsKey(key))
            {
                contextActions[key] = contextActionToAdd;
            }
            else
            {
                contextActions.Add(key, contextActionToAdd);
            }
        }

        public Dictionary<string, AContextAction> GetRawContextActions()
        {
            return contextActions;
        }

        public void ReplaceContextActions(Dictionary<string, AContextAction> contextActions)
        {
            this.contextActions = contextActions;
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