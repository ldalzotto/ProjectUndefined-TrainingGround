using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CustomTextRenderer : MonoBehaviour
{
    public Font Font;
    public string Text;

    public bool MakeProgression;
    public bool SelectCharacter;
    public bool LinePositions;

    private Text textComponent;
    private Mesh Mesh;
    private CanvasRenderer CanvasRenderer;

    private TextGenerationSettings TextGenerationSettings;
    private TextGenerator TextGenerator;

    private List<Vector3> initialVertices;

    void Start()
    {
        this.textComponent = GetComponentInChildren<Text>();
        this.TextGenerationSettings = this.textComponent.GetGenerationSettings(((RectTransform)this.transform).sizeDelta);
        this.textComponent.gameObject.SetActive(false);
        this.TextGenerationSettings.font = this.Font;

        this.TextGenerator = new TextGenerator();
        this.TextGenerator.Invalidate();
        this.TextGenerator.Populate(this.Text, this.TextGenerationSettings);

        this.TextGenToMesh(this.TextGenerator, out Mesh textMesh);
        this.CanvasRenderer = GetComponent<CanvasRenderer>();
        this.CanvasRenderer.SetMesh(textMesh);
        this.Mesh = textMesh;
        this.initialVertices = new List<Vector3>();
        this.Mesh.GetVertices(initialVertices);
        this.CanvasRenderer.SetMaterial(this.Font.material, null);
    }

    private string progressionString;
    private int currentCharacterSelected;

    void Update()
    {
        if (MakeProgression)
        {
            this.progressionString += this.Text[this.progressionString.Length];
            this.TextGenerator.Invalidate();
            this.TextGenerationSettings.generationExtents = ((RectTransform)this.transform).sizeDelta;
            this.TextGenerator.Populate(this.progressionString, this.TextGenerationSettings);

            this.TextGenToMesh(this.TextGenerator, out Mesh textMesh);
            this.CanvasRenderer.SetMesh(textMesh);
            this.Mesh = textMesh;
            this.CanvasRenderer.SetMaterial(this.Font.material, null);

            if (this.progressionString == this.Text)
            {
                this.MakeProgression = false;
            }
        }
        else
        {
            this.progressionString = "";
        }

        if (SelectCharacter)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.currentCharacterSelected = Mathf.Min(this.currentCharacterSelected + 1, this.Mesh.vertexCount / 4);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.currentCharacterSelected = Mathf.Max(this.currentCharacterSelected - 1, 0);
            }

            var verticesToSet = new List<Vector3>(this.initialVertices);
            var scaleMatrix = Matrix4x4.Scale(new Vector3(1,1,0) * 2);
            for (var i = 0; i < verticesToSet.Count; i++)
            {
                if (i >= this.currentCharacterSelected * 4 && i < ((this.currentCharacterSelected + 1) * 4))
                {
                    verticesToSet[i] = scaleMatrix * verticesToSet[i];
                }
            }
            this.Mesh.SetVertices(verticesToSet);
            this.CanvasRenderer.SetMesh(this.Mesh);
        }
    }

    private void TextGenToMesh(TextGenerator generator, out Mesh mesh)
    {
        mesh = new Mesh();

        mesh.vertices = generator.verts.Select(v => v.position).ToArray();
        mesh.colors32 = generator.verts.Select(v => v.color).ToArray();
        mesh.uv = generator.verts.Select(v => v.uv0).ToArray();
        var triangles = new int[generator.vertexCount * 6];
        for (var i = 0; i < mesh.vertices.Length / 4; i++)
        {
            var startVerticeIndex = i * 4;
            var startTriangleIndex = i * 6;
            triangles[startTriangleIndex++] = startVerticeIndex;
            triangles[startTriangleIndex++] = startVerticeIndex + 1;
            triangles[startTriangleIndex++] = startVerticeIndex + 2;
            triangles[startTriangleIndex++] = startVerticeIndex;
            triangles[startTriangleIndex++] = startVerticeIndex + 2;
            triangles[startTriangleIndex] = startVerticeIndex + 3;
        }
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }
}
