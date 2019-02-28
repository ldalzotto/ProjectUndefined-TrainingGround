using UnityEngine;

namespace AdventureGame
{
    public class PointOfInterestContextDataContainer : MonoBehaviour
    {
        private CutsceneTimelinePOIData[] cutsceneTimelinePOIDatas;

        public CutsceneTimelinePOIData[] CutsceneTimelinePOIDatas { get => cutsceneTimelinePOIDatas; }

        private void Start()
        {
            cutsceneTimelinePOIDatas = GetComponentsInChildren<CutsceneTimelinePOIData>();
        }
    }

}