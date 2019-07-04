using CoreGame;
using UnityEngine;

public class Test_Intersection : MonoBehaviour
{
    public Test_Box Test_Box;
    public Vector3 Point;
    public ObstacleListener Test_Sphere;
    
    private void OnDrawGizmos()
    {
        this.Point = transform.position;
        if (Test_Box != null && this.Test_Sphere != null)
        {
            var oldGizmoColor = Gizmos.color;
            if (Intersection.BoxIntersectsSphereV2(this.Test_Box.BoxCollider, this.Test_Sphere.transform.position, this.Test_Sphere.Radius))
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(this.Test_Sphere.transform.position, 0.1f);
            Gizmos.color = oldGizmoColor;
        }
    }
}
