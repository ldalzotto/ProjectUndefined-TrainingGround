using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public class CoreEventsManager : MonoBehaviour
    {
        public void CORE_EVT_OnLevelChanged()
        {
            GameObject.FindObjectOfType<AsbtractCoreGameManager>().OnLevelChanged();
        }
    }

}
