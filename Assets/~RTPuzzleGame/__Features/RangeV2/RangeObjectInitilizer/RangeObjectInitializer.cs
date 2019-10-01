using UnityEngine;
using UnityEngine.Profiling;

public class RangeObjectInitializer : MonoBehaviour
{
    [Inline()]
    public RangeObjectInitialization RangeObjectInitialization;

    public void Init()
    {
        Profiler.BeginSample("RangeObjectInitializer : Init");
        RangeObjectV2 rangeObjectV2 = null;
        if (RangeObjectInitialization.GetType() == typeof(SphereRangeObjectInitialization))
        {
            rangeObjectV2 = new SphereRangeObjectV2(new RangeGameObjectV2(this.gameObject), (SphereRangeObjectInitialization)this.RangeObjectInitialization);
        }
        else if (RangeObjectInitialization.GetType() == typeof(BoxRangeObjectInitialization))
        {
            rangeObjectV2 = new BoxRangeObjectV2(new RangeGameObjectV2(this.gameObject), (BoxRangeObjectInitialization)this.RangeObjectInitialization);
        }
        else if(RangeObjectInitialization.GetType() == typeof(FrustumRangeObjectInitialization))
        {
            rangeObjectV2 = new FrustumRangeObjectV2(new RangeGameObjectV2(this.gameObject), (FrustumRangeObjectInitialization)this.RangeObjectInitialization);
        }
        RangeObjectV2Manager.Get().ReceiveEvent(new RangeObjectV2ManagerAddRangeEvent { AddedRangeObject = rangeObjectV2 });
        MonoBehaviour.Destroy(this);
        Profiler.EndSample();
    }
}
