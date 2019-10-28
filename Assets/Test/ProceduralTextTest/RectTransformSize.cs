using CoreGame;
using UnityEngine;
using UnityEngine.UI;

public class RectTransformSize : MonoBehaviour
{
    public ContentSizeFitter ContentSizeFitter;
    public Texture2D DebugTexture;
    public bool Generate;
    public bool MoveToNext;
    public RectTransform ParentTransform;
    public Image ParentImage;
    public Text TextUI;
    public string TextToDisplay;
    public GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent;
    private ProceduralText ProceduralText;

    void Update()
    {
        if (Generate)
        {
            Generate = false;
            this.ProceduralText = new ProceduralText(this.TextToDisplay, this.GeneratedTextDimensionsComponent, null, this.TextUI);
            this.ProceduralText.CalculateCurrentPage();
        }

        if (MoveToNext)
        {
            MoveToNext = false;
            this.ProceduralText.MoveToNextPage();
        }

        if (this.ProceduralText != null)
        {
            this.ProceduralText.Increment();
            var sd = (this.ParentImage.transform as RectTransform).sizeDelta;
            (this.ParentImage.transform as RectTransform).sizeDelta = new Vector2(this.ProceduralText.GetWindowWidth(), this.ProceduralText.GetWindowHeight());
            (this.TextUI.transform as RectTransform).sizeDelta = new Vector2(this.ProceduralText.GetWindowWidth(), this.ProceduralText.GetWindowHeight());

            var oldAnchorMax = ParentTransform.anchorMax;
            var oldAnchorMin = ParentTransform.anchorMin;

            ParentTransform.anchorMax = new Vector2(0, 1);
            ParentTransform.anchorMin = new Vector2(0, 1);

            ParentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.ProceduralText.GetWindowWidth());
            ParentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.ProceduralText.GetWindowHeight());

            ParentTransform.anchorMax = oldAnchorMax;
            ParentTransform.anchorMin = oldAnchorMin;

            this.ContentSizeFitter.enabled = false;
            this.ContentSizeFitter.enabled = true;
        }
    }

    private Vector2 LocalSize(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetLocalCorners(corners);
        return new Vector2(corners[3].x - corners[0].x, corners[1].y - corners[0].y);
    }
}