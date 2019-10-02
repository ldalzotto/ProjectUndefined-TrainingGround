using UnityEngine;
using UnityEngine.Profiling;

public class RangeObjectInitializer : MonoBehaviour
{
    [Inline()]
    public RangeObjectInitialization RangeObjectInitialization;

    public void Init()
    {
        Profiler.BeginSample("RangeObjectInitializer : Init");
        FromRangeObjectInitialization(RangeObjectInitialization, this.gameObject);
        MonoBehaviour.Destroy(this);
        Profiler.EndSample();
    }

    public static RangeObjectV2 FromRangeObjectInitialization(RangeObjectInitialization RangeObjectInitialization, GameObject parent)
    {
        RangeObjectV2 rangeObjectV2 = null;
        if (RangeObjectInitialization.GetType() == typeof(SphereRangeObjectInitialization))
        {
            rangeObjectV2 = new SphereRangeObjectV2(new RangeGameObjectV2(parent), (SphereRangeObjectInitialization)RangeObjectInitialization);
        }
        else if (RangeObjectInitialization.GetType() == typeof(BoxRangeObjectInitialization))
        {
            rangeObjectV2 = new BoxRangeObjectV2(new RangeGameObjectV2(parent), (BoxRangeObjectInitialization)RangeObjectInitialization);
        }
        else if (RangeObjectInitialization.GetType() == typeof(FrustumRangeObjectInitialization))
        {
            rangeObjectV2 = new FrustumRangeObjectV2(new RangeGameObjectV2(parent), (FrustumRangeObjectInitialization)RangeObjectInitialization);
        }
        else if (RangeObjectInitialization.GetType() == typeof(RoundedFrustumRangeObjectInitialization))
        {
            rangeObjectV2 = new RoundedFrustumRangeObjectV2(new RangeGameObjectV2(parent), (RoundedFrustumRangeObjectInitialization)RangeObjectInitialization);
        }
        return rangeObjectV2;
    }
}
