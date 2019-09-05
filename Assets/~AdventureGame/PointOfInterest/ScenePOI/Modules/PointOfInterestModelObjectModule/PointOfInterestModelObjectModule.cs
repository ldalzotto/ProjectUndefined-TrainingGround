using CoreGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdventureGame
{

    public class PointOfInterestModelObjectModule : APointOfInterestModule, IRendererRetrievable
    {
        #region Internal Dependencies
        private List<GameObject> oneLevelDownChildObjects;
        private Collider[] allColliders;
        private List<Renderer> renderers;
        private Animator animator;
        private PointOfInterestType pointOfInterestTypeRef;

        public Animator Animator { get => animator; }
        #endregion

        #region Internal Managers
        private POIShowHideManager POIShowHideManager;
        #endregion

        #region Internal State
        private ExtendedBounds averageModeBounds;
        public ExtendedBounds AverageModeBounds { get => averageModeBounds; }
        #endregion

        #region Data Retrieval
        public List<Renderer> GetAllRenderers()
        {
            return this.renderers;
        }
        #endregion

        public void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectModule PointOfInterestModelObjectModule)
        {
            // This means that the POI has been persisted across scenes
            // We reactivate POI to let ghost POI manager sync re disable it
            if (this.POIShowHideManager != null)
            {
                this.POIShowHideManager.ShowModelAndEnablePhysics();
            }

            this.pointOfInterestTypeRef = pointOfInterestTypeRef;

            this.allColliders = this.GetComponentsInChildren<Collider>();
            this.renderers = this.GetComponentsInChildren<Renderer>().ToList();
            this.oneLevelDownChildObjects = this.gameObject.FindOneLevelDownChilds();
            this.animator = this.GetComponent<Animator>();
            if (this.animator == null)
            {
                this.animator = this.GetComponentInChildren<Animator>();
            }
            this.POIShowHideManager = new POIShowHideManager(pointOfInterestTypeRef, PointOfInterestModelObjectModule);
            this.InitAnimation();

            this.averageModeBounds = BoundsHelper.GetAverageRendererBounds(this.GetComponentsInChildren<Renderer>());
            this.transform.localScale = Vector3.one;
            this.transform.transform.localPosition = Vector3.zero;
        }

        private void InitAnimation()
        {
            if (this.animator != null)
            {
                var animationConfiguration = this.pointOfInterestTypeRef.GetCoreConfigurationManager().AnimationConfiguration();
                GenericAnimatorHelper.SetMovementLayer(this.animator, animationConfiguration, LevelType.ADVENTURE);
            }
        }

        public void OnPOIInit()
        {
            this.POIShowHideManager.OnPOIInit(this.pointOfInterestTypeRef);
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
        public void SetModelLocalScaleRelativeTo(Vector3 localScale, Vector3 localScalePoint)
        {
            var initialOrigin = Vector3.zero;
            this.transform.localPosition =
                new Vector3()
                {
                    x = initialOrigin.x + (localScalePoint.x * ((localScale.x * -1) + 1)),
                    y = localScalePoint.y * ((localScale.y * -1) + 1),
                    z = localScalePoint.z * ((localScale.z * -1) + 1)
                };
            this.transform.localScale = localScale;
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

        public void ShowModelAndEnablePhysics()
        {
            this.Show();
            this.EnablePhysicsInteraction();
        }
    }

}
