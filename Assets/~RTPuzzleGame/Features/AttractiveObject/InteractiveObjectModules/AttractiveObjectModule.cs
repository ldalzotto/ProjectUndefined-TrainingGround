using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface IAttractiveObjectModuleDataRetriever
    {
        ModelObjectModule GetModelObjectModule();
        Transform GetTransform();
    }

    public class AttractiveObjectModule : InteractiveObjectModule, IAttractiveObjectModuleDataRetriever
    {

        public static AttractiveObjectModule GetAttractiveObjectFromCollisionType(CollisionType collisionType)
        {
            var sphereRange = RangeType.RetrieveFromCollisionType(collisionType);
            if (sphereRange != null)
            {
                return sphereRange.GetComponentInParent<AttractiveObjectModule>();
            }
            return null;
        }

        #region ModuleDependencies
        private ModelObjectModule modelObjectModule;
        #endregion

        #region Internal Dependencies
        private RangeTypeObject sphereRange;
        #endregion

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        #region Data Retrieval
        public RangeTypeObject SphereRange { get => sphereRange; }
        #endregion

        #region IAttractiveObjectModuleDataRetriever
        public ModelObjectModule GetModelObjectModule()
        {
            return this.modelObjectModule;
        }
        public Transform GetTransform() { return this.transform; }
        #endregion

        public AttractiveObjectId AttractiveObjectId;
        private AttractiveObjectLifetimeTimer AttractiveObjectLifetimeTimer;

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            AttractiveObjectInherentConfigurationData AttractiveObjectInherentConfigurationData = interactiveObjectInitializationObject.AttractiveObjectInherentConfigurationData;

            if (interactiveObjectInitializationObject.AttractiveObjectInherentConfigurationData == null)
            {
                AttractiveObjectInherentConfigurationData = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[this.AttractiveObjectId];
            } 

            this.modelObjectModule = interactiveObjectType.GetModule<ModelObjectModule>();
            this.sphereRange = GetComponentInChildren<RangeTypeObject>();
            this.sphereRange.Init(new RangeTypeObjectInitializer(), null);
            this.sphereRange.SetIsAttractiveObject();
            this.AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(AttractiveObjectInherentConfigurationData.EffectiveTime);
            this.PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectLifetimeTimer.Tick(d, timeAttenuationFactor);
        }

        public override void OnInteractiveObjectDestroyed()
        {
            this.sphereRange.OnRangeDestroyed();
            this.PuzzleEventsManager.PZ_EVT_AttractiveObject_TpeDestroyed(this);
        }

        #region Logical Conditions
        public bool IsAskingToBeDestroyed()
        {
            return this.AttractiveObjectLifetimeTimer.IsTimeOver();
        }
        #endregion

        public static class AttractiveObjectModuleInstancer
        {
            public static void PopuplateFromDefinition(AttractiveObjectModule attractiveObjectModule, AttractiveObjectModuleDefinition attractiveObjectModuleDefinition,
                        PuzzlePrefabConfiguration puzzlePrefabConfiguration, PuzzleGameConfiguration puzzleGameConfiguration)
            {
                attractiveObjectModule.AttractiveObjectId = attractiveObjectModuleDefinition.AttractiveObjectId;
                var RangeTypeObject = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseRangeTypeObject, attractiveObjectModule.transform);
                new RangeTypeObjectDefinitionInherentData()
                {
                    RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                    {
                        { typeof(RangeTypeDefinition),  new RangeTypeDefinition()
                            {
                                RangeTypeID = RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
                                RangeShapeConfiguration = new SphereRangeShapeConfiguration()
                                {
                                    Radius = puzzleGameConfiguration.AttractiveObjectConfiguration.ConfigurationInherentData[attractiveObjectModule.AttractiveObjectId].EffectRange
                                }
                            }
                        },
                        {typeof(RangeObstacleListenerDefinition), new RangeObstacleListenerDefinition() }
                    },
                    RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                    {
                        {typeof(RangeTypeDefinition), true },
                        {typeof(RangeObstacleListenerDefinition), true }
                    }
                }.DefineRangeTypeObject(RangeTypeObject, puzzlePrefabConfiguration);
                RangeTypeObject.RangeTypeObjectDefinitionID = RangeTypeObjectDefinitionID.NONE;
            }
        }
    }

    class AttractiveObjectLifetimeTimer
    {
        private float effectiveTime;

        public AttractiveObjectLifetimeTimer(float effectiveTime)
        {
            this.effectiveTime = effectiveTime;
        }

        private float elapsedTime;

        #region Logical Condition
        public bool IsTimeOver()
        {
            return elapsedTime >= effectiveTime;
        }
        #endregion

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.elapsedTime += (d * timeAttenuationFactor);
        }

    }

    public static class AttractiveObjectTypeModuleEventHandling
    {
        public static void OnAttractiveObjectActionExecuted(RaycastHit attractiveObjectWorldPositionHit, InteractiveObjectType attractiveObject,
                    PuzzleGameConfigurationManager puzzleGameConfigurationManager)
        {
            attractiveObject.transform.position = attractiveObjectWorldPositionHit.point;

            //TODO make the rotation relative to the player
            attractiveObject.transform.LookAt(attractiveObject.transform.position + Vector3.forward);
        }
    }
}
