﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Sphere : MonoBehaviour
{
    public float Radius;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, this.Radius);
    }
}
