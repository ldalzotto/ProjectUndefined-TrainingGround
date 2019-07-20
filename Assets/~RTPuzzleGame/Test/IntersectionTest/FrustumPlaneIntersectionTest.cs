using CoreGame;
using RTPuzzle;
using UnityEngine;

public class FrustumPlaneIntersectionTest : MonoBehaviour
{
    public LineObjTest LineObjTest;
    public BoxCollider BoxObject;
    public FrustumRangeType FrustumRangeType;

    private bool intersect = false;

    private void Start()
    {
    }

    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (this.LineObjTest != null && this.FrustumRangeType != null && this.BoxObject != null)
        {
            var frustumPoints = this.FrustumRangeType.GetFrustumPointsWorldPositions();
            this.intersect = Intersection.FrustumBoxIntersection(frustumPoints, this.BoxObject) || Intersection.BoxEntirelyContainedInFrustum(frustumPoints, this.BoxObject);
         //   this.intersect = Intersection.LineFrustumIntersection(this.LineObjTest.WorldStart, this.LineObjTest.WorldEnd, this.FrustumRangeType.GetFrustumPointsWorldPositions());
        }

        var oldGizmoColor = Gizmos.color;
        if (this.intersect)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        if (this.BoxObject != null)
        {
            Gizmos.DrawWireSphere(this.BoxObject.transform.position, 0.3f);
        }
        Gizmos.color = oldGizmoColor;
    }

}
