using CoreGame;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdventureGame
{

    public class PointOfInterestType : APointOfInterestType
    {
        public PointOfInterestId PointOfInterestId;

        #region Internal Depencies
        private PointOfInterestScenarioState pointOfInterestScenarioState;
        private PointOfInterestModelState pointOfInterestModelState;
        private PointOfInterestContextDataContainer PointOfInterestContextData;

        private PointOfInterestInherentData PointOfInterestInherentData;
        #endregion

        #region External Dependencies
        private APointOfInterestEventManager PointOfInterestEventManager;
        private AdventureGameConfigurationManager AdventureGameConfigurationManager;
        #endregion

        private ContextActionSynchronizerManager ContextActionSynchronizerManager;
        private POIMeshRendererManager POIMeshRendererManager;

        public PointOfInterestScenarioState PointOfInterestScenarioState { get => pointOfInterestScenarioState; }
        public PointOfInterestModelState PointOfInterestModelState { get => pointOfInterestModelState; }

        #region Data Retrieval
        public float GetMaxDistanceToInteractWithPlayer()
        {
            return this.PointOfInterestInherentData.MaxDistanceToInteractWithPlayer;
        }
        public bool IsInteractionWithPlayerAllowed()
        {
            return this.PointOfInterestInherentData.InteractionWithPlayerAllowed;
        }
        #endregion

        public override void Init()
        {
            #region External Dependencies
            this.PointOfInterestEventManager = GameObject.FindObjectOfType<APointOfInterestEventManager>();
            this.AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
            #endregion

            this.ContextActionSynchronizerManager = new ContextActionSynchronizerManager();
            this.POIMeshRendererManager = new POIMeshRendererManager(GetRenderers(true));
            this.pointOfInterestScenarioState = new PointOfInterestScenarioState();
            this.PointOfInterestContextData = transform.parent.GetComponentInChildren<PointOfInterestContextDataContainer>();

            Debug.Log(MyLog.Format(this.PointOfInterestId.ToString()));
            this.PointOfInterestInherentData = this.AdventureGameConfigurationManager.POIConf()[this.PointOfInterestId];
        }

        public override void Init_EndOfFrame()
        {
            this.PointOfInterestEventManager.OnPOICreated(this);
        }

        private void OnDrawGizmos()
        {
            var labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.red;
#if UNITY_EDITOR
            Handles.Label(transform.position, PointOfInterestId.ToString(), labelStyle);
#endif

            Gizmos.DrawIcon(transform.position + new Vector3(0, 5f, 0), "Gizmo_POI", true);
        }

        #region Logical Conditions
        public bool IsElligibleToGiveItem(ItemID itemToGive)
        {
            return pointOfInterestScenarioState != null && pointOfInterestScenarioState.ReceivableItemsComponent != null && pointOfInterestScenarioState.ReceivableItemsComponent.IsElligible(itemToGive);
        }
        public bool IsInteractableWithItem(ItemID involvedItem)
        {
            return pointOfInterestScenarioState != null && pointOfInterestScenarioState.InteractableItemsComponent != null && pointOfInterestScenarioState.InteractableItemsComponent.IsElligible(involvedItem);
        }
        #endregion

        #region External Events
        public void SyncPOIFromGhostPOI(GhostPOI ghostPOI)
        {
            pointOfInterestScenarioState = ghostPOI.PointOfInterestScenarioState1;
            ContextActionSynchronizerManager = ghostPOI.ContextActionSynchronizerManager;
            pointOfInterestModelState = ghostPOI.PointOfInterestModelState;
            this.pointOfInterestModelState.SyncPointOfInterestModelState(POIMeshRendererManager.POIRenderers);
            if (ghostPOI.PointOfInterestModelState.IsDisabled)
            {
                PointOfInterestEventManager.DisablePOI(this);
            }
        }

        public void SyncPointOfInterestModelStateToGhostPOI(GhostPOI ghostPOI)
        {
            ghostPOI.PointOfInterestModelState = this.pointOfInterestModelState;
        }

        public override void OnPOIDisabled()
        {
            Debug.Log("Disabled By Player : " + PointOfInterestId.ToString());
            this.DisablePOI();
        }

        public override void OnPOIEnabled()
        {
            this.EnablePOI();
        }
        #endregion

        #region Prefab Data Retrieval
        public Renderer[] GetRenderers(bool includeInactives = false)
        {
            return transform.parent.GetComponentsInChildren<Renderer>(includeInactives);
        }
        public GameObject GetRootObject()
        {
            return transform.parent.gameObject;
        }
        #endregion

        public DiscussionTree GetAssociatedDiscussionTree()
        {
            return this.AdventureGameConfigurationManager.DiscussionTreeConf()[pointOfInterestScenarioState.DiscussionTreeID];
        }

        public PointOfInterestContextDataContainer GetContextData()
        {
            return PointOfInterestContextData;
        }

        internal List<AContextAction> GetContextActions()
        {
            return ContextActionSynchronizerManager.ContextActions;
        }

        private void DisablePOI()
        {
            pointOfInterestModelState.OnPOIDisabled();
            pointOfInterestModelState.SyncPointOfInterestModelState(POIMeshRendererManager.POIRenderers);
            this.gameObject.SetActive(false);
        }

        private void EnablePOI()
        {
            pointOfInterestModelState.OnPOIEnabled();
            pointOfInterestModelState.SyncPointOfInterestModelState(POIMeshRendererManager.POIRenderers);
            this.gameObject.SetActive(true);
        }
    }

    class POIMeshRendererManager
    {
        private Renderer[] pOIRenderers;

        public POIMeshRendererManager(Renderer[] pOIRenderers)
        {
            this.pOIRenderers = pOIRenderers;
        }

        public Renderer[] POIRenderers { get => pOIRenderers; }
    }
}