using CoreGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBox : MonoBehaviour
{
    public BoxCollider BoxCollider;
    // Start is called before the first frame update
    void Start()
    {
        this.BoxCollider = GetComponent<BoxCollider>();
    }

    private void OnDrawGizmos()
    {
        if (this.BoxCollider != null)
        {
            Gizmos.DrawCube(this.transform.position + this.BoxCollider.center, this.BoxCollider.size);
            /*
            Gizmos.DrawWireSphere(Intersection.boxPosition, 0.1f);
            Gizmos.DrawWireSphere(Intersection.C1, 0.1f);
            Gizmos.DrawWireSphere(Intersection.C2, 0.1f);
            Gizmos.DrawWireSphere(Intersection.C3, 0.1f);
            Gizmos.DrawWireSphere(Intersection.C4, 0.1f);
            Gizmos.DrawWireSphere(Intersection.C5, 0.1f);
            Gizmos.DrawWireSphere(Intersection.C6, 0.1f);
            Gizmos.DrawWireSphere(Intersection.C7, 0.1f);
            Gizmos.DrawWireSphere(Intersection.C8, 0.1f);
            */
        }
        
    }

}
