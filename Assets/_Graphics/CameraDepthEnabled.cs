using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public class CameraDepthEnabled : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        }

    }

}
