using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

namespace CoreGame
{
    public class TransitionableLevelFXType : MonoBehaviour
    {
        private PostProcessVolume postProcessVolume;

        public PostProcessVolume PostProcessVolume { get => postProcessVolume; }

        public void Init()
        {
            this.postProcessVolume = GetComponentInChildren<PostProcessVolume>();
            this.postProcessVolume.gameObject.SetActive(false);
        }

       
    }

}
