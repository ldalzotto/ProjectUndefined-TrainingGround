using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class ObjectRepelContainerManager : MonoBehaviour
    {
        #region External Dependencies
        private ObjectRepelContainer ObjectRepelContainer;
        #endregion

        public void Init()
        {
            this.ObjectRepelContainer = GameObject.FindObjectOfType<ObjectRepelContainer>();
        }

        #region External Events
        public void OnObjectRepelRepelled(ObjectRepelType objectRepelType, Vector3 targetWorldPosition)
        {
            Debug.Log(MyLog.Format(Vector3.Distance(objectRepelType.transform.position, targetWorldPosition)));
            objectRepelType.OnObjectRepelRepelled(BeziersControlPoints.Build(objectRepelType.transform.position, targetWorldPosition, objectRepelType.transform.up, BeziersControlPointsShape.CURVED));
        }
        #endregion

        public void Tick(float d, float timeAttenuation)
        {
            foreach (var repelObject in this.ObjectRepelContainer.ObjectsRepelable)
            {
                repelObject.Tick(d, timeAttenuation);
            }
        }
    }

}

