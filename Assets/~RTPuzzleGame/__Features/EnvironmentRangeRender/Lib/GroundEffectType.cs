using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class GroundEffectType : MonoBehaviour
    {

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private Mesh groundEffectMesh;

        private bool hasBeenInit;

        private List<GroundEffectIgnoredGroundObjectType> associatedGroundEffectIgnoredGroundObjectType;
        private List<CombineInstance> groundEffectCombinedInstances;

        public MeshRenderer MeshRenderer { get => meshRenderer; }
        public MeshFilter MeshFilter { get => meshFilter; }
        public Mesh GroundEffectMesh { get => groundEffectMesh; }

        public List<GroundEffectIgnoredGroundObjectType> AssociatedGroundEffectIgnoredGroundObjectType { get => associatedGroundEffectIgnoredGroundObjectType; }

        public void Init()
        {
            if (!hasBeenInit)
            {
                meshRenderer = GetComponent<MeshRenderer>();
                meshFilter = GetComponent<MeshFilter>();
                this.groundEffectMesh = new Mesh();
                this.groundEffectMesh.SetVertices(this.meshFilter.mesh.vertices.ToList());
                this.groundEffectMesh.SetTriangles(this.meshFilter.mesh.triangles, 0);


                var childsGroundEffectIgnoredGroundObjectType = GetComponentsInChildren<GroundEffectIgnoredGroundObjectType>();
                if (childsGroundEffectIgnoredGroundObjectType != null)
                {
                    this.associatedGroundEffectIgnoredGroundObjectType = childsGroundEffectIgnoredGroundObjectType.ToList();
                }
                else
                {
                    this.associatedGroundEffectIgnoredGroundObjectType = new List<GroundEffectIgnoredGroundObjectType>();
                }

                foreach (var GroundEffectIgnoredGroundObjectType in this.associatedGroundEffectIgnoredGroundObjectType)
                {
                    GroundEffectIgnoredGroundObjectType.Init();
                }

                this.groundEffectCombinedInstances = new List<CombineInstance>();
                groundEffectCombinedInstances.Add(new CombineInstance() { mesh = this.groundEffectMesh, transform = this.transform.localToWorldMatrix });
                groundEffectCombinedInstances.AddRange(this.associatedGroundEffectIgnoredGroundObjectType.ConvertAll(GroundEffectIgnoredGroundObjectType => new CombineInstance() { mesh = GroundEffectIgnoredGroundObjectType.GroundEffectMesh, transform = GroundEffectIgnoredGroundObjectType.transform.localToWorldMatrix }));

                this.hasBeenInit = true;
            }
        }

        public List<CombineInstance> GetCombineInstances()
        {
            return this.groundEffectCombinedInstances;
        }

    }

}
