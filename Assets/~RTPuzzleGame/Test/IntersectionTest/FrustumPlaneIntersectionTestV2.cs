using RTPuzzle;
using UnityEngine;

public class FrustumPlaneIntersectionTestV2 : MonoBehaviour
{
    public BoxCollider BoxObject;
    public RangeTypeObject FrustumRangeObject;

    private MeshRenderer mr;

    private void Start()
    {
        this.mr = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (this.FrustumRangeObject.IsInsideAndNotOccluded(this.BoxObject))
        {
            mr.material.SetColor("_Color", Color.red);
        } else
        {
            mr.material.SetColor("_Color", Color.green);
        }
    }


}
