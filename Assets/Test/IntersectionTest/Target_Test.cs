using UnityEngine;

public class Target_Test : MonoBehaviour
{

    public ObstacleListener Enemy;

    private bool isSeen;

    private void Update()
    {
        this.isSeen = this.Enemy.CanSee(this.transform.position);
    }

    private void OnDrawGizmos()
    {
        var oldGizmoColor = Gizmos.color;
        if (this.isSeen)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(this.transform.position, 0.1f);

        Gizmos.color = oldGizmoColor;
    }
}
