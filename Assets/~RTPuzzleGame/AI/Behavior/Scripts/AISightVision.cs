using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class AISightVision : MonoBehaviour
    {
        private RangeTypeObject sightVisionRange;

        #region DEBUG
        private Transform playerTransform;
        #endregion

        public void Init()
        {
            this.playerTransform = GameObject.FindGameObjectWithTag(TagConstants.PLAYER_TAG).transform;

            this.sightVisionRange = GetComponentInChildren<RangeTypeObject>();

            this.sightVisionRange.Init(new RangeTypeObjectInitializer(20f));
        }

        public void Tick(float d)
        {
            this.sightVisionRange.Tick(d);
        }
    }
}
