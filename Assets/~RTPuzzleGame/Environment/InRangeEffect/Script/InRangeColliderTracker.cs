using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class InRangeColliderTracker : MonoBehaviour
    {
        private List<Renderer> involvedRenderers = new List<Renderer>();

        #region External Dependencies
        private InRangeEffectManager InRangeEffectManager;
        #endregion

        public List<Renderer> InvolvedRenderers { get => involvedRenderers; }

        private void Start()
        {
            var r = this.GetComponent<Renderer>();
            if (r != null)
            {
                involvedRenderers.Add(r);
            }
            involvedRenderers.AddRange(this.GetComponentsInChildren<Renderer>());

            this.InRangeEffectManager = GameObject.FindObjectOfType<InRangeEffectManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var rangeType = RangeType.RetrieveFromCollisionType(other.GetComponent<CollisionType>());
            if (rangeType != null)
            {
                this.InRangeEffectManager.OnInRangeAdd(this, rangeType);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var rangeType = RangeType.RetrieveFromCollisionType(other.GetComponent<CollisionType>());
            if (rangeType != null)
            {
                this.InRangeEffectManager.OnInRangeRemove(this, rangeType);
            }
        }
    }
}
