using RTPuzzle;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjectTest
{
    public class InteractiveObjectV2Manager
    {
        private static InteractiveObjectV2Manager Instance;
        public static InteractiveObjectV2Manager Get()
        {
            if (Instance == null) { Instance = new InteractiveObjectV2Manager(); }
            return Instance;
        }

        public List<CoreInteractiveObject> InteractiveObjects { get; private set; } = new List<CoreInteractiveObject>();

        public Dictionary<Collider, CoreInteractiveObject> InteractiveObjectsIndexedByCollider { get; private set; } = new Dictionary<Collider, CoreInteractiveObject>();

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

        public void TickAlways(float d)
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < this.InteractiveObjects.Count; InteractiveObjectIndex++)
            {
                this.InteractiveObjects[InteractiveObjectIndex].TickAlways(d);
            }
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < this.InteractiveObjects.Count; InteractiveObjectIndex++)
            {
                this.InteractiveObjects[InteractiveObjectIndex].Tick(d, timeAttenuationFactor);
            }
        }

        public void TickWhenTimeIsStopped()
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < this.InteractiveObjects.Count; InteractiveObjectIndex++)
            {
                this.InteractiveObjects[InteractiveObjectIndex].TickWhenTimeIsStopped();
            }
        }

        public void AfterTicks()
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < this.InteractiveObjects.Count; InteractiveObjectIndex++)
            {
                this.InteractiveObjects[InteractiveObjectIndex].AfterTicks();
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

        public void OnInteractiveObjectCreated(CoreInteractiveObject InteractiveObject)
        {
            this.InteractiveObjects.Add(InteractiveObject);
            this.InteractiveObjectsIndexedByCollider.Add(InteractiveObject.InteractiveGameObject.LogicCollider, InteractiveObject);
        }

        public void OnInteractiveObjectDestroyed(CoreInteractiveObject InteractiveObject)
        {
            this.InteractiveObjects.Remove(InteractiveObject);
            this.InteractiveObjectsIndexedByCollider.Remove(InteractiveObject.InteractiveGameObject.LogicCollider);
            RangeObjectV2ManagerOperations.ClearAllReferencesOfInteractiveObject(InteractiveObject);
        }

        public void OnDestroy()
        {
            this.InteractiveObjects.Clear();
            this.InteractiveObjects = null;
            this.InteractiveObjectsIndexedByCollider.Clear();
            this.InteractiveObjectsIndexedByCollider = null;
            Instance = null;
        }
    }
}
