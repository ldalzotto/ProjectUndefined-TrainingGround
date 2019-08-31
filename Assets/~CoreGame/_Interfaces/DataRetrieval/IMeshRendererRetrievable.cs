using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public interface IRendererRetrievable
    {
        List<Renderer> GetAllRenderers();
    }
}
