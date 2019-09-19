using ConfigurationEditor;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ActionInteractableObjectConfiguration", menuName = "Configuration/PuzzleGame/ActionInteractableObjectConfiguration/ActionInteractableObjectConfiguration", order = 1)]
    public class ActionInteractableObjectConfiguration : ConfigurationSerialization<ActionInteractableObjectID, ActionInteractableObjectInherentData>
    {    }
    
 
}
