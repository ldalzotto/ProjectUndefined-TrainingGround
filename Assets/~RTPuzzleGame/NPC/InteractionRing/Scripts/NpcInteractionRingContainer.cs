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
            this.NpcInteractionRingRendererManager = GameObject.FindObjectOfType<NpcInteractionRingRendererManager>();
        }

        private List<NpcInteractionRingType> activeNpcInteractionRings = new List<NpcInteractionRingType>();
        private List<NpcInteractionRingType> inactiveInteractionRings = new List<NpcInteractionRingType>();

        public List<NpcInteractionRingType> ActiveNpcInteractionRings { get => activeNpcInteractionRings; }

        #region External Events
        public void OnNpcInteractionRingCreated(NpcInteractionRingType createdNpcInteractionRingType)
        {
            this.activeNpcInteractionRings.Add(createdNpcInteractionRingType);
        }
        public void OnNpcInteractionRingDisabled(NpcInteractionRingType disabledNpcInteractionRingType)
        {
            disabledNpcInteractionRingType.OnDeactivate();
            this.activeNpcInteractionRings.Remove(disabledNpcInteractionRingType);
            this.inactiveInteractionRings.Add(disabledNpcInteractionRingType);
        }
        public void OnNpcInteractionRingEnabled(NpcInteractionRingType enabledNpcInteractionRingType)
        {
            enabledNpcInteractionRingType.OnActivate();
            this.inactiveInteractionRings.Remove(enabledNpcInteractionRingType);
            this.activeNpcInteractionRings.Add(enabledNpcInteractionRingType);
        }
        #endregion
    }

}
