using UnityEngine;
using GameConfigurationID;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    public class AttractiveObjectTypeModule : InteractiveObjectModule
    {

        public static InteractiveObjectType Instanciate(Vector3 worldPosition, Transform parent, AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData)
        {
            InteractiveObjectType attractiveObject = null;
            if (parent != null)
            {
                attractiveObject = MonoBehaviour.Instantiate(attractiveObjectInherentConfigurationData.AttractiveInteractiveObjectPrefab, parent);
            }
            else
            {
                attractiveObject = MonoBehaviour.Instantiate(attractiveObjectInherentConfigurationData.AttractiveInteractiveObjectPrefab);
            }

            attractiveObject.transform.position = worldPosition;
            attractiveObject.Init(attractiveObjectInherentConfigurationData);
            return attractiveObject;
        }

        public static AttractiveObjectTypeModule GetAttractiveObjectFromCollisionType(CollisionType collisionType)
        {
            var sphereRange = RangeType.RetrieveFromCollisionType(collisionType);
            if (sphereRange != null)
            {
                return sphereRange.GetComponentInParent<AttractiveObjectTypeModule>();
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
            this.sphereRange.Init(new RangeTypeObjectInitializer(sphereRadius: attractiveObjectInherentConfigurationData.EffectRange));

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
        public static void OnAttractiveObjectActionExecuted(RaycastHit attractiveObjectWorldPositionHit, AttractiveObjectId attractiveObjectId, 
                    PuzzleGameConfigurationManager puzzleGameConfigurationManager, AttractiveObjectsInstanciatedParent AttractiveObjectsInstanciatedParent)
        {
            var attractiveObjectInherentConfigurationData = puzzleGameConfigurationManager.AttractiveObjectsConfiguration()[attractiveObjectId];
            var instanciatedAttractiveObject = AttractiveObjectTypeModule.Instanciate(attractiveObjectWorldPositionHit.point, AttractiveObjectsInstanciatedParent.transform, attractiveObjectInherentConfigurationData);
            //TODO make the rotation relative to the player
            instanciatedAttractiveObject.transform.LookAt(instanciatedAttractiveObject.transform.position + Vector3.forward);
        }
    }
}
