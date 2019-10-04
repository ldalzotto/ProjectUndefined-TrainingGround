using InteractiveObjectTest;

namespace RTPuzzle
{
    public static class RangeObjectV2ManagerOperations
    {
        public static void ClearAllReferencesOfInteractiveObject(CoreInteractiveObject InteractiveObject)
        {
            var allRangeObjects = RangeObjectV2Manager.Get().RangeObjects;
            for (var rangeObjectIndex = 0; rangeObjectIndex < allRangeObjects.Count; rangeObjectIndex++)
            {
                var rangeObject = allRangeObjects[rangeObjectIndex];
                if (rangeObject.RangeIntersectionV2System != null)
                {
                    var RangeIntersectionListeners = rangeObject.RangeIntersectionV2System.RangeIntersectionListeners;
                    if (RangeIntersectionListeners != null)
                    {
                        for(var RangeIntersectionListenerIndex = 0; RangeIntersectionListenerIndex < RangeIntersectionListeners.Count; RangeIntersectionListenerIndex++)
                        {
                            RangeIntersectionListeners[RangeIntersectionListenerIndex].RemoveReferencesToInteractiveObject(InteractiveObject);
                        }
                    }
                }
            }
        }
    }
}
