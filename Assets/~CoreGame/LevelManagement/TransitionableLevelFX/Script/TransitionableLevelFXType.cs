using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

namespace CoreGame
{
    public class TransitionableLevelFXType : MonoBehaviour
    {
        private PostProcessVolume postProcessVolume;
        private Light mainDirectionalLight;

        public PostProcessVolume PostProcessVolume { get => postProcessVolume; }
        public Light MainDirectionalLight { get => mainDirectionalLight; }

        public void Init()
        {
            this.postProcessVolume = GetComponentInChildren<PostProcessVolume>();
            this.postProcessVolume.gameObject.SetActive(false);
            this.mainDirectionalLight = GetComponentInChildren<Light>();
            this.mainDirectionalLight.gameObject.SetActive(false);
        }

       
    }

}
