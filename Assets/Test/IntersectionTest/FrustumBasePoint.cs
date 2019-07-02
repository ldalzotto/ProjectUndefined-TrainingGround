using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class FrustumBasePoint : MonoBehaviour
{
    public List<Test_Frustum> Test_Frustums;

    private void Update()
    {
        if (this.Test_Frustums != null)
        {
            foreach(var Test_Frustum in Test_Frustums)
            {
                Test_Frustum.LocalFrustumV2.SetLocalStartAngleProjection(this.transform.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, 0.1f);
    }
}
