using System.Collections.Generic;
using CoreGame;
using SelectionWheel;
using UnityEngine;

namespace RTPuzzle
{
    public class PlayerActionWheelManager : GameSingleton<PlayerActionWheelManager>
    {
        private SelectionWheelObject PlayerActionSelectionWheel;

        public void Init(Transform followingTransform)
        {
            this.PlayerActionSelectionWheel = new SelectionWheelObject(followingTransform);
        }

        public void Tick(float d)
        {
            this.PlayerActionSelectionWheel.Tick(d);
        }

        public void LateTick(float d)
        {
            this.PlayerActionSelectionWheel.LateTick(d);
        }

        #region External Events

        internal void PlayerActionWheelAwake(List<RTPPlayerAction> availablePlayerActions)
        {
            this.PlayerActionSelectionWheel.AwakeWheel(availablePlayerActions.ConvertAll(rtpPlayerAction => new PlayerSelectionWheelNodeData(rtpPlayerAction) as SelectionWheelNodeData));
        }

        internal void PlayerActionWheelSleep(bool detroyImmediate)
        {
            this.PlayerActionSelectionWheel.SleepWheel(detroyImmediate);
        }

        internal void PlayerActionWheelRefresh(List<RTPPlayerAction> availablePlayerActions)
        {
            if (this.PlayerActionSelectionWheel.IsWheelEnabled)
            {
                this.PlayerActionSelectionWheel.RefreshWheel(availablePlayerActions.ConvertAll(rtpPlayerAction => new PlayerSelectionWheelNodeData(rtpPlayerAction) as SelectionWheelNodeData));
            }
        }

        #endregion

        #region Data Retrieval

        public RTPPlayerAction GetCurrentlySelectedPlayerAction()
        {
            if (this.PlayerActionSelectionWheel.IsWheelEnabled)
            {
                return ((PlayerSelectionWheelNodeData) this.PlayerActionSelectionWheel.GetSelectedNodeData()).Data as RTPPlayerAction;
            }

            return null;
        }

        #endregion

        #region Logical Conditions

        public bool IsSelectionWheelEnabled()
        {
            return this.PlayerActionSelectionWheel.IsWheelEnabled;
        }

        #endregion
    }
}