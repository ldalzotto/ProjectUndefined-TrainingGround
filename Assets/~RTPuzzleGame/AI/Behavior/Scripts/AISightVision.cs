using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RTPuzzle
{
    public class AISightVision : MonoBehaviour
    {
        private List<RangeTypeObject>  sightVisionRange;

        #region DEBUG
        private Transform playerTransform;
        #endregion

        public void Init()
        {
            this.playerTransform = GameObject.FindGameObjectWithTag(TagConstants.PLAYER_TAG).transform;

            this.sightVisionRange = GetComponentsInChildren<RangeTypeObject>().ToList();

            foreach(var range in this.sightVisionRange)
            {
                range.Init(new RangeTypeObjectInitializer(30f));
            }
        }

        public void Tick(float d)
        {
            foreach (var range in this.sightVisionRange)
            {
                range.Tick(d);
            }
        }
    }
}
