using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdventureGame
{

    public class PointOfInterestType : APointOfInterestType, IVisualMovementPermission
    {
        [CustomEnum(ConfigurationType = typeof(PointOfInterestDefinitionConfiguration))]
        public PointOfInterestDefinitionID PointOfInterestDefinitionID;

        #region Ghost POI Persistance States
        private PointOfInterestScenarioState pointOfInterestScenarioState;
        private PointOfInterestModelState pointOfInterestModelState;
        private PointOfInterestAnimationPositioningState pointOfInterestAnimationPositioningState;
        private PointOfInterestLevelPositioningState pointOfInterestLevelPositioningState;
        #endregion

        #region Internal State
        private bool hasBeenDefined = false;
        private PointOfInterestDefinitionInherentData pointOfInterestDefinitionInherentData;
        #endregion

        #region Modules
        private PointOfInterestModules PointOfInteresetModules;
        private PointOfInterestModulesEventManager PointOfInterestModulesEventManager;
        #endregion

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
        public PointOfInterestDefinitionInherentData PointOfInterestDefinitionInherentData { get => pointOfInterestDefinitionInherentData; }

        #region Data Retrieval
        public bool IsInteractionWithPlayerAllowed()
        {
            return this.pointOfInterestDefinitionInherentData.PointOfInterestSharedDataTypeInherentData.InteractionWithPlayerAllowed;
        }
        public AbstractCutsceneController GetPointOfInterestCutsceneController()
        {
            return this.PointOfInteresetModules.PointOfInterestCutsceneController.GetCutsceneController();
        }
        public PointOfInterestModelObjectModule GetPointOfInterestModelObject()
        {
            return this.PointOfInteresetModules.PointOfInterestModelObjectModule;
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

        public static PointOfInterestType Instanciate(PointOfInterestDefinitionID PointOfInterestDefinitionID)
        {
            var AdventureGameStaticConfiguration = GameObject.FindObjectOfType<AdventureStaticConfigurationContainer>().AdventureStaticConfiguration;
            var pointOfInterestType = MonoBehaviour.Instantiate(AdventureGameStaticConfiguration.AdventurePrefabConfiguration.BasePointOfInterestType, GameObject.FindObjectOfType<LevelManager>().transform);
            pointOfInterestType.PointOfInterestDefinitionID = PointOfInterestDefinitionID;
            pointOfInterestType.Init();
            return pointOfInterestType;
        }

        public static void InstanciateNow(PointOfInterestDefinitionID PointOfInterestDefinitionID)
        {
            var pointOfInterestTypeInstanciated = Instanciate(PointOfInterestDefinitionID);
            pointOfInterestTypeInstanciated.Init_EndOfFrame();
        }

        public void DestroyWithoutSync()
        {
            this.PointOfInterestEventManager.OnPOIDestroyed(this);
            MonoBehaviour.Destroy(this.GetRootObject());
        }

        public override void Init()
        {
            Debug.Log(MyLog.Format(this.PointOfInterestId.ToString()));

            #region External Dependencies
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
            this.PointOfInterestEventManager = GameObject.FindObjectOfType<APointOfInterestEventManager>();
            this.AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
            this.CoreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();
            var adventureStaticConfiguration = GameObject.FindObjectOfType<AdventureStaticConfigurationContainer>().AdventureStaticConfiguration;
            #endregion

            if (this.PointOfInterestDefinitionID != PointOfInterestDefinitionID.NONE && !this.hasBeenDefined)
            {
                this.pointOfInterestDefinitionInherentData = this.AdventureGameConfigurationManager.PointOfInterestDefinitionConfiguration()[this.PointOfInterestDefinitionID];
                this.pointOfInterestDefinitionInherentData.DefinePointOfInterest(this, adventureStaticConfiguration.AdventurePrefabConfiguration);
                this.hasBeenDefined = true;
            }
            
            this.PointOfInteresetModules = new PointOfInterestModules(this);
            this.PointOfInterestModulesEventManager = new PointOfInterestModulesEventManager(this.PointOfInteresetModules);

            this.ContextActionSynchronizerManager = new ContextActionSynchronizerManager();
            this.POIMeshRendererManager = new POIMeshRendererManager(GetRenderers(true));
            this.pointOfInterestScenarioState = new PointOfInterestScenarioState();
            this.pointOfInterestAnimationPositioningState = new PointOfInterestAnimationPositioningState();
            this.pointOfInterestLevelPositioningState = new PointOfInterestLevelPositioningState();

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
            return (this.PointOfInteresetModules.PointOfInterestCutsceneController != null
                                && this.PointOfInteresetModules.PointOfInterestCutsceneController.GetCutsceneController() != null
                                && !this.PointOfInteresetModules.PointOfInterestCutsceneController.GetCutsceneController().IsAnimationPlaying)
                    || this.PointOfInteresetModules.PointOfInterestCutsceneController == null;
        }
        public bool IsDirectedByCutscene()
        {
            if (this.PointOfInteresetModules.PointOfInterestCutsceneController != null)
            {
                return this.PointOfInteresetModules.PointOfInterestCutsceneController.IsDirectedByCutscene();
            }
            return false;
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

            if (ghostPOI.PointOfInterestLevelPositioningState != null && this.LevelManager.AllLoadedLevelZonesChunkID.Contains(ghostPOI.PointOfInterestLevelPositioningState.LevelZoneChunkID))
            {
                PointOfInterestEventManager.SetPosition(this, ghostPOI.PointOfInterestLevelPositioningState.TransformBinarry.Format());
            }
        }

        public void SynchIndentificationStateToGhostPOI(GhostPOI ghostPOI)
        {
            ghostPOI.PointOfInterestIdentificationState.PointOfInterestDefinitionID = this.PointOfInterestDefinitionID;
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
            return this.GetComponentsInChildren<Renderer>(includeInactives);
        }
        public GameObject GetRootObject()
        {
            return this.gameObject;
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