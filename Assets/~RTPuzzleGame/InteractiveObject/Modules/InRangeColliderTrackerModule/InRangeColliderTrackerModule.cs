//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class InRangeColliderTrackerModule : InteractiveObjectModule, IAILogicColliderModuleListener
    {

        #region Module Dependencies
        private ModelObjectModule ModelObjectModule;
        private ObjectRepelModule ObjectRepelModule;
        #endregion

        #region External Dependencies
        private RangeEventsManager RangeEventsManager;
        #endregion

        #region Internal Dependencies
        private MonoBehaviour trackedObject;
        #endregion

        #region Data Retrieval
        public List<Renderer> GetRenderers()
        {
            if (this.ModelObjectModule == null) { return new List<Renderer>(); }
            return this.ModelObjectModule.GetAllRenderers();
        }
        #endregion

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            #region Module Dependencies
            this.ModelObjectModule = interactiveObjectType.GetModule<ModelObjectModule>();
            this.ObjectRepelModule = interactiveObjectType.GetModule<ObjectRepelModule>();
            #endregion

            interactiveObjectType.GetModule<AILogicColliderModule>().AddListener(this);

            #region External Dependencies
            this.RangeEventsManager = PuzzleGameSingletonInstances.RangeEventsManager;
            #endregion

            this.ResolveTrackedObject();
        }

        public void OnTriggerEnter(Collider other)
        {
            var rangeType = RangeType.RetrieveFromCollisionType(other.GetComponent<CollisionType>());
            if (rangeType != null)
            {
                this.RangeEventsManager.RANGE_EVT_InsideRangeTracker(this, rangeType);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            var rangeType = RangeType.RetrieveFromCollisionType(other.GetComponent<CollisionType>());
            if (rangeType != null)
            {
                this.RangeEventsManager.RANGE_EVT_OutsideRangeTracker(this, rangeType);
            }
        }

        public void OnTriggerStay(Collider other) { }

        public void Tick(float d, float timeAttenuationFactor) { }


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

        private void ResolveTrackedObject()
        {
            if (this.ObjectRepelModule != null)
            {
                this.trackedObject = this.ObjectRepelModule;
            }
        }
    }
}