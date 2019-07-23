using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdventureGame
{

    public class PointOfInterestType : APointOfInterestType, IVisualMovementPermission
    {
        #region Ghost POI Persistance States
        private PointOfInterestScenarioState pointOfInterestScenarioState;
        private PointOfInterestModelState pointOfInterestModelState;
        private PointOfInterestAnimationPositioningState pointOfInterestAnimationPositioningState;
        private PointOfInterestLevelPositioningState pointOfInterestLevelPositioningState;
        #endregion

        #region Modules
        private PointOfInterestModules PointOfInteresetModules;
        private PointOfInterestModulesEventManager PointOfInterestModulesEventManager;
        #endregion

        #region Data Components
        private DataComponentContainer pOIDataComponentContainer;
        #endregion

        private PointOfInterestInherentData pointOfInterestInherentData;

        #region External Dependencies
        private LevelManager LevelManager;
        private APointOfInterestEventManager PointOfInterestEventManager;
        private AdventureGameConfigurationManager AdventureGameConfigurationManager;
        private CoreConfigurationManager CoreConfigurationManager;
        #endregion

        #region Internal Managers
        private ContextActionSynchronizerManager ContextActionSynchronizerManager;
        private POIMeshRendererManager POIMeshRendererManager;
        #endregion
        public PointOfInterestScenarioState PointOfInterestScenarioState { get => pointOfInterestScenarioState; }
        public PointOfInterestModelState PointOfInterestModelState { get => pointOfInterestModelState; }
        public PointOfInterestInherentData PointOfInterestInherentData { get => pointOfInterestInherentData; }
        public DataComponentContainer POIDataComponentContainer { get => pOIDataComponentContainer; }

        #region Data Retrieval
        public float GetMaxDistanceToInteractWithPlayer()
        {
            return this.pointOfInterestInherentData.MaxDistanceToInteract;
        }
        public bool IsInteractionWithPlayerAllowed()
        {
            return this.pointOfInterestInherentData.InteractionWithPlayerAllowed;
        }
        public PointOfInterestCutsceneController GetPointOfInterestCutsceneController()
        {
            return this.PointOfInteresetModules.PointOfInterestCutsceneController;
        }
        public PointOfInterestTrackerModule GetPointOfInterestTrackerModule()
        {
            return this.PointOfInteresetModules.PointOfInterestTrackerModule;
        }
        public CoreConfigurationManager GetCoreConfigurationManager()
        {
            return this.CoreConfigurationManager;
        }

        #endregion

        public static PointOfInterestType Instanciate(PointOfInterestId pointOfInterestId)
        {
            var poiInstanciated = MonoBehaviour.Instantiate(GameObject.FindObjectOfType<AdventureGameConfigurationManager>().POIConf()[pointOfInterestId].PointOfInterestPrefab, GameObject.FindObjectOfType<LevelManager>().transform);
            var pointOfInterestTypeInstanciated = poiInstanciated.GetComponentInChildren<PointOfInterestType>();
            pointOfInterestTypeInstanciated.Init();
            return pointOfInterestTypeInstanciated;
        }

        public static void InstanciateNow(PointOfInterestId pointOfInterestId)
        {
            var pointOfInterestTypeInstanciated = Instanciate(pointOfInterestId);
            pointOfInterestTypeInstanciated.Init_EndOfFrame();
        }

        public void DestroyWithoutSync()
        {
            this.PointOfInterestEventManager.OnPOIDestroyed(this);
            MonoBehaviour.Destroy(this.GetRootObject());
        }

        public override void Init()
        {
            #region External Dependencies
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
            this.PointOfInterestEventManager = GameObject.FindObjectOfType<APointOfInterestEventManager>();
            this.AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
            this.CoreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();
            #endregion

            this.pointOfInterestInherentData = this.AdventureGameConfigurationManager.POIConf()[this.PointOfInterestId];

            this.pOIDataComponentContainer = this.transform.parent.GetComponentInChildren<DataComponentContainer>();
            this.pOIDataComponentContainer.Init();

            this.PointOfInteresetModules = transform.parent.GetComponentInChildren<PointOfInterestModules>();
            this.PointOfInteresetModules.Init(this);
            this.PointOfInterestModulesEventManager = new PointOfInterestModulesEventManager(this.PointOfInteresetModules);

            this.ContextActionSynchronizerManager = new ContextActionSynchronizerManager();
            this.POIMeshRendererManager = new POIMeshRendererManager(GetRenderers(true));
            this.pointOfInterestScenarioState = new PointOfInterestScenarioState();
            this.pointOfInterestAnimationPositioningState = new PointOfInterestAnimationPositioningState();
            this.pointOfInterestLevelPositioningState = new PointOfInterestLevelPositioningState();

            Debug.Log(MyLog.Format(this.PointOfInterestId.ToString()));
        }

        public override void Init_EndOfFrame()
        {
            this.PointOfInterestModulesEventManager.OnPOIInit();
            this.PointOfInterestEventManager.OnPOICreated(this);
        }

        public override void Tick(float d)
        {
            this.PointOfInteresetModules.Tick(d);
        }

        public void LateTick(float d)
        {
            this.PointOfInteresetModules.LateTick(d);
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
        public bool IsPlayer()
        {
            return this.PointOfInterestId == PointOfInterestId.PLAYER;
        }
        public bool IsVisualMovementAllowed()
        {
            return (this.PointOfInteresetModules.PointOfInterestCutsceneController != null && !this.PointOfInteresetModules.PointOfInterestCutsceneController.IsAnimationPlaying)
                    || this.PointOfInteresetModules.PointOfInterestCutsceneController == null;
        }
        #endregion

        #region External Events
        public void SyncPOIFromGhostPOI(GhostPOI ghostPOI)
        {
            pointOfInterestScenarioState = ghostPOI.PointOfInterestScenarioState1;
            ContextActionSynchronizerManager = ghostPOI.ContextActionSynchronizerManager;
            pointOfInterestModelState = ghostPOI.PointOfInterestModelState;
            this.pointOfInterestModelState.SyncPointOfInterestModelState(POIMeshRendererManager.POIRenderers);
            //disabling state
            if (ghostPOI.PointOfInterestModelState.IsDisabled)
            {
                PointOfInterestEventManager.DisablePOI(this);
            }
            //animation positioning state
            if (ghostPOI.PointOfInterestAnimationPositioningState != null && ghostPOI.PointOfInterestAnimationPositioningState.LastPlayedAnimation != AnimationID.NONE)
            {
                PointOfInterestEventManager.SetAnimationPosition(ghostPOI.PointOfInterestAnimationPositioningState.LastPlayedAnimation, this);
            }
            
            if (ghostPOI.PointOfInterestLevelPositioningState != null && ghostPOI.PointOfInterestLevelPositioningState.LevelZoneChunkID == this.LevelManager.CurrentLevelZoneChunkID)
            {
                PointOfInterestEventManager.SetPosition(this, ghostPOI.PointOfInterestLevelPositioningState.TransformBinarry.Format());
            }
        }

        public void SyncPointOfInterestModelStateToGhostPOI(GhostPOI ghostPOI)
        {
            ghostPOI.PointOfInterestModelState = this.pointOfInterestModelState;
        }

        public override void OnPOIDisabled(APointOfInterestType disabledPointOfInterest)
        {
            if (this.PointOfInterestId == disabledPointOfInterest.PointOfInterestId)
            {
                Debug.Log("Disabled By Player : " + PointOfInterestId.ToString());
                this.DisablePOI();
            }
            this.PointOfInterestModulesEventManager.OnPOIDisabled(disabledPointOfInterest);
        }

        public override void OnPOIDestroyed(APointOfInterestType poiToBeDestroyed)
        {
            this.PointOfInterestModulesEventManager.OnPOIDisabled(poiToBeDestroyed);
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
            pointOfInterestModelState.SyncPointOfInterestModelState(POIMeshRendererManager.POIRenderers);
            this.gameObject.SetActive(true);
        }

        public override void SetAnimationPosition(AnimationID animationID)
        {
            Debug.Log(MyLog.Format(this.PointOfInterestId.ToString()));
            var PointOfInterestModelObjectModule = this.PointOfInteresetModules.PointOfInterestModelObjectModule;
            this.pointOfInterestAnimationPositioningState.SyncPointOfInterestAnimationPositioningState(animationID, ref PointOfInterestModelObjectModule, this.CoreConfigurationManager.AnimationConfiguration());
        }

        public override void SetPosition(TransformBinarryFormatted position)
        {
            this.GetRootObject().transform.position = position.position;
            this.GetRootObject().transform.rotation = position.rotation;
            this.GetRootObject().transform.localScale = position.localScale;
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