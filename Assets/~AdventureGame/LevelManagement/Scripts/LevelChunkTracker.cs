

using CoreGame;
using UnityEngine;

#if UNITY_EDITOR
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

        private bool hasBeenInit = false;

        internal void Init()
        {
            Debug.Log("LevelChunkTracker Init : " + this.name);
            this.AdventureEventManager = GameObject.FindObjectOfType<AdventureEventManager>();
            this.transitionableLevelFXType = GetComponentInChildren<TransitionableLevelFXType>();
            this.transitionableLevelFXType.Init();
            this.hasBeenInit = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasBeenInit)
            {
                if (other.tag == TagConstants.PLAYER_TAG)
                {
                    this.AdventureEventManager.AD_EVT_OnChunkLevelEnter(this);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (hasBeenInit)
            {
                if (other.tag == TagConstants.PLAYER_TAG)
                {
                    this.AdventureEventManager.AD_EVT_OnChunkLevelExit(this);
                }
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
