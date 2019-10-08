using CoreGame;
using RTPuzzle;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

public interface IIntersectionManager
{
    void ClearState();
    void CountingForRangeIntersectionCalculator(RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2);
    void CalculationDataSetupForRangeIntersectionCalculator(RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2,
        IsOccludedByObstacleJobData IsOccludedByObstacleJobData, int currentObstacleIntersectionCalculatorCounter);
    void BuildJobHandle(NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData, RangeObstacleOcclusionIntersection RangeObstacleOcclusionIntersection);
    void Complete();
    void WaitForResults();
    void CreateNativeArrays();
    void Dispose();
}

public struct RoundedFrustumIntersectionManager : IIntersectionManager
{
    private NativeArray<RoundedFrustumIntersectionJobData> RoundedFrustumIntersectionJobData;
    public NativeArray<RangeIntersectionResult> RoundedFrustumIntersectionJobResult { get; private set; }

    private JobHandle JobHandle;

    private int totalRoundedFrustumTypeIntersection;
    private int currentRoundedFrustumIntersectionJobDataCounter;

    public void ClearState()
    {
        this.totalRoundedFrustumTypeIntersection = 0;
        this.currentRoundedFrustumIntersectionJobDataCounter = 0;
    }

    public void CountingForRangeIntersectionCalculator(RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2)
    {
        if (RangeIntersectionCalculatorV2.GetAssociatedRangeObjectType() == RangeType.ROUNDED_FRUSTUM)
        {
            this.totalRoundedFrustumTypeIntersection += 1;
        }
    }

    public void CalculationDataSetupForRangeIntersectionCalculator(RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2,
        IsOccludedByObstacleJobData IsOccludedByObstacleJobData, int currentObstacleIntersectionCalculatorCounter)
    {
        if (RangeIntersectionCalculatorV2.GetAssociatedRangeObjectType() == RangeType.ROUNDED_FRUSTUM)
        {
            var RoundedFrustumRangeObject = (RoundedFrustumRangeObjectV2)RangeIntersectionCalculatorV2.GetAssociatedRangeObject();
            var RoundedFrustumIntersectionJobData = new RoundedFrustumIntersectionJobData
            {
                RangeIntersectionCalculatorV2UniqueID = RangeIntersectionCalculatorV2.RangeIntersectionCalculatorV2UniqueID,
                FrustumRadius = RoundedFrustumRangeObject.GetFrustum().GetFrustumFaceRadius(),
                RangeTransform = RoundedFrustumRangeObject.GetTransform(),
                IsOccludedByObstacleJobData = IsOccludedByObstacleJobData,
                RoundedFrustumPositions = RoundedFrustumRangeObject.GetFrustumWorldPositions(),
                ComparedCollider = RangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.GetLogicColliderBoxDefinition(),
                ObstacleCalculationDataIndex = RoundedFrustumRangeObject.GetObstacleListener() == null ? -1 : (currentObstacleIntersectionCalculatorCounter - 1)
            };
            this.RoundedFrustumIntersectionJobData[currentRoundedFrustumIntersectionJobDataCounter] = RoundedFrustumIntersectionJobData;
            currentRoundedFrustumIntersectionJobDataCounter += 1;
        }
    }

    public void BuildJobHandle(NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData, RangeObstacleOcclusionIntersection RangeObstacleOcclusionIntersection)
    {
        this.JobHandle = new RoundedFrustumIntersectionJob
        {
            RoundedFrustumIntersectionJobData = this.RoundedFrustumIntersectionJobData,
            IsOccludedByObstacleJobData = IsOccludedByObstacleJobData,
            AssociatedObstacleFrustumPointsPositions = RangeObstacleOcclusionIntersection.AssociatedObstacleFrustumPointsPositions,
            IntersectionResult = this.RoundedFrustumIntersectionJobResult
        }.Schedule(this.totalRoundedFrustumTypeIntersection, 5);
    }

    public void Complete()
    {
        this.JobHandle.Complete();
    }

    public void WaitForResults()
    {
        while (!this.JobHandle.IsCompleted) { }
    }

    public void CreateNativeArrays()
    {
        this.RoundedFrustumIntersectionJobData = new NativeArray<RoundedFrustumIntersectionJobData>(totalRoundedFrustumTypeIntersection, Allocator.TempJob);
        this.RoundedFrustumIntersectionJobResult = new NativeArray<RangeIntersectionResult>(totalRoundedFrustumTypeIntersection, Allocator.TempJob);
    }

