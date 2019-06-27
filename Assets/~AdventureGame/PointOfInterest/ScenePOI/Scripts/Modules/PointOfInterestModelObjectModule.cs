using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public class PointOfInterestModelObjectModule : APointOfInterestModule
    {
        #region Internal Dependencies
        private List<GameObject> oneLevelDownChildObjects;
        private Collider[] allColliders;
        private Animator animator;

        public Animator Animator { get => animator; }
        #endregion

        #region Internal Managers
        private POIShowHideManager POIShowHideManager;
        #endregion

        public override void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModules pointOfInteresetModules)
        {
            base.Init(pointOfInterestTypeRef, pointOfInteresetModules);

            var modelObject = this.pointOfInterestTypeRef.transform.parent.gameObject.FindChildObjectWithLevelLimit("Model", 0);

            this.allColliders = modelObject.GetComponentsInChildren<Collider>();
            this.oneLevelDownChildObjects = modelObject.FindOneLevelDownChilds();
            this.animator = modelObject.GetComponent<Animator>();
            if (this.animator == null)
            {
                this.animator = modelObject.GetComponentInChildren<Animator>();
            }
            this.POIShowHideManager = new POIShowHideManager(pointOfInterestTypeRef, pointOfInteresetModules.GetModule<PointOfInterestModelObjectModule>());
        }

        public override void Tick(float d)
        {
        }

        #region External Events
        public void SetActive(bool value)
        {
            foreach (var oneLevelDownChildObject in oneLevelDownChildObjects)
            {
                oneLevelDownChildObject.SetActive(value);
            }

            // this.gameObject.SetActive(value);
        }
        public void SetAllColliders(bool value)
        {
            if (this.allColliders != null)
            {
                foreach (var collider in this.allColliders)
                {
                    collider.enabled = value;
                }
            }
        }

        public void OnPOIInit()
        {
        }
        #endregion
    }

    class POIShowHideManager
    {
        #region External Dependencies
        private LevelManager LevelManager;
        private PointOfInterestType PointOfInterestTypeRef;
        private PointOfInterestModelObjectModule PointOfInterestModelObjectModule;
        #endregion

        private Collider poiCollider;

        public POIShowHideManager(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectModule PointOfInterestModelObjectModule)
        {
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
            this.PointOfInterestTypeRef = pointOfInterestTypeRef;
            this.poiCollider = pointOfInterestTypeRef.GetComponent<Collider>();
            this.PointOfInterestModelObjectModule = PointOfInterestModelObjectModule;
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
                this.PointOfInterestModelObjectModule.SetActive(true);
            }
        }

        private void Hide()
        {
            if (this.IsPOICanBeHideable())
            {
                this.PointOfInterestModelObjectModule.SetActive(false);
            }
        }


        private void DisablePhysicsInteraction()
        {
            if (this.IsPOICanBeHideable())
            {
                this.poiCollider.enabled = false;
                this.PointOfInterestModelObjectModule.SetAllColliders(false);
            }
        }

        private void EnablePhysicsInteraction()
        {
            if (this.IsPOICanBeHideable())
            {
                this.poiCollider.enabled = true;
                this.PointOfInterestModelObjectModule.SetAllColliders(true);
            }
        }

        private bool IsPOICanBeHideable()
        {
            return this.PointOfInterestModelObjectModule != null && !this.PointOfInterestTypeRef.PointOfInterestInherentData.IsAlwaysDisplayed;
        }
    }

}
