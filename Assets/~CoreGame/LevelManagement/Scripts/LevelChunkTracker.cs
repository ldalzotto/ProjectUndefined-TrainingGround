﻿using UnityEngine;

#if UNITY_EDITOR
#endif

namespace CoreGame
{
    public class LevelChunkTracker : MonoBehaviour
    {
        #region External Dependencies
        private LevelManagerEventManager LevelManagerEventManager;
        #endregion

        #region Internal Dependencies
        private LevelChunkType associatedLevelChunkType;
        #endregion

        private TransitionableLevelFXType transitionableLevelFXType;

        #region Data Retrieval
        public TransitionableLevelFXType TransitionableLevelFXType { get => transitionableLevelFXType; }
        public LevelChunkType AssociatedLevelChunkType { get => associatedLevelChunkType; set => associatedLevelChunkType = value; }
        #endregion

        private bool hasInit;

        internal void Init()
        {
            this.LevelManagerEventManager = GameObject.FindObjectOfType<LevelManagerEventManager>();

            Debug.Log("LevelChunkTracker Init : " + this.name);
            this.associatedLevelChunkType = GetComponent<LevelChunkType>();
            this.transitionableLevelFXType = GetComponentInChildren<TransitionableLevelFXType>();
            this.transitionableLevelFXType.Init();
            hasInit = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == TagConstants.PLAYER_TAG)
            {
                this.LevelManagerEventManager.CORE_EVT_OnChunkLevelEnter(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == TagConstants.PLAYER_TAG)
            {
                this.LevelManagerEventManager.CORE_EVT_OnChunkLevelExit(this);
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