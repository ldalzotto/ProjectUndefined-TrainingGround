using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class AttractiveObjectType : MonoBehaviour
    {

        public static AttractiveObjectType Instanciate(Vector3 worldPosition, Transform parent, float effectDistance)
        {
            var attractiveObject = MonoBehaviour.Instantiate(PrefabContainer.Instance.AttractiveObjectPrefab, parent);
            attractiveObject.transform.position = worldPosition;
            attractiveObject.Init(effectDistance);
            return attractiveObject;
        }

        public void Init(float effectDistance)
        {
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.radius = effectDistance;
        }

    }
}
