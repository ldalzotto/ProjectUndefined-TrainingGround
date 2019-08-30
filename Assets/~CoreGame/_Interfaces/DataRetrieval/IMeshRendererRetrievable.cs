using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public interface IMeshRendererRetrievable
    {
        List<MeshRenderer> GetAllMeshRenderers();
    }
}
