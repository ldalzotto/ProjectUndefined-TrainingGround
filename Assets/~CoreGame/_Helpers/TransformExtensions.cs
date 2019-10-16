using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public static class TransformExtensions 
    {
        public static void ResetLocal(this Transform transform)
        {
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public static void ResetScale(this Transform transform)
        {
            transform.localScale = Vector3.one;
        }
    }
    
}
