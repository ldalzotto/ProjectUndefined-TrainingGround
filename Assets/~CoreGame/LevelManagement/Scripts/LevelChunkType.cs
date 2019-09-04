using AnimationSystem;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class LevelChunkType : MonoBehaviour
    {
        public LevelZoneChunkID LevelZoneChunkID;
        private BaseAnimationSystem[] AnimationSystems;

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
            var animationSystems = this.GetComponentsInChildren<BaseAnimationSystem>();
            if (animationSystems != null)
            {
                this.AnimationSystems = animationSystems;
                for(var i = 0; i < this.AnimationSystems.Length;i++)
                {
                    this.AnimationSystems[i].Init();
                }
            }
        }

        private void Update()
        {
            var d = Time.deltaTime;
            if (this.AnimationSystems != null)
            {
                for (var i = 0; i < this.AnimationSystems.Length; i++)
                {
                    this.AnimationSystems[i].Tick(d);
                }
            }
        }
    }

}
