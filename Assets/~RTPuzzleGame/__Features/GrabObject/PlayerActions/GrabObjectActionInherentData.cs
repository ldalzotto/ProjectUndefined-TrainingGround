using UnityEngine;
using System.Collections;
using InteractiveObjectTest;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "GrabObjectActionInherentData", menuName = "Test/GrabObjectActionInherentData", order = 1)]
    public class GrabObjectActionInherentData : PlayerActionInherentData
    {
        public PlayerActionInherentData AddedPlayerActionInherentData;
        
        public override RTPPlayerAction BuildPlayerAction(PlayerInteractiveObject PlayerInteractiveObject)
        {
            return new GrabObjectAction(this.AddedPlayerActionInherentData.BuildPlayerAction(PlayerInteractiveObject), this.CorePlayerActionDefinition);
        }
    }

}
