using System.Collections.Generic;
using UnityEngine;

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
            var r = this.GetAllowedRenderer();
            if (r != null)
            {
                involvedRenderers.Add(r);
            }
            involvedRenderers.AddRange(this.GetAllRenderersInChildrenFiltered(this.transform));
            //parent fallback
            if (involvedRenderers.Count == 0)
            {
                involvedRenderers.AddRange(this.GetAllRenderersInChildrenFiltered(this.transform.parent));
            }

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

        private Renderer GetAllowedRenderer()
        {
            var r = this.GetComponent<Renderer>();
            if (this.FilterRendererToAllowed(r))
            {
                return r;
            }
            return null;
        }

        private List<Renderer> GetAllRenderersInChildrenFiltered(Transform baseTransform)
        {
            var foundRenderes = baseTransform.GetComponentsInChildren<Renderer>();
            if (foundRenderes != null)
            {
                var filteredRenderes = new List<Renderer>();
                foreach (var foundRend in foundRenderes)
                {
                    if (this.FilterRendererToAllowed(foundRend))
                    {
                        filteredRenderes.Add(foundRend);
                    }
                }
                return filteredRenderes;
            }
            return new List<Renderer>();
        }

        private bool FilterRendererToAllowed(Renderer r)
        {
            return (r != null && (r.GetType() == typeof(MeshRenderer) || r.GetType() == typeof(SkinnedMeshRenderer)));
        }
    }
}
