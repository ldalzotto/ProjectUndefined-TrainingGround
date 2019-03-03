using UnityEngine;

public class FXContainerManager : MonoBehaviour
{
    public TriggerableEffect TriggerFX(TriggerableEffect FXPrefab, Transform worldTransform)
    {
        var fx = Instantiate(FXPrefab, this.transform, true);
        fx.transform.position = worldTransform.position;
        fx.transform.rotation = worldTransform.rotation;
        fx.Init();
        fx.TriggerEffect(() => { Destroy(fx.gameObject); });
        return fx;
    }

    public TriggerableEffect TriggerFX(TriggerableEffect FXPrefab)
    {
        var fx = Instantiate(FXPrefab, this.transform, true);
        fx.Init();
        fx.TriggerEffect(() => { Destroy(fx.gameObject); });
        return fx;
    }
}
