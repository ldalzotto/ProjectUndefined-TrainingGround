using UnityEngine;
using System.Collections;
using GameConfigurationID;
using CoreGame;

namespace RTPuzzle
{
    public interface IPlayerActionManagerEvent : SelectableObjectSelectionManagerEventListener<ISelectableModule>
    {
        void ExecuteAction(RTPPlayerAction rTPPlayerAction);
        void StopAction();
        void OnSelectionWheelAwake();
        void OnSelectionWheelSleep(bool destroyImmediate);
        void IncreaseOrAddActionsRemainingExecutionAmount(PlayerActionId playerActionId, int deltaRemaining);
        void AddActionToAvailable(RTPPlayerAction selectableObjectPlayerAction);
        void RemoveActionToAvailable(RTPPlayerAction selectableObjectPlayerAction);
    }
}
