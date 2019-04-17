using CoreGame;
using UnityEditor;
using UnityEngine;

public class RotationFollowTestScript : MonoBehaviour
{

    public GameObject target;
    public float maxAngle;

    private float targetAngle;

    private void LateUpdate()
    {
        var rotationQuaternion = Quaternion.LookRotation(target.transform.position - transform.position, transform.up);
        var forwardDirection = (transform.forward).normalized;
        var targetDirection = (target.transform.position - transform.position).normalized;
        Debug.DrawLine(transform.position, transform.position + (forwardDirection * 10), Color.blue);
        Debug.DrawLine(transform.position, transform.position + (targetDirection * 10), Color.green);
        var adjustedRotation = QuaternionHelper.ConeReduction(forwardDirection, targetDirection, this.maxAngle);
        var interpolatedDirection = (adjustedRotation * targetDirection).normalized;
        Debug.DrawLine(transform.position, transform.position + (interpolatedDirection * 10), Color.red);
        transform.Rotate(rotationQuaternion.eulerAngles);
    }



    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, targetAngle.ToString());
    }

}