    public void Dispose()
    {
        if (this.RoundedFrustumIntersectionJobData.IsCreated) { this.RoundedFrustumIntersectionJobData.Dispose(); }
        if (this.RoundedFrustumIntersectionJobResult.IsCreated) { this.RoundedFrustumIntersectionJobResult.Dispose(); }
    }
}

[BurstCompile(CompileSynchronously = true)]
public struct RoundedFrustumIntersectionJob : IJobParallelFor
{
    public NativeArray<RoundedFrustumIntersectionJobData> RoundedFrustumIntersectionJobData;
    public NativeArray<RangeIntersectionResult> IntersectionResult;

    [ReadOnly]
    public NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;
    [ReadOnly]
    public NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions;

    public void Execute(int RoundedFrustumIntersectionDataIndex)
    {
        var RoundedFrustumIntersectionJobData = this.RoundedFrustumIntersectionJobData[RoundedFrustumIntersectionDataIndex];
        bool isInsideRange = IsInside(RoundedFrustumIntersectionJobData);
        if (RoundedFrustumIntersectionJobData.ObstacleCalculationDataIndex != -1 && isInsideRange)
        {
            isInsideRange = isInsideRange && !this.IsOccludedByObstacleJobData[RoundedFrustumIntersectionJobData.ObstacleCalculationDataIndex].IsOccluded(this.AssociatedObstacleFrustumPointsPositions);
        }
        this.IntersectionResult[RoundedFrustumIntersectionDataIndex] =
            new RangeIntersectionResult
            {
                RangeIntersectionCalculatorV2UniqueID = RoundedFrustumIntersectionJobData.RangeIntersectionCalculatorV2UniqueID,
                IsInsideRange = isInsideRange
            };
    }

    private bool IsInside(RoundedFrustumIntersectionJobData RoundedFrustumIntersectionJobData)
    {
        return Intersection.BoxIntersectsOrEntirelyContainedInSphere(RoundedFrustumIntersectionJobData.ComparedCollider, RoundedFrustumIntersectionJobData.RangeTransform.WorldPosition, RoundedFrustumIntersectionJobData.FrustumRadius)
                && (Intersection.FrustumBoxIntersection(RoundedFrustumIntersectionJobData.RoundedFrustumPositions, RoundedFrustumIntersectionJobData.ComparedCollider) || Intersection.BoxEntirelyContainedInFrustum(RoundedFrustumIntersectionJobData.RoundedFrustumPositions, RoundedFrustumIntersectionJobData.ComparedCollider))
         ;
    }
}

public struct RangeIntersectionResult
{
    public int RangeIntersectionCalculatorV2UniqueID;
    public bool IsInsideRange;
}

public struct RoundedFrustumIntersectionJobData
{
    public int RangeIntersectionCalculatorV2UniqueID;
    public TransformStruct RangeTransform;
    public FrustumPointsPositions RoundedFrustumPositions;
    public float FrustumRadius;
    public BoxDefinition ComparedCollider;
    public IsOccludedByObstacleJobData IsOccludedByObstacleJobData;
    public int ObstacleCalculationDataIndex;
}


public struct SphereIntersectionManager : IIntersectionManager
{
    private int totalSphereTypeIntersection;
    private int currentSphereIntersectionJobdataCounter;

    private NativeArray<SphereIntersectionJobData> SphereIntersectionJobData;
    public NativeArray<RangeIntersectionResult> SphereIntersectionJobResult { get; private set; }

    private JobHandle JobHandle;

    public void CountingForRangeIntersectionCalculator(RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2)
    {
        if (RangeIntersectionCalculatorV2.GetAssociatedRangeObjectType() == RangeType.SPHERE)
        {
            this.totalSphereTypeIntersection += 1;
        }
    }

    public void BuildJobHandle(NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData, RangeObstacleOcclusionIntersection RangeObstacleOcclusionIntersection)
    {
        this.JobHandle = new SphereIntersectionJob
        {
            SphereIntersectionJobDatas = this.SphereIntersectionJobData,
            IsOccludedByObstacleJobData = IsOccludedByObstacleJobData,
            AssociatedObstacleFrustumPointsPositions = RangeObstacleOcclusionIntersection.AssociatedObstacleFrustumPointsPositions,
            IntersectionResult = this.SphereIntersectionJobResult
        }.Schedule(totalSphereTypeIntersection, 5);
    }

