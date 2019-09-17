using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IDisarmObjectModuleEvent 
    {
        void OnDisarmObjectStart(AIObjectDataRetriever InvolvedAI);
        void OnDisarmObjectEnd(AIObjectDataRetriever InvolvedAI);
    }

}
