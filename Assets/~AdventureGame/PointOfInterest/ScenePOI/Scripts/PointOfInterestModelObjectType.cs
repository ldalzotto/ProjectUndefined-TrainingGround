using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class PointOfInterestModelObjectType : MonoBehaviour
    {

        #region Internal Dependencies
        private List<GameObject> oneLevelDownChildObjects;
        private Collider[] allColliders;
        private Animator animator;

        public Animator Animator { get => animator; }
        #endregion

        public void Init()
        {
            this.allColliders = GetComponentsInChildren<Collider>();
            this.oneLevelDownChildObjects = gameObject.FindOneLevelDownChilds();
            this.animator = GetComponent<Animator>();
            if (this.animator == null)
            {
                this.animator = GetComponentInChildren<Animator>();
            }

            //TODO - TESET -> REMOVE
            if (transform.parent.GetComponentInChildren<PointOfInterestType>().PointOfInterestId == GameConfigurationID.PointOfInterestId._1_Town_Girl)
            {
                AnimationPlayerHelper.Play(this.animator, GameObject.FindObjectOfType<CoreConfigurationManager>().AnimationConfiguration().ConfigurationInherentData[GameConfigurationID.AnimationID.BACK_TO_WALL_POSE], 0f);
            }
        }

        #region External Events
        public void SetActive(bool value)
        {
            foreach (var oneLevelDownChildObject in oneLevelDownChildObjects)
            {
                oneLevelDownChildObject.SetActive(value);
            }

            // this.gameObject.SetActive(value);
        }
        public void SetAllColliders(bool value)
        {
            if (this.allColliders != null)
            {
                foreach (var collider in this.allColliders)
                {
                    collider.enabled = value;
                }
            }
        }
        #endregion
    }

}
