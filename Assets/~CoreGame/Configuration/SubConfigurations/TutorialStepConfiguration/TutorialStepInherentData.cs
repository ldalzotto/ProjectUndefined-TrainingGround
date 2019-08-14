using OdinSerializer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TutorialStepInherentData", menuName = "Configuration/CoreGame/TutorialStepConfiguration/TutorialStepInherentData", order = 1)]
    public class TutorialStepInherentData : SerializedScriptableObject
    {
        public TutorialGraph TutorialGraph;
    }

}
