using CoreGame;
using GameConfigurationID;
using RTPuzzle;
using UnityEngine;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class SelectableObjectSystemDefinition
    {
        public float SelectionRange;
        
        [Inline()]
        public PlayerActionInherentData AssociatedPlayerAction;
    }

    #region Callback Events
    public delegate void OnPlayerTriggerInSelectionEnterDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    public delegate void OnPlayerTriggerInSelectionExitDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    #endregion

    public class SelectableObjectSystem : AInteractiveObjectSystem, ISelectableModule
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private RangeObjectV2 SphereRange;
        private RTPPlayerAction AssociatedPlayerAction;

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        public SelectableObjectSystem(CoreInteractiveObject AssociatedInteractiveObject,
            SelectableObjectSystemDefinition SelectableObjectSystemDefinition)
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
            this.AssociatedPlayerAction = new GrabObjectAction(new GrabActionInherentData(PlayerActionId.STONE_PROJECTILE_ACTION_1, SelectionWheelNodeConfigurationId.ATTRACTIVE_OBJECT_LAY_WHEEL_CONFIG, 1f));
        }

        private void OnPlayerTriggerInSelectionEnter(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.PuzzleEventsManager.PZ_EVT_OnActionInteractableEnter(this);
        }

        private void OnPlayerTriggerInSelectionExit(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.PuzzleEventsManager.PZ_EVT_OnActionInteractableExit(this);
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