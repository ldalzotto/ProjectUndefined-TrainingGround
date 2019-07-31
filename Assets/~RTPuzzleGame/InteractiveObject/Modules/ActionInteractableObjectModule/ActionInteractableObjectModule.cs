using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class ActionInteractableObjectModule : InteractiveObjectModule
    {
        [CustomEnum()]
        public ActionInteractableObjectID ActionInteractableObjectID;

        #region Module Dependencies
        #endregion

        private ActionInteractableObjectInherentData ActionInteractableObjectInherentData;
        
        public static InteractiveObjectType Instanciate(Vector3 worldPosition, ActionInteractableObjectInherentData ActionInteractableObjectInherentData)
        {
            InteractiveObjectType createdDisarmObject = MonoBehaviour.Instantiate(ActionInteractableObjectInherentData.ActionInteractableObjectPrefab);
            createdDisarmObject.Init(new InteractiveObjectInitializationObject(ActionInteractableObjectInherentData: ActionInteractableObjectInherentData));
            createdDisarmObject.transform.position = worldPosition;
            return createdDisarmObject;
        }

        public void Init(ActionInteractableObjectInherentData ActionInteractableObjectInherentData)
        {
            this.ActionInteractableObjectInherentData = ActionInteractableObjectInherentData;

            var triggerCollider = GetComponent<SphereCollider>();
            triggerCollider.radius = this.ActionInteractableObjectInherentData.InteractionRange;
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsPlayer)
            {
                Debug.Log(MyLog.Format("ActionInteractableObjectModule  OnTriggerEnter Player"));
            }
        }

        private void OnTriggerExit(Collider other)
        {
        }

    }
}

