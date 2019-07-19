using System.Collections.Generic;
using System.Linq;

namespace RTPuzzle
{
    public class AISightVision : AbstractAIManager
    {
        private List<RangeTypeObject> sightVisionRange;

        public void Init()
        {
            this.sightVisionRange = GetComponentsInChildren<RangeTypeObject>().ToList();

            foreach (var range in this.sightVisionRange)
            {
                range.Init(new RangeTypeObjectInitializer());
            }
        }

        public void Tick(float d)
        {
            foreach (var range in this.sightVisionRange)
            {
                range.Tick(d);
            }
        }

#if UNITY_EDITOR
        public void HandlesTick()
        {
            foreach (var rangeType in GetComponentsInChildren<RangeType>().ToList())
            {
                rangeType.HandlesDraw();
            }
        }
#endif
    }
}
