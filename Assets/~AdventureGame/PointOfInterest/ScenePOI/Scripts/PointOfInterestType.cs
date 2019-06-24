using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdventureGame
{

    public class PointOfInterestType : APointOfInterestType
    {
        public const string MODEL_OBJECT_NAME = "Model";

        public PointOfInterestId PointOfInterestId;

        #region Internal Depencies
        private PointOfInterestScenarioState pointOfInterestScenarioState;
        private PointOfInterestModelState pointOfInterestModelState;
        private PointOfInterestAnimationPositioningState pointOfInterestAnimationPositioningState;


        #region Optional
        private PointOfInterestCutsceneController pointOfInterestCutsceneController;
        private Animator pointOfInterestAnimator;
        #endregion

        private PointOfInterestInherentData pointOfInterestInherentData;
        #endregion

        #region External Dependencies
        private APointOfInterestEventManager PointOfInterestEventManager;
        private AdventureGameConfigurationManager AdventureGameConfigurationManager;
        private CoreConfigurationManager CoreConfigurationManager;
        #endregion

        #region Internal Managers
        private ContextActionSynchronizerManager ContextActionSynchronizerManager;
        private POIMeshRendererManager POIMeshRendererManager;
        private POIShowHideManager POIShowHideManager;
        #endregion
        public PointOfInterestScenarioState PointOfInterestScenarioState { get => pointOfInterestScenarioState; }
        public PointOfInterestModelState PointOfInterestModelState { get => pointOfInterestModelState; }
        public PointOfInterestInherentData PointOfInterestInherentData { get => pointOfInterestInherentData; }
        public PointOfInterestCutsceneController PointOfInterestCutsceneController { get => pointOfInterestCutsceneController; }

        #region Data Retrieval
        public float GetMaxDistanceToInteractWithPlayer()
        {
            return this.pointOfInterestInherentData.MaxDistanceToInteractWithPlayer;
        }
        public bool IsInteractionWithPlayerAllowed()
        {
            return this.pointOfInterestInherentData.InteractionWithPlayerAllowed;
        }
        #endregion

        public override void Init()
        {
            #region External Dependencies
            this.PointOfInterestEventManager = GameObject.FindObjectOfType<APointOfInterestEventManager>();
            this.AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
            this.CoreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();
            #endregion
            this.pointOfInterestCutsceneController = transform.parent.GetComponentInChildren<PointOfInterestCutsceneController>();
            var modelObject = transform.parent.gameObject.FindChildObjectRecursively(MODEL_OBJECT_NAME);
            if (modelObject != null)
            {
                this.pointOfInterestAnimator = modelObject.GetComponent<Animator>();
                if (this.pointOfInterestAnimator == null)
                {
                    this.pointOfInterestAnimator = modelObject.GetComponentInChildren<Animator>();
                }
            }

            this.ContextActionSynchronizerManager = new ContextActionSynchronizerManager();
            this.POIMeshRendererManager = new POIMeshRendererManager(GetRenderers(true));
            this.POIShowHideManager = new POIShowHideManager(this);
            this.pointOfInterestScenarioState = new PointOfInterestScenarioState();
            this.pointOfInterestAnimationPositioningState = new PointOfInterestAnimationPositioningState();

            Debug.Log(MyLog.Format(this.PointOfInterestId.ToString()));
            this.pointOfInterestInherentData = this.AdventureGameConfigurationManager.POIConf()[this.PointOfInterestId];

            if (this.pointOfInterestCutsceneController != null)
            {
                this.pointOfInterestCutsceneController.Init();
            }

        }

        public override void Init_EndOfFrame()
        {
            this.POIShowHideManager.OnPOIInit(this);
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

        public override void SetAnimationPosition(AnimationID animationID)
        {
            this.pointOfInterestAnimationPositioningState.SyncPointOfInterestAnimationPositioningState(animationID, ref this.pointOfInterestAnimator, this.CoreConfigurationManager.AnimationConfiguration());
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

    class POIShowHideManager
    {
        #region External Dependencies
        private LevelManager LevelManager;
        #endregion

        private GameObject poiModelObject;
        private Collider poiCollider;
        private Collider[] modelColliders;

        public POIShowHideManager(PointOfInterestType pointOfInterestTypeRef)
        {
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
            this.poiModelObject = pointOfInterestTypeRef.transform.parent.gameObject.FindChildObjectRecursively("Model");
            if (this.poiModelObject != null)
            {
                this.modelColliders = this.poiModelObject.GetComponentsInChildren<Collider>();
            }
            this.poiCollider = pointOfInterestTypeRef.GetComponent<Collider>();
        }

        public void OnPOIInit(PointOfInterestType pointOfInterestTypeRef)
        {
            if (this.LevelManager.CurrentLevelType == LevelType.PUZZLE)
            {
                if (!pointOfInterestTypeRef.PointOfInterestInherentData.IsPersistantToPuzzle)
                {
                    this.Hide();
                }
                this.DisablePhysicsInteraction();
            }
            else
            {
                if (!pointOfInterestTypeRef.PointOfInterestInherentData.IsPersistantToPuzzle)
                {
                    this.Show();
                }
                this.EnablePhysicsInteraction();
            }
        }


        private void Show()
        {
            if (this.IsADisplayedPOI())
            {
                this.poiModelObject.SetActive(true);
            }
        }

        private void Hide()
        {
            if (this.IsADisplayedPOI())
            {
                this.poiModelObject.SetActive(false);
            }
        }


        private void DisablePhysicsInteraction()
        {
            if (this.IsADisplayedPOI())
            {
                this.poiCollider.enabled = false;
                if (this.modelColliders != null)
                {
                    foreach (var modelCollider in this.modelColliders)
                    {
                        modelCollider.enabled = false;
                    }
                }
            }
        }

        private void EnablePhysicsInteraction()
        {
            if (this.IsADisplayedPOI())
            {
                this.poiCollider.enabled = true;
                if (this.modelColliders != null)
                {
                    foreach (var modelCollider in this.modelColliders)
                    {
                        modelCollider.enabled = true;
                    }
                }
            }
        }

        private bool IsADisplayedPOI()
        {
            return this.poiModelObject != null;
        }
    }
}