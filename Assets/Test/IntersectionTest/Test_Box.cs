using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Box : MonoBehaviour
{
    public BoxCollider BoxCollider;

    private void Start()
    {
        this.BoxCollider = GetComponent<BoxCollider>();
    }
}
