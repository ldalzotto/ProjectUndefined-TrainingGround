using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class InRangeColliderTracker : MonoBehaviour
    {
        private List<Renderer> involvedRenderers = new List<Renderer>();

        #region External Dependencies
        private RangeEventsManager RangeEventsManager;
        #endregion

        #region Internal Dependencies
        private CollisionType CollisionType;
        private MonoBehaviour trackedObject;
        #endregion

        private Dictionary<RangeTypeID, bool> rangesInContact;

        public List<Renderer> InvolvedRenderers { get => involvedRenderers; }

        public void Init()
        {
            #region External Dependencies
            this.RangeEventsManager = GameObject.FindObjectOfType<RangeEventsManager>();
            #endregion

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

            this.CollisionType = GetComponent<CollisionType>();
            this.rangesInContact = new Dictionary<RangeTypeID, bool>();
            this.ResolveTrackedObject();
        }

        private void OnTriggerEnter(Collider other)
        {
            var rangeType = RangeType.RetrieveFromCollisionType(other.GetComponent<CollisionType>());
            if (rangeType != null)
            {
                this.RangeEventsManager.RANGE_EVT_InsideRangeTracker(this, rangeType);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var rangeType = RangeType.RetrieveFromCollisionType(other.GetComponent<CollisionType>());
            if (rangeType != null)
            {
                this.RangeEventsManager.RANGE_EVT_OutsideRangeTracker(this, rangeType);
            }
        }

        #region DataRetrieval
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
            if (baseTransform == null) { return new List<Renderer>(); }

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
        #endregion

        #region Tracked Object Retrieval
        public ObjectRepelModule GetTrackedRepelableObject()
        {
            if (this.trackedObject != null && this.trackedObject.GetType() == typeof(ObjectRepelModule))
            {
                return (ObjectRepelModule)this.trackedObject;
            }
            return null;
        }
        #endregion

        private bool FilterRendererToAllowed(Renderer r)
        {
            return (r != null && (r.GetType() == typeof(MeshRenderer) || r.GetType() == typeof(SkinnedMeshRenderer)));
        }

        private void ResolveTrackedObject()
        {
            if (this.CollisionType.IsRepelable)
            {
                this.trackedObject = this.GetComponent<ObjectRepelModule>();
            }
        }
    }

}
