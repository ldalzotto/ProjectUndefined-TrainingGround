using GameConfigurationID;
using UnityEngine;

namespace CoreGame
{
    public class LevelChunkType : MonoBehaviour
    {
        public LevelZoneChunkID LevelZoneChunkID;

        public static void DestroyAllDestroyOnStartObjects()
        {
            foreach (var objectToDestroy in GameObject.FindGameObjectsWithTag(TagConstants.TO_DESTROY_ON_START))
            {
                MonoBehaviour.Destroy(objectToDestroy);
            }
        }

        private void Start()
        {
            this.GetComponent<LevelChunkTracker>().Init();
        }
    }

}
