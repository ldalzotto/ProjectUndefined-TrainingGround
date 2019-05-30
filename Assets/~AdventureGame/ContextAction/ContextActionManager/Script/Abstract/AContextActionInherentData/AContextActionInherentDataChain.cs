using UnityEngine;
using System.Collections;
using OdinSerializer;
using System.Collections.Generic;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AContextActionInherentDataChain", menuName = "Configuration/AdventureGame/ContextAction/AContextActionInherentDataChain")]
    public class AContextActionInherentDataChain : SerializedScriptableObject
    {
        public List<AContextActionInherentData> ContextActionChain;

        public AContextAction BuildContextActionChain()
        {
            AContextAction rootContextAction = null;
            AContextAction lastBuildedContextAction = null;


            foreach (var aContextActionInherentData in ContextActionChain)
            {
                if (rootContextAction == null)
                {
                    rootContextAction = aContextActionInherentData.BuildContextAction();
                    lastBuildedContextAction = rootContextAction;
                }
                else
                {
                    var buildedContextAction = aContextActionInherentData.BuildContextAction();
                    lastBuildedContextAction.SetNextContextAction(buildedContextAction);
                    lastBuildedContextAction = buildedContextAction;
                }
            }

            return rootContextAction;
        }

    }

}
