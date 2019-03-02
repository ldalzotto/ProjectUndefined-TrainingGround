using UnityEngine;
using UnityEngine.UI;

namespace RTPuzzle
{
    public class TimeFlowPlayPauseManager : MonoBehaviour
    {
        public TimeFlowPlayPauseIconSwitchComponent TimeFlowPlayPauseIconSwitchComponent;

        private TimeFlowPlayPauseIconSwitchManager TimeFlowPlayPauseIconSwitchManager;

        public void Init()
        {
            var image = GetComponent<Image>();
            this.TimeFlowPlayPauseIconSwitchManager = new TimeFlowPlayPauseIconSwitchManager(image, TimeFlowPlayPauseIconSwitchComponent);
        }

        public void Tick(bool isPlaying)
        {
            this.TimeFlowPlayPauseIconSwitchManager.Tick(isPlaying);
        }
    }

    class TimeFlowPlayPauseIconSwitchManager
    {
        private Image iconImage;
        private TimeFlowPlayPauseIconSwitchComponent TimeFlowPlayPauseIconSwitchComponent;

        public TimeFlowPlayPauseIconSwitchManager(Image iconImage, TimeFlowPlayPauseIconSwitchComponent TimeFlowPlayPauseIconSwitchComponent)
        {
            this.iconImage = iconImage;
            this.TimeFlowPlayPauseIconSwitchComponent = TimeFlowPlayPauseIconSwitchComponent;
        }

        private bool isPlaying;

        public void Tick(bool isPlaying)
        {
            if (isPlaying)
            {
                if (!this.isPlaying)
                {
                    this.iconImage.sprite = TimeFlowPlayPauseIconSwitchComponent.PlaySprite;
                }
            } else
            {
                if (this.isPlaying)
                {
                    this.iconImage.sprite = TimeFlowPlayPauseIconSwitchComponent.PauseSprite;
                }
            }
            this.isPlaying = isPlaying;
        }
    }

    [System.Serializable]
    public class TimeFlowPlayPauseIconSwitchComponent
    {
        public Sprite PlaySprite;
        public Sprite PauseSprite;
    }

}
