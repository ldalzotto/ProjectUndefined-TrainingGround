using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace RTPuzzle
{
    public class TimeFlowBarGameObject
    {
        private Canvas ParentCanvas;
        private GameObject timeFlowBarGameObject;

        private Image EmptyBar;
        private Image Fullbar;

        public TimeFlowBarGameObject(PuzzleGlobalStaticConfiguration PuzzleGlobalStaticConfiguration)
        {
            this.ParentCanvas = CoreGameSingletonInstances.GameCanvas;
            this.timeFlowBarGameObject = new GameObject(this.GetType().Name, typeof(RectTransform));
            this.timeFlowBarGameObject.layer = LayerMask.NameToLayer("UI");
            this.timeFlowBarGameObject.transform.parent = this.ParentCanvas.transform;

            var CanvasScaler = CanvasScalerManager.Get().CanvasScaler;

            (this.timeFlowBarGameObject.transform as RectTransform).anchorMin = Vector2.zero;
            (this.timeFlowBarGameObject.transform as RectTransform).anchorMax = Vector2.zero;
            (this.timeFlowBarGameObject.transform as RectTransform).localScale = Vector2.one;
            (this.timeFlowBarGameObject.transform as RectTransform).pivot = Vector2.zero;

            (this.timeFlowBarGameObject.transform as RectTransform).sizeDelta = new Vector2(CanvasScaler.referenceResolution.x * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowBarSizeDeltaFactor.x,
                CanvasScaler.referenceResolution.y * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowBarSizeDeltaFactor.y);
            (this.timeFlowBarGameObject.transform as RectTransform).offsetMin = new Vector2(CanvasScaler.referenceResolution.x * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowBarOffsetMinFactor.x,
                CanvasScaler.referenceResolution.y * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowBarOffsetMinFactor.y);
            (this.timeFlowBarGameObject.transform as RectTransform).offsetMax =
                (this.timeFlowBarGameObject.transform as RectTransform).offsetMin + new Vector2(CanvasScaler.referenceResolution.x * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowBarSizeDeltaFactor.x,
                CanvasScaler.referenceResolution.y * PuzzleGlobalStaticConfiguration.TimeFlowBarUILayoutData.TimeFlowBarSizeDeltaFactor.y);

            this.EmptyBar = this.BuildBar(this.timeFlowBarGameObject.transform, "EmptyBar", new Color(0.2065217f, 0.25f, 0.2347826f));
            this.Fullbar = this.BuildBar(this.timeFlowBarGameObject.transform, "Fullbar", new Color(0.8879397f, 0.95f, 0.9261307f));
        }

        private Image BuildBar(Transform parent, string name, Color imageColor)
        {
            var imageGO = new GameObject(name, typeof(RectTransform));
            imageGO.transform.parent = parent;

            (imageGO.transform as RectTransform).anchorMin = Vector2.zero;
            (imageGO.transform as RectTransform).anchorMax = Vector2.one;
            (imageGO.transform as RectTransform).pivot = new Vector2(0, 0.5f);

            (imageGO.transform as RectTransform).offsetMin = Vector2.zero;
            (imageGO.transform as RectTransform).offsetMax = Vector2.zero;
            (imageGO.transform as RectTransform).sizeDelta = Vector2.zero;
            (imageGO.transform as RectTransform).localScale = Vector2.one;

            imageGO.AddComponent<CanvasRenderer>();
            Image image = imageGO.AddComponent<Image>();

            image.color = imageColor;

            return image;
        }

        public void SetFullBarScale(Vector3 localScale)
        {
            this.Fullbar.transform.localScale = localScale;
        }
        public Transform GetTransform() { return this.timeFlowBarGameObject.transform; }
    }
}
