using UnityEngine;
using System.Collections;

public class ShaderVectorTest : MonoBehaviour
{
    public GameObject Center;
    public MeshRenderer groundRenderer;
    public float MaxAngle = 180f;

    // Update is called once per frame
    void Update()
    {

        groundRenderer.material.SetVector("_Center", this.Center.transform.position);
        groundRenderer.material.SetVector("_ForwardDirection", this.Center.transform.forward);
        groundRenderer.material.SetFloat("_MaxAngle", this.MaxAngle);
    }
}
