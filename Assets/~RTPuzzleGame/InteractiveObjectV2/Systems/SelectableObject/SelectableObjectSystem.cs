using CoreGame;
using GameConfigurationID;
using RangeObjects;
using RTPuzzle;
using UnityEngine;

namespace InteractiveObjects
{
    #region Callback Events

    public delegate void OnPlayerTriggerInSelectionEnterDelegate(CoreInteractiveObject IntersectedInteractiveObject);

    public delegate void OnPlayerTriggerInSelectionExitDelegate(CoreInteractiveObject IntersectedInteractiveObject);

    public delegate RTPPlayerAction ProvideSelectableObjectPlayerActionDelegate(PlayerInteractiveObject PlayerInteractiveObject);

    #endregion

    public class SelectableObjectSystem : AInteractiveObjectSystem, ISelectableModule
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private RTPPlayerAction AssociatedPlayerAction;

        private ProvideSelectableObjectPlayerActionDelegate ProvideSelectableObjectPlayerActionDelegate;

        #region External Dependencies

        private PuzzleEventsManager PuzzleEventsManager = PuzzleEventsManager.Get();

        #endregion

        private RangeObjectV2 SphereRange;

        public SelectableObjectSystem(CoreInteractiveObject AssociatedInteractiveObject,
            SelectableObjectSystemDefinition SelectableObjectSystemDefinition,
            ProvideSelectableObjectPlayerActionDelegate ProvideSelectableObjectPlayerAction)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            SphereRange = new SphereRangeObjectV2(AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new SphereRangeObjectInitialization()
                {
                    RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                    IsTakingIntoAccountObstacles = false,
                    SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                    {
                        Radius = SelectableObjectSystemDefinition.SelectionRange
                    }
                }, AssociatedInteractiveObject, AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.name + "_SelectionRangeTrigger");
            SphereRange.RegisterPhysicsEventListener(new SelectableObjectPhysicsEventListener(OnPlayerTriggerInSelectionEnter, OnPlayerTriggerInSelectionExit));
            ProvideSelectableObjectPlayerActionDelegate = ProvideSelectableObjectPlayerAction;
        }

        public RTPPlayerAction GetAssociatedPlayerAction()
        {
            return AssociatedPlayerAction;
        }

        public ExtendedBounds GetAverageModelBoundLocalSpace()
        {
            return AssociatedInteractiveObject.InteractiveGameObject.AverageModelBounds;
        }

        public Transform GetTransform()
        {
            return AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform;
        }

        private void OnPlayerTriggerInSelectionEnter(CoreInteractiveObject IntersectedInteractiveObject)
        {
            AssociatedPlayerAction = ProvideSelectableObjectPlayerActionDelegate((PlayerInteractiveObject) IntersectedInteractiveObject);
            PuzzleEventsManager.PZ_EVT_OnActionInteractableEnter(this);
        }

        private void OnPlayerTriggerInSelectionExit(CoreInteractiveObject IntersectedInteractiveObject)
        {
            PuzzleEventsManager.PZ_EVT_OnActionInteractableExit(this);
            AssociatedPlayerAction = null;
        }

        public override void OnDestroy()
        {
            SphereRange.OnDestroy();
        }
    }

    internal class SelectableObjectPhysicsEventListener : ARangeObjectV2PhysicsEventListener
    {
        private InteractiveObjectTagStruct InteractiveObjectTagStruct;

        private OnPlayerTriggerInSelectionEnterDelegate OnPlayerTriggerInSelectionEnter;
        private OnPlayerTriggerInSelectionExitDelegate OnPlayerTriggerInSelectionExit;

        public SelectableObjectPhysicsEventListener(OnPlayerTriggerInSelectionEnterDelegate OnPlayerTriggerInSelectionEnter, OnPlayerTriggerInSelectionExitDelegate OnPlayerTriggerInSelectionExit)
        {
            InteractiveObjectTagStruct = new InteractiveObjectTagStruct {IsPlayer = 1};
            this.OnPlayerTriggerInSelectionEnter = OnPlayerTriggerInSelectionEnter;
            this.OnPlayerTriggerInSelectionExit = OnPlayerTriggerInSelectionExit;
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return InteractiveObjectTagStruct.Compare(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject.InteractiveObjectTag);
        }

        public override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            OnPlayerTriggerInSelectionEnter.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }

        public override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            OnPlayerTriggerInSelectionExit.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }
    }
}