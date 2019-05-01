using UnityEngine;

public class CollisionType : MonoBehaviour
{
    public bool IsPoi;

    [Header("RT_Puzzle")]
  //  public bool IsRTPProjectile = false;
    public bool IsRTAttractiveObject = false;
    public bool IsAI = false;
    public bool IsTargetZone = false;

    [Header("RT_Puzzle_Range")]
    public bool IsRange = false;
}
