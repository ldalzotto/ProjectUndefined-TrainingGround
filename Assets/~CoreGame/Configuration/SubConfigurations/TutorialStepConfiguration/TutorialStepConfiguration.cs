using ConfigurationEditor;
using GameConfigurationID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TutorialStepConfiguration", menuName = "Configuration/CoreGame/TutorialStepConfiguration/TutorialStepConfiguration", order = 1)]
    public class TutorialStepConfiguration : ConfigurationSerialization<TutorialStepID, TutorialStepInherentData>
    {
    }

}
