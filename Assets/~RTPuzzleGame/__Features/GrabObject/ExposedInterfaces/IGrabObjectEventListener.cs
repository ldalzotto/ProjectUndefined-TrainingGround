using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IGrabObjectEventListener
    {
        void PZ_EVT_OnGrabObjectEnter(IGrabObjectModuleDataRetrieval IGrabObjectModuleDataRetrieval);
        void PZ_EVT_OnGrabObjectExit(IGrabObjectModuleDataRetrieval IGrabObjectModuleDataRetrieval);
    }

}
