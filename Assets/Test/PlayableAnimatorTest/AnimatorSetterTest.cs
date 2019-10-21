using UnityEngine;

public class AnimatorSetterTest : MonoBehaviour
{
    [Range(0f, 1f)] public float Speed;
    private Animator anim;

    private void Start()
    {
        this.anim = this.GetComponent<Animator>();
    }

    private void Update()
    {
        this.anim.SetFloat("Speed", this.Speed);
    }
}