using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CutsceneInherentData", menuName = "Configuration/AdventureGame/CutsceneConfiguration/CutsceneInherentData", order = 1)]
    public class CutsceneInherentData : ScriptableObject
    {
        public PlayableAsset PlayableAsset;
    }

}
