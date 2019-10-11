using CoreGame;
using RTPuzzle;
using UnityEngine;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class SelectableObjectSystemDefinition
    {
        public float SelectionRange;
    }

    #region Callback Events
    public delegate void OnPlayerTriggerInSelectionEnterDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    public delegate void OnPlayerTriggerInSelectionExitDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    public delegate RTPPlayerAction ProvideSelectableObjectPlayerActionDelegate(PlayerInteractiveObject PlayerInteractiveObject);
    #endregion

    public class SelectableObjectSystem : AInteractiveObjectSystem, ISelectableModule
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private RangeObjectV2 SphereRange;
        private RTPPlayerAction AssociatedPlayerAction;

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private ProvideSelectableObjectPlayerActionDelegate ProvideSelectableObjectPlayerActionDelegate;

        public SelectableObjectSystem(CoreInteractiveObject AssociatedInteractiveObject,
            SelectableObjectSystemDefinition SelectableObjectSystemDefinition,
            ProvideSelectableObjectPlayerActionDelegate ProvideSelectableObjectPlayerAction)
        {
            this.PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;

            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.SphereRange = new SphereRangeObjectV2(AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new SphereRangeObjectInitialization()
            {
                RangeTypeID = GameConfigurationID.RangeTypeID.NOT_DISPLAYED,
                IsTakingIntoAccountObstacles = false,
                SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                {
                    Radius = SelectableObjectSystemDefinition.SelectionRange
                }
            }, AssociatedInteractiveObject, "SelectionRangeTrigger");
            this.SphereRange.ReceiveEvent(new RangeExternalPhysicsOnlyAddListener { ARangeObjectV2PhysicsEventListener = new SelectableObjectPhysicsEventListener(this.OnPlayerTriggerInSelectionEnter, this.OnPlayerTriggerInSelectionExit) });
            this.ProvideSelectableObjectPlayerActionDelegate = ProvideSelectableObjectPlayerAction;
        }

        private void OnPlayerTriggerInSelectionEnter(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.AssociatedPlayerAction = this.ProvideSelectableObjectPlayerActionDelegate((PlayerInteractiveObject)IntersectedInteractiveObject);
            this.PuzzleEventsManager.PZ_EVT_OnActionInteractableEnter(this);
        }

        private void OnPlayerTriggerInSelectionExit(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.PuzzleEventsManager.PZ_EVT_OnActionInteractableExit(this);
            this.AssociatedPlayerAction = null;
        }

        public override void OnDestroy()
        { 
            this.SphereRange.OnDestroy();
        }

        public RTPPlayerAction GetAssociatedPlayerAction()
        {
            return this.AssociatedPlayerAction;
        }

        public ExtendedBounds GetAverageModelBoundLocalSpace()
        {
            return this.AssociatedInteractiveObject.InteractiveGameObject.AverageModelBounds;
        }

        public Transform GetTransform()
        {
            return this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform;
        }
    }

    class SelectableObjectPhysicsEventListener : ARangeObjectV2PhysicsEventListener
    {
        private InteractiveObjectTagStruct InteractiveObjectTagStruct;

        private OnPlayerTriggerInSelectionEnterDelegate OnPlayerTriggerInSelectionEnter;
        private OnPlayerTriggerInSelectionExitDelegate OnPlayerTriggerInSelectionExit;

        public SelectableObjectPhysicsEventListener(OnPlayerTriggerInSelectionEnterDelegate OnPlayerTriggerInSelectionEnter, OnPlayerTriggerInSelectionExitDelegate OnPlayerTriggerInSelectionExit)
        {
            this.InteractiveObjectTagStruct = new InteractiveObjectTagStruct { IsPlayer = 1 };
            this.OnPlayerTriggerInSelectionEnter = OnPlayerTriggerInSelectionEnter;
            this.OnPlayerTriggerInSelectionExit = OnPlayerTriggerInSelectionExit;
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return this.InteractiveObjectTagStruct.Compare(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject.InteractiveObjectTag);
        }

        public override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            this.OnPlayerTriggerInSelectionEnter.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }

        public override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            this.OnPlayerTriggerInSelectionExit.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }
    }
}