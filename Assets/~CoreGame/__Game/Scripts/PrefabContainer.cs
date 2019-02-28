using System.Collections.Generic;
using UnityEngine;

public class PrefabContainer : MonoBehaviour
{

    private static PrefabContainer instance;

    public GameObject ActionWheelNodePrefab;
    public GameObject InventoryMenuCellPrefab;
    public GameObject GiveActionMiniaturePrefab;

    [Header("Player FX")]
    public TriggerableEffect PlayerSmokeEffectPrefab;

    [Header("Projectile Prefabs")]
    [Space(20)]
    [Header("RT Puzzle Prefabs")]
    [Space(20)]
    public RTPuzzle.LaunchProjectile ProjectilePrefab;
    public RTPuzzle.ThrowProjectilePath ThrowProjectilePathPrefab;

    [Header("NPC prefabs")]
    public RTPuzzle.NpcInteractionRingType NpcInteractionRingPrefab;

    [Header("Cooldown Feed Prefabs")]
    public RTPuzzle.CooldownFeedManager CooldownFeedManager;
    public RTPuzzle.CooldownFeedLineType CooldownFeedLineType;

    public static PrefabContainer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<PrefabContainer>();
            }
            return instance;
        }
    }

}
