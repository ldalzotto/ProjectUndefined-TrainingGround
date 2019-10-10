using CoreGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class GenericPuzzleAIBehavior : PuzzleAIBehavior
    {
        public IFovManagerCalcuation FovManagerCalcuation;

        public void Init(List<InterfaceAIManager> aiManagers, AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            this.BaseInit(aiManagers, AIBheaviorBuildInputData);

            this.FovManagerCalcuation = AIBheaviorBuildInputData.FovManagerCalcuation;
            this.aIBehaviorManagerContainer = new AIBehaviorManagerContainer(aiManagers);

            var dic = new Dictionary<int, InterfaceAIManager>()
            {
                 { 1, this.aIBehaviorManagerContainer.GetAIManager<AbstractAIFearStunManager>() },
                 { 2, this.aIBehaviorManagerContainer.GetAIManager<AbstractAIEscapeWithoutTriggerManager>() },
                 { 3, this.aIBehaviorManagerContainer.GetAIManager<AbstractAITargetZoneManager>( )},
                 { 4, this.aIBehaviorManagerContainer.GetAIManager<AbstractPlayerEscapeManager>() },
                 { 5, this.aIBehaviorManagerContainer.GetAIManager<AbstractAIProjectileEscapeManager>() },
                 { 6, this.aIBehaviorManagerContainer.GetAIManager<AbstractAIMoveTowardPlayerManager>() },
                 { 8, this.aIBehaviorManagerContainer.GetAIManager<AbstractAIAttractiveObjectManager>() },
            };
            this.aIBehaviorManagerContainer.SetAIManagersByExecutionOrder(new SortedList<int, InterfaceAIManager>(
                dic.Select(s => s).Where(s => s.Value != null).ToDictionary(s => s.Key, s => s.Value)
            ));

            this.AfterChildInit();

        }


        #region Logical Conditions
        public bool IsProjectileTriggerAllowedToInterruptOtherStates()
        {
            return this.IsManagerEnabled<AbstractAIAttractiveObjectManager>()
                        || this.IsManagerEnabled<AbstractAITargetZoneManager>()
                        || this.IsManagerEnabled<AbstractPlayerEscapeManager>()
                        || this.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>();
        }
        public bool IsPlayerEscapeAllowedToInterruptOtherStates()
        {
            return this.IsManagerEnabled<AbstractAITargetZoneManager>()
                  || this.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>();
        }
        #endregion

        protected override void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
            this.GetAIManager<AbstractAIFearStunManager>().IfNotNull((AbstractAIFearStunManager AbstractAIFearStunManager) => AbstractAIFearStunManager.BeforeManagersUpdate(d, timeAttenuationFactor));
            this.GetAIManager<AbstractPlayerEscapeManager>().IfNotNull((AbstractPlayerEscapeManager AbstractPlayerEscapeManager) => AbstractPlayerEscapeManager.BeforeManagersUpdate(d, timeAttenuationFactor));
        }

        public override void OnDestinationReached()
        {
            this.GetAIManager<AbstractAIProjectileEscapeManager>().IfNotNull((aIProjectileEscapeManager) => aIProjectileEscapeManager.OnDestinationReached());
            this.GetAIManager<AbstractAIEscapeWithoutTriggerManager>().IfNotNull((aIEscapeWithoutTriggerManager) => aIEscapeWithoutTriggerManager.OnDestinationReached());
            this.GetAIManager<AbstractAITargetZoneManager>().IfNotNull((aITargetZoneManager) => aITargetZoneManager.OnDestinationReached());
            this.GetAIManager<AbstractAIAttractiveObjectManager>().IfNotNull((aIAttractiveObjectManager) => aIAttractiveObjectManager.OnDestinationReached());
            this.GetAIManager<AbstractPlayerEscapeManager>().IfNotNull((playerEscapeManager) => playerEscapeManager.OnDestinationReached());
            this.GetAIManager<AbstractAIMoveTowardPlayerManager>().IfNotNull((aIPlayerAttractiveManager) => aIPlayerAttractiveManager.OnDestinationReached());

            if (this.FovManagerCalcuation != null)
            {
                if (!this.IsManagerEnabled<AbstractAIFearStunManager>() && !this.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>() && !this.IsManagerEnabled<AbstractAITargetZoneManager>()
                                   && !this.IsManagerEnabled<AbstractAIProjectileEscapeManager>() && !this.IsManagerEnabled<AbstractPlayerEscapeManager>())
                {
                    this.FovManagerCalcuation.ResetFOV();
                }
            }

            this.puzzleAIBehaviorExternalEventManager.AfterDestinationReached(this);
        }

        public override void TickGizmo()
        {
            this.GetAIManager<AbstractAIProjectileEscapeManager>().IfNotNull(aIProjectileEscapeManager => aIProjectileEscapeManager.GizmoTick());
        }

        public void DebugGUITick()
        {
            GUILayout.Label("State : " + this.currentManagerState.GetType().Name);
        }

        public override string ToString()
        {
            return "TE MERE";
        }
    }
}
