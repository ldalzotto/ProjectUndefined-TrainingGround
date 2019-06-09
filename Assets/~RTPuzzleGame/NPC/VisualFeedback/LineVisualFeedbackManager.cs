using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class LineVisualFeedbackManager 
    {
        #region External Dependencies
        private AttractiveObjectsContainerManager AttractiveObjectsContainerManager;
        #endregion

        public LineVisualFeedbackManager()
        {
            this.AttractiveObjectsContainerManager = GameObject.FindObjectOfType<AttractiveObjectsContainerManager>();
        }

        private DottedLine AttractiveObjectDottedLine;
        private AttractiveObjectId AttractiveObjectId;


        public void Tick(float d, Vector3 npcAIBoundsCenterWorldPosition)
        {
            if (this.AttractiveObjectDottedLine != null)
            {
                this.AttractiveObjectDottedLine.Tick(d, npcAIBoundsCenterWorldPosition, this.AttractiveObjectsContainerManager.GetAttractiveObjectType(AttractiveObjectId).transform.position);
            }
        }

        #region External Events
        public void OnAttractiveObjectStart(AttractiveObjectId attractiveObjectId)
        {
            if(this.AttractiveObjectDottedLine == null)
            {
                this.AttractiveObjectDottedLine = DottedLine.CreateInstance();
            }
            this.AttractiveObjectId = attractiveObjectId;
        }
        public void OnAttractiveObjectEnd()
        {
            if (this.AttractiveObjectDottedLine != null)
            {
                this.AttractiveObjectDottedLine.DestroyInstance();
            }
        }
        #endregion

    }
}
