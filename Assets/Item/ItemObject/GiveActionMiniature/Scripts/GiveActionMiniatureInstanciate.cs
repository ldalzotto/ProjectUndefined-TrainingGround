using UnityEngine;

public class GiveActionMiniatureInstanciate
{

    public static GameObject Instance(GameObject itemModel, Transform parent, Vector3 itemModelScaleFactor)
    {
        var giveActionMiniature = MonoBehaviour.Instantiate(PrefabContainer.Instance.GiveActionMiniaturePrefab, parent, false);
        var itemModelInstance = MonoBehaviour.Instantiate(itemModel, giveActionMiniature.transform, false);
        itemModelInstance.transform.localScale = new Vector3(
              itemModelInstance.transform.localScale.x / itemModelScaleFactor.x,
              itemModelInstance.transform.localScale.y / itemModelScaleFactor.y,
              itemModelInstance.transform.localScale.z / itemModelScaleFactor.z);
        itemModelInstance.transform.localPosition = Vector3.zero;
        itemModelInstance.transform.localRotation = Quaternion.identity;
        return giveActionMiniature;
    }

}
