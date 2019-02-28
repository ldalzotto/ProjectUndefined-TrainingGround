using System.Collections.Generic;
using UnityEngine;

public class AIComponentsManager : MonoBehaviour
{

    private Dictionary<AiID, AIComponents> AIComponentsContainer = new Dictionary<AiID, AIComponents>();

    public void Init()
    {
        var aiComponentsContainer = GetComponentsInChildren<AIComponentsContainer>();
        if (aiComponentsContainer != null)
        {
            for (var i = 0; i < aiComponentsContainer.Length; i++)
            {
                var aiComponents = aiComponentsContainer[i].InitAIComponents();
                AIComponentsContainer[aiComponentsContainer[i].AiID] = aiComponents;
            }
        }
    }

    public AIComponents Get(AiID AiID)
    {
        return AIComponentsContainer[AiID];
    }

}
