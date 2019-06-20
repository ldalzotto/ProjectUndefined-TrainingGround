using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CutsceneConfiguration", menuName = "Configuration/AdventureGame/CutsceneConfiguration/CutsceneConfiguration", order = 1)]
    public class CutsceneConfiguration : ConfigurationSerialization<CutsceneId, CutsceneInherentData>
    {

    }

}
