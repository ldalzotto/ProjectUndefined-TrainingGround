using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "NearPlayerGameOverTriggerConfiguration", menuName = "Configuration/PuzzleGame/NearPlayerGameOverTriggerConfiguration/NearPlayerGameOverTriggerConfiguration", order = 1)]
    public class NearPlayerGameOverTriggerConfiguration : ConfigurationSerialization<NearPlayerGameOverTriggerID, NearPlayerGameOverTriggerInherentData>
    {
        
    }

}
