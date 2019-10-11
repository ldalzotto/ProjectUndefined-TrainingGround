using CoreGame;
using RTPuzzle;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjectTest
{
    public class InteractiveObjectV2Manager : GameSingleton<InteractiveObjectV2Manager>
    {
        public List<CoreInteractiveObject> InteractiveObjects { get; private set; } = new List<CoreInteractiveObject>();

        public Dictionary<Collider, CoreInteractiveObject> InteractiveObjectsIndexedByLogicCollider { get; private set; } = new Dictionary<Collider, CoreInteractiveObject>();

        public void Init()
        {
            var InteractiveObjectInitializers = GameObject.FindObjectsOfType<A_InteractiveObjectInitializer>();
            if (InteractiveObjectInitializers != null)
            {
                for (var InteractiveObjectInitializerIndex = 0; InteractiveObjectInitializerIndex < InteractiveObjectInitializers.Length; InteractiveObjectInitializerIndex++)
                {
                    InteractiveObjectInitializers[InteractiveObjectInitializerIndex].Init();
                }
            }
        }

        public void FixedTick(float d)
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < this.InteractiveObjects.Count; InteractiveObjectIndex++)
            {
                if (this.InteractiveObjects[InteractiveObjectIndex].IsUpdatedInMainManager)
                {
                    this.InteractiveObjects[InteractiveObjectIndex].FixedTick(d);
                }
            }
        }

        public void TickAlways(float d)
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < this.InteractiveObjects.Count; InteractiveObjectIndex++)
            {
                if (this.InteractiveObjects[InteractiveObjectIndex].IsUpdatedInMainManager)
                {
                    this.InteractiveObjects[InteractiveObjectIndex].TickAlways(d);
                }
            }
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < this.InteractiveObjects.Count; InteractiveObjectIndex++)
            {
                if (this.InteractiveObjects[InteractiveObjectIndex].IsUpdatedInMainManager)
                {
                    this.InteractiveObjects[InteractiveObjectIndex].Tick(d, timeAttenuationFactor);
                }
            }
        }

        public void TickWhenTimeIsStopped()
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < this.InteractiveObjects.Count; InteractiveObjectIndex++)
            {
                if (this.InteractiveObjects[InteractiveObjectIndex].IsUpdatedInMainManager)
                {
                    this.InteractiveObjects[InteractiveObjectIndex].TickWhenTimeIsStopped();
                }
            }
        }

        public void AfterTicks()
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < this.InteractiveObjects.Count; InteractiveObjectIndex++)
            {
                if (this.InteractiveObjects[InteractiveObjectIndex].IsUpdatedInMainManager)
                {
                    this.InteractiveObjects[InteractiveObjectIndex].AfterTicks();
                }
            }

            List<CoreInteractiveObject> InteractiveObjectsToDelete = null;
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < this.InteractiveObjects.Count; InteractiveObjectIndex++)
            {
                if (this.InteractiveObjects[InteractiveObjectIndex].IsAskingToBeDestroyed)
                {
                    if (InteractiveObjectsToDelete == null) { InteractiveObjectsToDelete = new List<CoreInteractiveObject>(); }
                    InteractiveObjectsToDelete.Add(this.InteractiveObjects[InteractiveObjectIndex]);
                }
            }
            if (InteractiveObjectsToDelete != null)
            {
                foreach (var InteractiveObjectToDelete in InteractiveObjectsToDelete)
                {
                    InteractiveObjectToDelete.Destroy();
                    this.OnInteractiveObjectDestroyed(InteractiveObjectToDelete);
                }
            }
        }

        public void LateTick(float d)
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < this.InteractiveObjects.Count; InteractiveObjectIndex++)
            {
                if (this.InteractiveObjects[InteractiveObjectIndex].IsUpdatedInMainManager)
                {
                    this.InteractiveObjects[InteractiveObjectIndex].LateTick(d);
                }
            }
        }

        public void OnInteractiveObjectCreated(CoreInteractiveObject InteractiveObject)
        {
            this.InteractiveObjects.Add(InteractiveObject);
            this.InteractiveObjectsIndexedByLogicCollider.Add(InteractiveObject.InteractiveGameObject.GetLogicColliderAsBox(), InteractiveObject);
        }

        public void OnInteractiveObjectDestroyed(CoreInteractiveObject InteractiveObject)
        {
            this.InteractiveObjects.Remove(InteractiveObject);
            this.InteractiveObjectsIndexedByLogicCollider.Remove(InteractiveObject.InteractiveGameObject.GetLogicColliderAsBox());
            RangeObjectV2ManagerOperations.ClearAllReferencesOfInteractiveObject(InteractiveObject);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.InteractiveObjects.Clear();
            this.InteractiveObjects = null;
            this.InteractiveObjectsIndexedByLogicCollider.Clear();
            this.InteractiveObjectsIndexedByLogicCollider = null;
        }
    }
}
