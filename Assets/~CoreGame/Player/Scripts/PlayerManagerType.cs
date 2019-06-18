using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public abstract class PlayerManagerType : MonoBehaviour
    {

        protected CameraFollowManager CameraFollowManager;

        public PlayerPosition GetPlayerPosition()
        {
            if (this.CameraFollowManager == null)
            {
                return new PlayerPosition(this.transform.position, this.transform.rotation, GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG).transform.rotation);
            }
            else
            {
                return new PlayerPosition(this.transform.position, this.transform.rotation, this.CameraFollowManager.GetCameraPivotPointTransform().rotation);
            }
        }
    }

}
