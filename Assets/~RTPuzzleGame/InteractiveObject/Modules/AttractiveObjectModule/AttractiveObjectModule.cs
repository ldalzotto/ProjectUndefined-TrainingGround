using UnityEngine;
using GameConfigurationID;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    public class AttractiveObjectModule : InteractiveObjectModule
    {

        public static InteractiveObjectType Instanciate(Vector3 worldPosition, Transform parent, AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, List<Type> exclusiveInitialEnabledModules)
        {
            InteractiveObjectType attractiveObject = null;
            if (parent != null)
            {
                attractiveObject = MonoBehaviour.Instantiate(attractiveObjectInherentConfigurationData.AssociatedInteractiveObjectType, parent);
            }
            else
            {
                attractiveObject = MonoBehaviour.Instantiate(attractiveObjectInherentConfigurationData.AssociatedInteractiveObjectType);
            }

            attractiveObject.transform.position = worldPosition;
            attractiveObject.Init(new InteractiveObjectInitializationObject() { AttractiveObjectInherentConfigurationData = attractiveObjectInherentConfigurationData }, exclusiveInitialEnabledModules);
            return attractiveObject;
        }

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

        #region Data Retrieval
        public ModelObjectModule GetModel()
        {
            return this.modelObjectModule;
        }
        public RangeTypeObject SphereRange { get => sphereRange; }
        #endregion

        public AttractiveObjectId AttractiveObjectId;
        private AttractiveObjectLifetimeTimer AttractiveObjectLifetimeTimer;

        public void Init(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, ModelObjectModule ModelObjectModule)
        {
            this.modelObjectModule = ModelObjectModule;
            this.sphereRange = GetComponentInChildren<RangeTypeObject>();
            this.sphereRange.Init(new RangeTypeObjectInitializer(), null);
            this.sphereRange.SetIsAttractiveObject();
          //  this.sphereRange.PopuplateSphereRangeData(attractiveObjectInherentConfigurationData.EffectRange, RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE, new RangeTypeObjectInitializer());
            this.AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(attractiveObjectInherentConfigurationData.EffectiveTime);
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectLifetimeTimer.Tick(d, timeAttenuationFactor);
        }

        #region Logical Conditions
        public bool IsAskingToBeDestroyed()
        {
            return this.AttractiveObjectLifetimeTimer.IsTimeOver();
        }
        #endregion

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.yellow;
            Handles.Label(transform.position + new Vector3(0, 3f, 0), AttractiveObjectId.ToString(), labelStyle);
            Gizmos.DrawIcon(transform.position + new Vector3(0, 5.5f, 0), "Gizmo_AttractiveObject", true);
#endif
        }

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
                    PuzzleGameConfigurationManager puzzleGameConfigurationManager, AttractiveObjectsInstanciatedParent AttractiveObjectsInstanciatedParent)
        {
            attractiveObject.transform.position = attractiveObjectWorldPositionHit.point;

            //TODO make the rotation relative to the player
            attractiveObject.transform.LookAt(attractiveObject.transform.position + Vector3.forward);
        }
    }
}
