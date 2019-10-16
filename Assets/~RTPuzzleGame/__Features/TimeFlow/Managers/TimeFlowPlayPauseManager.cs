using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace RTPuzzle
{
    public class TimeFlowPlayPauseManager : GameSingleton<TimeFlowPlayPauseManager>
    {
        private TimeFlowPlayPauseIconSwitchManager TimeFlowPlayPauseIconSwitchManager;

        public void Init()
        {
            this.TimeFlowPlayPauseIconSwitchManager = new TimeFlowPlayPauseIconSwitchManager(new TimeFlowPlayPauseGameObject(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzleGlobalStaticConfiguration),
                PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowPlayPauseIconSwitchComponent);
        }

        public void Tick(bool isPlaying)
        {
            this.TimeFlowPlayPauseIconSwitchManager.Tick(isPlaying);
        }
    }

    class TimeFlowPlayPauseIconSwitchManager
    {
        private TimeFlowPlayPauseGameObject TimeFlowPlayPauseGameObject;
        private TimeFlowPlayPauseIconSwitchComponent TimeFlowPlayPauseIconSwitchComponent;

        public TimeFlowPlayPauseIconSwitchManager(TimeFlowPlayPauseGameObject TimeFlowPlayPauseGameObject, TimeFlowPlayPauseIconSwitchComponent TimeFlowPlayPauseIconSwitchComponent)
        {
            this.TimeFlowPlayPauseGameObject = TimeFlowPlayPauseGameObject;
            this.TimeFlowPlayPauseIconSwitchComponent = TimeFlowPlayPauseIconSwitchComponent;
            this.TimeFlowPlayPauseGameObject.SetSprite(TimeFlowPlayPauseIconSwitchComponent.PlaySprite);
        }

        private bool isPlaying;

        public void Tick(bool isPlaying)
        {
            if (isPlaying)
            {
                if (!this.isPlaying)
                {
                    this.TimeFlowPlayPauseGameObject.SetSprite(TimeFlowPlayPauseIconSwitchComponent.PlaySprite);
                }
            }
            else
            {
                if (this.isPlaying)
                {
                    this.TimeFlowPlayPauseGameObject.SetSprite(TimeFlowPlayPauseIconSwitchComponent.PauseSprite);
                }
            }
            this.isPlaying = isPlaying;
        }
    }

    public class TimeFlowPlayPauseGameObject
    {
        private GameObject timeFlowPlayPauseGameObject;
        private Image timeFlowPlayPauseImage;

        public TimeFlowPlayPauseGameObject(PuzzleGlobalStaticConfiguration PuzzleGlobalStaticConfiguration)
        {
            var gameCanvas = CoreGameSingletonInstances.GameCanvas;

            this.timeFlowPlayPauseGameObject = new GameObject(this.GetType().Name, typeof(RectTransform));
            this.timeFlowPlayPauseGameObject.layer = LayerMask.NameToLayer("UI");
            this.timeFlowPlayPauseGameObject.transform.parent = gameCanvas.transform;

            (this.timeFlowPlayPauseGameObject.transform as RectTransform).anchorMin = Vector2.zero;
            (this.timeFlowPlayPauseGameObject.transform as RectTransform).anchorMax = Vector2.zero;
            (this.timeFlowPlayPauseGameObject.transform as RectTransform).localScale = Vector2.one;
            (this.timeFlowPlayPauseGameObject.transform as RectTransform).pivot = Vector2.zero;
            
            var CanvasScaler = CanvasScalerManager.Get().CanvasScaler;

            (this.timeFlowPlayPauseGameObject.transform as RectTransform).sizeDelta = new Vector2(
                    CanvasScaler.referenceResolution.x * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowPlayIconSizeDeltaFactor.x,
                    CanvasScaler.referenceResolution.y * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowPlayIconSizeDeltaFactor.y
                );
            (this.timeFlowPlayPauseGameObject.transform as RectTransform).offsetMin = new Vector2(
                    CanvasScaler.referenceResolution.x * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowPlayIconOffsetMinFactor.x,
                    CanvasScaler.referenceResolution.y * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowPlayIconOffsetMinFactor.y
                );
            (this.timeFlowPlayPauseGameObject.transform as RectTransform).offsetMax = (this.timeFlowPlayPauseGameObject.transform as RectTransform).offsetMin + new Vector2(
                   CanvasScaler.referenceResolution.x * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowPlayIconSizeDeltaFactor.x,
                   CanvasScaler.referenceResolution.y * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowPlayIconSizeDeltaFactor.y
               );

            timeFlowPlayPauseGameObject.AddComponent<CanvasRenderer>();

            this.timeFlowPlayPauseImage = timeFlowPlayPauseGameObject.AddComponent<Image>();
            this.timeFlowPlayPauseImage.color = new Color(0.72f, 0.78f, 0.76f);
        }

        public void SetSprite(Sprite sprite)
        {
            this.timeFlowPlayPauseImage.sprite = sprite;
        }
    }

    [System.Serializable]
    public class TimeFlowPlayPauseIconSwitchComponent
    {
        public Sprite PlaySprite;
        public Sprite PauseSprite;
    }

}
