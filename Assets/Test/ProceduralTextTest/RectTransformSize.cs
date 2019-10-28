using System.Collections.Generic;
using CoreGame;
using Test.ProceduralTextTest;
using UnityEngine;

public class RectTransformSize : MonoBehaviour
{
    public Texture2D DebugTexture;
    public bool MoveToNext;
    public string TextToDisplay;
    private ProceduralText ProceduralText;
    public ProceduralTextWindowDefinition ProceduralTextWindowDefinition;

    private ProceduralTextWindow ProceduralTextWindow;

    private void Start()
    {
        this.ProceduralTextWindow = new ProceduralTextWindow(FindObjectOfType<Canvas>().gameObject,
            this.TextToDisplay,
            this.ProceduralTextWindowDefinition,
            new ProceduralTextParametersV2(new ProceduralTextParameterParser(
                new Dictionary<ProceduralTextParameterParserKey, string>(),
                new List<Texture2D>()
                {
                    this.DebugTexture, this.DebugTexture
                }
            )));
        this.ProceduralTextWindow.SetTransformPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
        this.ProceduralTextWindow.Play();
    }

    void Update()
    {
        if (MoveToNext)
        {
            MoveToNext = false;
            this.ProceduralTextWindow.MoveToNextPage();
        }

        this.ProceduralTextWindow.Tick(Time.deltaTime);
    }

    private void OnGUI()
    {
        this.ProceduralTextWindow.GUITick();
    }

    private Vector2 LocalSize(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetLocalCorners(corners);
        return new Vector2(corners[3].x - corners[0].x, corners[1].y - corners[0].y);
    }
}