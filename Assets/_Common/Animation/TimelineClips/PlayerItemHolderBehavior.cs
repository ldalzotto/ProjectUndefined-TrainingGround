using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class PlayerItemHolderBehavior : PlayableBehaviour
{
    public PlayerBone PlayerBone;
    public ItemID HoldedItem;

    private Transform BoneTransformResolved;
    private GameObject InstanciatedObject;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (Application.isPlaying)
        {
            #region External Dependencies
            var PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            #endregion
            var boneObj = PlayerBoneRetriever.GetPlayerBone(PlayerBone, PlayerManager.GetPlayerAnimator());
            if (boneObj != null)
            {
                BoneTransformResolved = boneObj.transform;
            }

            if (PrefabContainer.InventoryItemsPrefabs != null)
            {
                var AniamtionObjectContainer = GameObject.FindGameObjectWithTag(TagConstants.ANIMATION_OBJECT_CONTAINER_TAG);
                InstanciatedObject = MonoBehaviour.Instantiate(PrefabContainer.InventoryItemsPrefabs[HoldedItem].ItemModel, AniamtionObjectContainer.transform);
            }

            base.OnBehaviourPlay(playable, info);
        }

    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (InstanciatedObject != null)
        {
            MonoBehaviour.Destroy(InstanciatedObject);
        }
        base.OnBehaviourPause(playable, info);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (BoneTransformResolved != null)
        {
            InstanciatedObject.transform.position = BoneTransformResolved.position;
            InstanciatedObject.transform.rotation = BoneTransformResolved.rotation;
        }
        base.ProcessFrame(playable, info, playerData);
    }
}