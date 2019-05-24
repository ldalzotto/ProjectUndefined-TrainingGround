

using UnityEngine;
using System.Collections;
using System;
using CoreGame;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AdventureGame
{
    public class LevelChunkTracker : MonoBehaviour
    {
        #region External dependencies
        private AdventureEventManager AdventureEventManager;
        #endregion

        private TransitionableLevelFXType transitionableLevelFXType;

        public TransitionableLevelFXType TransitionableLevelFXType { get => transitionableLevelFXType; }

        internal void Init()
        {
            this.AdventureEventManager = GameObject.FindObjectOfType<AdventureEventManager>();
            this.transitionableLevelFXType = GetComponentInChildren<TransitionableLevelFXType>();
            this.transitionableLevelFXType.Init();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == TagConstants.PLAYER_TAG)
            {
                this.AdventureEventManager.AD_EVT_OnChunkLevelEnter(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == TagConstants.PLAYER_TAG)
            {
                this.AdventureEventManager.AD_EVT_OnChunkLevelExit(this);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var box = GetComponent<BoxCollider>();
            GizmoHelper.DrawBoxCollider(box, transform, Color.magenta, gameObject.name, MyEditorStyles.LabelMagenta);
        }
#endif

    }

}
