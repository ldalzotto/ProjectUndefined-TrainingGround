using CoreGame;
using GameConfigurationID;
using RTPuzzle;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class DisarmSystemDefinition
    {
        public float DisarmRange;
        public float DisarmTime;
    }

    #region Callback Events
    public delegate void OnAssociatedDisarmObjectTriggerEnterDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    public delegate void OnAssociatedDisarmObjectTriggerExitDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    #endregion

    public class DisarmObjectSystem : AInteractiveObjectSystem
    {
        #region Internal Dependencies
        private GameObject ProgressBarGameObject;
        private CircleFillBarType progressbar;
        #endregion

        private DisarmSystemDefinition DisarmSystemDefinition;

        #region State
        private HashSet<CoreInteractiveObject> InteractiveObjectDisarmingThisObject = new HashSet<CoreInteractiveObject>();
        private float elapsedTime;
        #endregion

        #region External Events
        public void AddInteractiveObjectDisarmingThisObject(CoreInteractiveObject CoreInteractiveObject)
        {
            this.InteractiveObjectDisarmingThisObject.Add(CoreInteractiveObject);
        }
        public void RemoveInteractiveObjectDisarmingThisObject(CoreInteractiveObject CoreInteractiveObject)
        {
            this.InteractiveObjectDisarmingThisObject.Remove(CoreInteractiveObject);
        }
        #endregion

        public bool IsTimeElasped()
        {
            return (this.elapsedTime >= this.DisarmSystemDefinition.DisarmTime);
        }

        private RangeObjectV2 SphereRange;

        public DisarmObjectSystem(CoreInteractiveObject AssociatedInteractiveObject, DisarmSystemDefinition DisarmObjectInitializationData,
            InteractiveObjectTagStruct PhysicsEventListenerGuard, OnAssociatedDisarmObjectTriggerEnterDelegate OnAssociatedDisarmObjectTriggerEnter,
            OnAssociatedDisarmObjectTriggerExitDelegate OnAssociatedDisarmObjectTriggerExit)
        {
            this.DisarmSystemDefinition = DisarmObjectInitializationData;
            this.SphereRange = new SphereRangeObjectV2(AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new SphereRangeObjectInitialization
            {
                RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                IsTakingIntoAccountObstacles = false,
                SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                {
                    Radius = DisarmObjectInitializationData.DisarmRange
                }
            }, AssociatedInteractiveObject);
            this.SphereRange.ReceiveEvent(new RangeExternalPhysicsOnlyAddListener { ARangeObjectV2PhysicsEventListener = new DisarmObjectPhysicsEventListener(PhysicsEventListenerGuard, OnAssociatedDisarmObjectTriggerEnter, OnAssociatedDisarmObjectTriggerExit) });

            this.ProgressBarGameObject = new GameObject("ProgressBar");
            this.ProgressBarGameObject.transform.parent = AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform;
            this.progressbar = this.ProgressBarGameObject.AddComponent<CircleFillBarType>();

            this.progressbar.Init(Camera.main);
            this.progressbar.transform.position = AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition + IRenderBoundRetrievableStatic.GetDisarmProgressBarLocalOffset(AssociatedInteractiveObject.InteractiveGameObject.AverageModelBounds);
            this.progressbar.gameObject.SetActive(false);

            this.elapsedTime = 0f;
        }

        private float GetDisarmPercentage01()
        {
            return this.elapsedTime / this.DisarmSystemDefinition.DisarmTime;
        }

        public override void TickAlways(float d)
        {
            if (this.progressbar.gameObject.activeSelf)
            {
                this.progressbar.Tick(this.GetDisarmPercentage01());
            }
        }

        public override void Tick(float d, float timeAttenuationFactor)
        {
            if (this.InteractiveObjectDisarmingThisObject.Count > 0)
            {
                for (var i = 0; i < this.InteractiveObjectDisarmingThisObject.Count; i++)
                {
                    this.IncreaseTimeElapsedBy(d * timeAttenuationFactor);
                }
            }
        }

        public override void OnDestroy()
        {
            this.SphereRange.OnDestroy();
        }

        private void IncreaseTimeElapsedBy(float increasedTime)
        {
            this.elapsedTime += increasedTime;

            if (this.GetDisarmPercentage01() > 0 && !this.progressbar.gameObject.activeSelf)
            {
                CircleFillBarType.EnableInstace(this.progressbar);
            }
        }
    }


    class DisarmObjectPhysicsEventListener : ARangeObjectV2PhysicsEventListener
    {
        private InteractiveObjectTagStruct PhysicsEventListenerGuard;
        private OnAssociatedDisarmObjectTriggerEnterDelegate OnAssociatedDisarmObjectTriggerEnter;
        private OnAssociatedDisarmObjectTriggerExitDelegate OnAssociatedDisarmObjectTriggerExit;

        public DisarmObjectPhysicsEventListener(InteractiveObjectTagStruct physicsEventListenerGuard, OnAssociatedDisarmObjectTriggerEnterDelegate OnAssociatedDisarmObjectTriggerEnter,
            OnAssociatedDisarmObjectTriggerExitDelegate OnAssociatedDisarmObjectTriggerExit)
        {
            this.OnAssociatedDisarmObjectTriggerEnter = OnAssociatedDisarmObjectTriggerEnter;
            this.OnAssociatedDisarmObjectTriggerExit = OnAssociatedDisarmObjectTriggerExit;
            PhysicsEventListenerGuard = physicsEventListenerGuard;
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return this.PhysicsEventListenerGuard.Compare(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject.InteractiveObjectTag);
        }

        public override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            this.OnAssociatedDisarmObjectTriggerEnter.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }

        public override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            this.OnAssociatedDisarmObjectTriggerExit.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }
    }
}
