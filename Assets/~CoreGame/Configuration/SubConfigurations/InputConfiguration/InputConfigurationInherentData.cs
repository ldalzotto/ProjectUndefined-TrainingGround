using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameConfigurationID;
using UnityEngine.InputSystem;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "InputConfigurationInherentData", menuName = "Configuration/CoreGame/InputConfiguration/InputConfigurationInherentData", order = 1)]
    public class InputConfigurationInherentData : ScriptableObject
    {
        [ReorderableListAttribute]
        public List<Key> AttributedKeys;
        [ReorderableListAttribute]
        public List<MouseButton> AttributedMouseButtons;
        public bool Down;
        public bool DownHold;
    }

}