    public void CalculationDataSetupForRangeIntersectionCalculator(RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2,
        IsOccludedByObstacleJobData IsOccludedByObstacleJobData, int currentObstacleIntersectionCalculatorCounter)
    {
        if (RangeIntersectionCalculatorV2.GetAssociatedRangeObjectType() == RangeType.SPHERE)
        {
            var SphereRangeObject = (SphereRangeObjectV2)RangeIntersectionCalculatorV2.GetAssociatedRangeObject();
            var SphereIntersectionJobData = new SphereIntersectionJobData
            {
                RangeIntersectionCalculatorV2UniqueID = RangeIntersectionCalculatorV2.RangeIntersectionCalculatorV2UniqueID,
                RangeTransform = SphereRangeObject.GetTransform(),
                IsOccludedByObstacleJobData = IsOccludedByObstacleJobData,
                ComparedCollider = RangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.GetLogicColliderBoxDefinition(),
                ObstacleCalculationDataIndex = SphereRangeObject.GetObstacleListener() == null ? -1 : (currentObstacleIntersectionCalculatorCounter - 1),
                SphereRadius = SphereRangeObject.SphereBoundingCollider.radius
            };
            this.SphereIntersectionJobData[this.currentSphereIntersectionJobdataCounter] = SphereIntersectionJobData;
            this.currentSphereIntersectionJobdataCounter += 1;
        }
    }

    public void ClearState()
    {
        this.totalSphereTypeIntersection = 0;
        this.currentSphereIntersectionJobdataCounter = 0;
    }

    public void Complete()
    {
        this.JobHandle.Complete();
    }

    public void CreateNativeArrays()
    {
        this.SphereIntersectionJobData = new NativeArray<SphereIntersectionJobData>(this.totalSphereTypeIntersection, Allocator.TempJob);
        this.SphereIntersectionJobResult = new NativeArray<RangeIntersectionResult>(this.totalSphereTypeIntersection, Allocator.TempJob);
    }

    public void Dispose()
    {
        if (this.SphereIntersectionJobData.IsCreated) { this.SphereIntersectionJobData.Dispose(); }
        if (this.SphereIntersectionJobResult.IsCreated) { this.SphereIntersectionJobResult.Dispose(); }
    }

    public void WaitForResults()
    {
        while (!this.JobHandle.IsCompleted) { }
    }
}


[BurstCompile(CompileSynchronously = true)]
public struct SphereIntersectionJob : IJobParallelFor
{
    public NativeArray<SphereIntersectionJobData> SphereIntersectionJobDatas;
    public NativeArray<RangeIntersectionResult> IntersectionResult;

    [ReadOnly]
    public NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;
    [ReadOnly]
    public NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions;

    public void Execute(int SphereIntersectionJobDataIndex)
    {
        var SphereIntersectionJobData = this.SphereIntersectionJobDatas[SphereIntersectionJobDataIndex];
        bool isInsideRange = Intersection.BoxIntersectsOrEntirelyContainedInSphere(SphereIntersectionJobData.ComparedCollider, SphereIntersectionJobData.RangeTransform.WorldPosition, SphereIntersectionJobData.SphereRadius);
        if (SphereIntersectionJobData.ObstacleCalculationDataIndex != -1 && isInsideRange)
        {
            isInsideRange = isInsideRange && !this.IsOccludedByObstacleJobData[SphereIntersectionJobData.ObstacleCalculationDataIndex].IsOccluded(this.AssociatedObstacleFrustumPointsPositions);
        }
        this.IntersectionResult[SphereIntersectionJobDataIndex] =
          new RangeIntersectionResult
          {
              RangeIntersectionCalculatorV2UniqueID = SphereIntersectionJobData.RangeIntersectionCalculatorV2UniqueID,
              IsInsideRange = isInsideRange
          };
    }
}


public struct SphereIntersectionJobData
{
    public int RangeIntersectionCalculatorV2UniqueID;
    public TransformStruct RangeTransform;
    public float SphereRadius;
    public BoxDefinition ComparedCollider;
    public IsOccludedByObstacleJobData IsOccludedByObstacleJobData;
    public int ObstacleCalculationDataIndex;
}
