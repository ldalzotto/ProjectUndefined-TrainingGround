using UnityEngine;

public class FXContainerManager : MonoBehaviour
{
    public TriggerableEffect TriggerFX(TriggerableEffect FXPrefab, Transform worldTransform)
    {
        var fx = Instantiate(FXPrefab);
        fx.TriggerEffect(() => { Destroy(fx.gameObject); });
        return fx;
    }
}
