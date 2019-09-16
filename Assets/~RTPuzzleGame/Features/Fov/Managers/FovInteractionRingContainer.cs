using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class FovInteractionRingContainer : MonoBehaviour
    {
        #region External dependencies
        private FovInteractionRingRendererManager NpcInteractionRingRendererManager;
        #endregion

        public void Init()
        {
            this.NpcInteractionRingRendererManager = PuzzleGameSingletonInstances.NpcInteractionRingRendererManager;
        }

        private HashSet<FovInteractionRingType> activeNpcInteractionRings = new HashSet<FovInteractionRingType>();
        private HashSet<FovInteractionRingType> inactiveInteractionRings = new HashSet<FovInteractionRingType>();

        public HashSet<FovInteractionRingType> ActiveNpcInteractionRings { get => activeNpcInteractionRings; }

        #region External Events
        public void OnNpcInteractionRingCreated(FovInteractionRingType createdNpcInteractionRingType)
        {
            this.activeNpcInteractionRings.Add(createdNpcInteractionRingType);
        }
        public void OnNpcInteractionRingSetTo360(FovInteractionRingType disabledNpcInteractionRingType)
        {
            Debug.Log(MyLog.Format("DISABLING : " + disabledNpcInteractionRingType.name));
            disabledNpcInteractionRingType.OnDeactivate();
            this.activeNpcInteractionRings.Remove(disabledNpcInteractionRingType);
            this.inactiveInteractionRings.Add(disabledNpcInteractionRingType);
        }
        public void OnNpcInteractionRingSetUnder360(FovInteractionRingType enabledNpcInteractionRingType)
        {
            Debug.Log(MyLog.Format("ENABLING : " + enabledNpcInteractionRingType.name));
            enabledNpcInteractionRingType.OnActivate();
            this.inactiveInteractionRings.Remove(enabledNpcInteractionRingType);
            this.activeNpcInteractionRings.Add(enabledNpcInteractionRingType);
        }
        #endregion
    }

}
