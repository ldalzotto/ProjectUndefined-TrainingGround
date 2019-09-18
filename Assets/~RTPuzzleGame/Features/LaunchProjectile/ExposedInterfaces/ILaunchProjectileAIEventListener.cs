using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface ILaunchProjectileAIEventListener 
    {
        void PZ_EVT_AI_Projectile_Hitted(AIObjectDataRetriever AIObjectDataRetriever);
        void PZ_EVT_AI_Projectile_NoMoreAffected(AIObjectDataRetriever AIObjectDataRetriever);
    }

}
