using UnityEngine;
using System.Collections;
using OdinSerializer;
using System.Collections.Generic;

namespace AdventureGame
{
    [System.Serializable]
    public abstract class AContextActionInherentData : SerializedScriptableObject
    {
        public abstract AContextAction BuildContextAction();

        public static AContextAction BuildContextActionChain(List<AContextActionInherentData> aContextActionInherentDatas)
        {
            AContextAction rootContextAction = null;
            AContextAction lastBuildedContextAction = null;


            foreach (var aContextActionInherentData in aContextActionInherentDatas)
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
