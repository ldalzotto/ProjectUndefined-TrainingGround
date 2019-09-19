using UnityEngine;
using System.Collections;
using System.Linq;

namespace RTPuzzle
{
    public class GroundEffectIgnoredGroundObjectType : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private Mesh groundEffectMesh;

        public Mesh GroundEffectMesh { get => groundEffectMesh; }
        public MeshRenderer MeshRenderer { get => meshRenderer;  }
        public MeshFilter MeshFilter { get => meshFilter; }

        public void Init()
        {
            this.meshFilter = GetComponent<MeshFilter>();
            this.meshRenderer = GetComponent<MeshRenderer>();

            this.groundEffectMesh = new Mesh();
            this.groundEffectMesh.SetVertices(this.meshFilter.mesh.vertices.ToList());
            this.groundEffectMesh.SetTriangles(this.meshFilter.mesh.triangles, 0);
        }

    }
}
