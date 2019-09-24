using UnityEngine;


public class CollisionType : MonoBehaviour
{
    [Header("Adventure")]
    public bool IsPoi;

    [Header("RT_Puzzle")]
    public bool IsPlayer = false;
    public bool IsRTAttractiveObject = false;
    public bool IsAI = false;
    public bool IsTargetZone = false;

    [Header("RT_Puzzle_Range")]
    public bool IsRange = false;
    public bool IsObstacle = false;

    [Header("RT_Puzzle object behaior")]
    public bool IsRepelable = false;

    private Collider associatedCollider;
    public Collider GetAssociatedCollider()
    {
        if (this.associatedCollider == null) { this.associatedCollider = this.GetComponent<Collider>(); }
        return this.associatedCollider;
    }
}
