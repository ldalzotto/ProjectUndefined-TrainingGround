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
            var playerAnimator = PlayerManager.GetPlayerAnimator();
            var boneObj = PlayerBoneRetriever.GetPlayerBone(PlayerBone, playerAnimator);
            if (boneObj != null)
            {
                BoneTransformResolved = boneObj.transform;
            }

            if (AdventureGamePrefabContainer.InventoryItemsPrefabs != null)
            {
                var scaleFactor = Vector3.one;
                ComponentSearchHelper.ComputeScaleFactorRecursively(BoneTransformResolved, playerAnimator.transform, ref scaleFactor);

                InstanciatedObject = MonoBehaviour.Instantiate(AdventureGamePrefabContainer.InventoryItemsPrefabs[HoldedItem].ItemModel, BoneTransformResolved.transform);
                InstanciatedObject.transform.localScale = new Vector3(
                        InstanciatedObject.transform.localScale.x / scaleFactor.x,
                        InstanciatedObject.transform.localScale.y / scaleFactor.y,
                        InstanciatedObject.transform.localScale.z / scaleFactor.z
                    );
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
}