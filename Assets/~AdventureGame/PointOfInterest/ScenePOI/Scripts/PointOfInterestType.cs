using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdventureGame
{

    public class PointOfInterestType : MonoBehaviour
    {
        public PointOfInterestId PointOfInterestId;

        #region Internal Depencies
        private PointOfInterestScenarioState pointOfInterestScenarioState;
        private PointOfInterestModelState pointOfInterestModelState;
        private PointOfInterestContextDataContainer PointOfInterestContextData;

        private PointOfInterestInherentData PointOfInterestInherentData;
        #endregion

        #region External Dependencies
        private PointOfInterestEventManager PointOfInterestEventManager;
        #endregion

        private ContextActionSynchronizerManager ContextActionSynchronizerManager;
        private POIMeshRendererManager POIMeshRendererManager;

        public PointOfInterestScenarioState PointOfInterestScenarioState { get => pointOfInterestScenarioState; }

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

        public void Init()
        {
            #region External Dependencies
            var AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
            #endregion
            this.ContextActionSynchronizerManager = new ContextActionSynchronizerManager();
            this.POIMeshRendererManager = new POIMeshRendererManager(GetRenderers(true));
            this.pointOfInterestScenarioState = new PointOfInterestScenarioState();
            this.PointOfInterestContextData = GetComponentInChildren<PointOfInterestContextDataContainer>();

            Debug.Log(MyLog.Format(this.PointOfInterestId.ToString()));
            this.PointOfInterestInherentData = AdventureGameConfigurationManager.POIConf()[this.PointOfInterestId];
        }

        public void Init_EndOfFrame()
        {
            #region External Dependencies
            PointOfInterestEventManager = GameObject.FindObjectOfType<PointOfInterestEventManager>();
            #endregion
         
            PointOfInterestEventManager.OnPOICreated(this);
        }

        private void OnDrawGizmos()
        {
            var labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.red;
#if UNITY_EDITOR
            Handles.Label(transform.position, PointOfInterestId.ToString(), labelStyle);
#endif

            Gizmos.DrawIcon(transform.position + new Vector3(0, 1.5f, 0), "Gizmo_POI", true);
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
        public void SyncCreateFromGhostPOI(GhostPOI ghostPOI)
        {
            pointOfInterestScenarioState = ghostPOI.PointOfInterestScenarioState1;
            ContextActionSynchronizerManager = ghostPOI.ContextActionSynchronizerManager;
            if (ghostPOI.PointOfInterestModelState == null)
            {
                this.pointOfInterestModelState = new PointOfInterestModelState(POIMeshRendererManager.POIRenderers);
                ghostPOI.PointOfInterestModelState = this.pointOfInterestModelState;
            }
            else
            {

                this.pointOfInterestModelState = ghostPOI.PointOfInterestModelState;
                this.pointOfInterestModelState.SyncScenePOIRenderers(POIMeshRendererManager.POIRenderers);
                if (ghostPOI.PointOfInterestModelState.IsDestroyed)
                {
                    PointOfInterestEventManager.DestroyPOI(this);
                }
            }
        }
        public void SyncDestroyedFromGhostPOI(GhostPOI ghostPOI)
        {
            this.pointOfInterestModelState.SynchGhostPOIRenderers(POIMeshRendererManager.POIRenderers);
            ghostPOI.PointOfInterestModelState = this.pointOfInterestModelState;
        }
        public void OnPOIDestroyedFromPlayerAction()
        {
            Debug.Log("Destroyed By Player : " + PointOfInterestId.ToString());
            pointOfInterestModelState.IsDestroyed = true;
        }
        #endregion

        #region Prefab Data Retrieval
        public Renderer[] GetRenderers(bool includeInactives = false)
        {
            var parentObject = transform.parent;
            return parentObject.GetComponentsInChildren<Renderer>(includeInactives);
        }
        public GameObject GetRootObject()
        {
            return transform.parent.gameObject;
        }
        #endregion

        public DiscussionTree GetAssociatedDiscussionTree()
        {
            return pointOfInterestScenarioState.DiscussionTree;
        }

        public PointOfInterestContextDataContainer GetContextData()
        {
            return PointOfInterestContextData;
        }

        internal List<AContextAction> GetContextActions()
        {
            return ContextActionSynchronizerManager.ContextActions;
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