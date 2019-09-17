using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IDisarmObjectModuleEventListener
    {
        void PZ_DisarmObject_TriggerEnter(IDisarmObjectModuleDataRetrieval DisarmObjectModule, AIObjectDataRetriever AIObjectDataRetriever);
        void PZ_DisarmObject_TriggerExit(IDisarmObjectModuleDataRetrieval DisarmObjectModule, AIObjectDataRetriever AIObjectDataRetriever);
    }
}
