using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RTPuzzle
{
    public class AISightVision : MonoBehaviour
    {
        private List<RangeTypeObject>  sightVisionRange;

        public void Init()
        {
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
