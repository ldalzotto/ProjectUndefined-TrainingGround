using CoreGame;
using GameConfigurationID;
using RTPuzzle;
using System;
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

    public class DisarmObjectSystem : AInteractiveObjectSystem
    {
        #region Internal Dependencies
        [VE_Ignore]
        private GameObject ProgressBarGameObject;

        [VE_Nested]
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
            InteractiveObjectTagStruct PhysicsEventListenerGuard, Action<CoreInteractiveObject> OnAssociatedDisarmObjectTriggerEnter,
            Action<CoreInteractiveObject> OnAssociatedDisarmObjectTriggerExit)
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
            this.SphereRange.ReceiveEvent(new RangeExternalPhysicsOnlyAddListener {
                ARangeObjectV2PhysicsEventListener = new RangeObjectV2PhysicsEventListener_Delegated(PhysicsEventListenerGuard, onTriggerEnterAction: OnAssociatedDisarmObjectTriggerEnter, onTriggerExitAction: OnAssociatedDisarmObjectTriggerExit)
            });

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
}
