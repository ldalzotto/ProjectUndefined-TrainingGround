using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CutsceneInherentData", menuName = "Configuration/AdventureGame/CutsceneConfiguration/CutsceneInherentData", order = 1)]
    public class CutsceneInherentData : ScriptableObject
    {
        public CutsceneGraph CutsceneGraph;
    }

}
