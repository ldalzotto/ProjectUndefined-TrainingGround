using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdventureGame
{

    public class PointOfInterestType : APointOfInterestType
    {
        public PointOfInterestId PointOfInterestId;

        #region Internal Depencies
        private PointOfInterestModelObjectType PointOfInterestModelObjectType;

        private PointOfInterestScenarioState pointOfInterestScenarioState;
        private PointOfInterestModelState pointOfInterestModelState;
        private PointOfInterestAnimationPositioningState pointOfInterestAnimationPositioningState;


        #region Optional
        private PointOfInterestCutsceneController pointOfInterestCutsceneController;
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

            this.pointOfInterestInherentData = this.AdventureGameConfigurationManager.POIConf()[this.PointOfInterestId];

            this.PointOfInterestModelObjectType = transform.parent.GetComponentInChildren<PointOfInterestModelObjectType>();
            if (this.PointOfInterestModelObjectType != null)
            {
                this.PointOfInterestModelObjectType.Init();
            }

            this.ContextActionSynchronizerManager = new ContextActionSynchronizerManager();
            this.POIMeshRendererManager = new POIMeshRendererManager(GetRenderers(true));
            this.POIShowHideManager = new POIShowHideManager(this, this.PointOfInterestModelObjectType, this.IsGenericPOI);
            this.pointOfInterestScenarioState = new PointOfInterestScenarioState();
            this.pointOfInterestAnimationPositioningState = new PointOfInterestAnimationPositioningState();

            Debug.Log(MyLog.Format(this.PointOfInterestId.ToString()));

            this.pointOfInterestCutsceneController = transform.parent.GetComponentInChildren<PointOfInterestCutsceneController>();
            if (this.pointOfInterestCutsceneController != null)
            {
                this.pointOfInterestCutsceneController.Init(this.PointOfInterestModelObjectType);
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

        //Is the POI visible on scene == not player and not item
        private bool IsGenericPOI()
        {
            return this.PointOfInterestId != PointOfInterestId.PLAYER && !this.pointOfInterestInherentData.IsItem;
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
            Debug.Log(MyLog.Format(this.PointOfInterestId.ToString()));
            this.pointOfInterestAnimationPositioningState.SyncPointOfInterestAnimationPositioningState(animationID, ref this.PointOfInterestModelObjectType, this.CoreConfigurationManager.AnimationConfiguration());
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
        private PointOfInterestModelObjectType pointOfInterestModelObject;
        #endregion

        private Func<bool> IsGenericPOI;
        private Collider poiCollider;

        public POIShowHideManager(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectType pointOfInterestModelObject, Func<bool> IsGenericPOI)
        {
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
            this.poiCollider = pointOfInterestTypeRef.GetComponent<Collider>();
            this.pointOfInterestModelObject = pointOfInterestModelObject;
            this.IsGenericPOI = IsGenericPOI;
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
            if (this.IsPOICanBeHideable())
            {
                this.pointOfInterestModelObject.SetActive(true);
            }
        }

        private void Hide()
        {
            if (this.IsPOICanBeHideable())
            {
                this.pointOfInterestModelObject.SetActive(false);
            }
        }


        private void DisablePhysicsInteraction()
        {
            if (this.IsPOICanBeHideable())
            {
                this.poiCollider.enabled = false;
                this.pointOfInterestModelObject.SetAllColliders(false);
            }
        }

        private void EnablePhysicsInteraction()
        {
            if (this.IsPOICanBeHideable())
            {
                this.poiCollider.enabled = true;
                this.pointOfInterestModelObject.SetAllColliders(true);
            }
        }

        private bool IsPOICanBeHideable()
        {
            return this.pointOfInterestModelObject != null && this.IsGenericPOI.Invoke();
        }
    }
}