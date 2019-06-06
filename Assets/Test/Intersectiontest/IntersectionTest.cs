using CoreGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionTest : MonoBehaviour
{

    public MyBox MyBox;
    public Circle Circle;

    private void Update()
    {
        Debug.Log(Intersection.BoxIntersectsSphere(MyBox.BoxCollider, Circle.transform.position, Circle.Radius));
    }
}
