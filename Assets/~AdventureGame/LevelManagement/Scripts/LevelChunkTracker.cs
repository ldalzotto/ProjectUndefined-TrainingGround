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
            if(other.tag == TagConstants.PLAYER_TAG)
            {
                this.AdventureEventManager.AD_EVT_OnChunkLevelExit(this);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var box = GetComponent<BoxCollider>();
            var oldColor = Gizmos.color;
            Gizmos.color = Color.magenta;
            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Color.magenta;
            Handles.Label(transform.TransformPoint(new Vector3(box.center.x, box.center.y + box.bounds.max.y + 10f, box.center.z)), gameObject.name, labelStyle);
            Gizmos.DrawWireCube(transform.TransformPoint(box.center), box.size);
            Gizmos.color = oldColor;
        }
#endif

    }

}
