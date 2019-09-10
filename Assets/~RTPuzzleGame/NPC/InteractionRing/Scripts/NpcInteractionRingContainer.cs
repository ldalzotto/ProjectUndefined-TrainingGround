using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class NpcInteractionRingContainer : MonoBehaviour
    {
        #region External dependencies
        private NpcInteractionRingRendererManager NpcInteractionRingRendererManager;
        #endregion

        public void Init()
        {
            this.NpcInteractionRingRendererManager = PuzzleGameSingletonInstances.NpcInteractionRingRendererManager;
        }

        private HashSet<NpcInteractionRingType> activeNpcInteractionRings = new HashSet<NpcInteractionRingType>();
        private HashSet<NpcInteractionRingType> inactiveInteractionRings = new HashSet<NpcInteractionRingType>();

        public HashSet<NpcInteractionRingType> ActiveNpcInteractionRings { get => activeNpcInteractionRings; }

        #region External Events
        public void OnNpcInteractionRingCreated(NpcInteractionRingType createdNpcInteractionRingType)
        {
            this.activeNpcInteractionRings.Add(createdNpcInteractionRingType);
        }
        public void OnNpcInteractionRingSetTo360(NpcInteractionRingType disabledNpcInteractionRingType)
        {
            Debug.Log(MyLog.Format("DISABLING : " + disabledNpcInteractionRingType.name));
            disabledNpcInteractionRingType.OnDeactivate();
            this.activeNpcInteractionRings.Remove(disabledNpcInteractionRingType);
            this.inactiveInteractionRings.Add(disabledNpcInteractionRingType);
        }
        public void OnNpcInteractionRingSetUnder360(NpcInteractionRingType enabledNpcInteractionRingType)
        {
            Debug.Log(MyLog.Format("ENABLING : " + enabledNpcInteractionRingType.name));
            enabledNpcInteractionRingType.OnActivate();
            this.inactiveInteractionRings.Remove(enabledNpcInteractionRingType);
            this.activeNpcInteractionRings.Add(enabledNpcInteractionRingType);
        }
        #endregion
    }

}
