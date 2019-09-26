using UnityEngine;

namespace RTPuzzle
{
    public partial class AttractiveObjectModule : IAttractiveObjectModuleEvent
    {
        #region IAttractiveObjectModuleEvent
        public void OnAttractiveObjectPlayerActionExecuted(RaycastHit attractiveObjectWorldPositionHit)
        {
            this.transform.position = attractiveObjectWorldPositionHit.point;

            //TODO make the rotation relative to the player
            this.transform.LookAt(this.transform.position + Vector3.forward);
        }
        #endregion
    }

}
