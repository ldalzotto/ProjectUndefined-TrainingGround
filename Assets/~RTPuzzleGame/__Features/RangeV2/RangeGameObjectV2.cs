using RTPuzzle;
using System;
using UnityEngine;

public class RangeGameObjectV2
{
    private GameObject attachedGameObject;
    private GameObject rangeGameObject;
    private RangeObjectV2 assocaitedRangeObject;

    public RangeObjectV2PhysicsEventListener RangeObjectV2PhysicsEventListener { get; private set; }
    public ObstacleListener ObstacleListener { get; private set; }

    public RangeGameObjectV2(GameObject attachedGameObject)
    {
        this.attachedGameObject = attachedGameObject;
    }

    private Collider boundingBoxCollider;


    private void CommontInit(RangeObjectInitialization RangeObjectInitialization, RangeObjectV2 RangeObjectV2)
    {
        this.assocaitedRangeObject = RangeObjectV2;
        this.rangeGameObject = new GameObject();
        this.rangeGameObject.transform.parent = this.attachedGameObject.transform;
        this.rangeGameObject.transform.localPosition = Vector3.zero;

        var rigidbody = this.rangeGameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = false;

        this.RangeObjectV2PhysicsEventListener = this.rangeGameObject.AddComponent<RangeObjectV2PhysicsEventListener>();

        //TODO -> Remove the unnecessary MonoBehavior ObstacleListener
        if (RangeObjectInitialization.IsTakingIntoAccountObstacles)
        {
            this.ObstacleListener = this.rangeGameObject.AddComponent<ObstacleListener>();
            this.ObstacleListener.Init();
        }

    }

    public void Init(SphereRangeObjectInitialization SphereRangeObjectInitialization, RangeObjectV2 RangeObjectV2)
    {
        this.CommontInit(SphereRangeObjectInitialization, RangeObjectV2);
        var sphereCollider = this.rangeGameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = SphereRangeObjectInitialization.SphereRangeTypeDefinition.Radius;
        this.boundingBoxCollider = sphereCollider;
        this.boundingBoxCollider.isTrigger = true;
    }

    public void Init(BoxRangeObjectInitialization BoxRangeObjectInitialization, RangeObjectV2 RangeObjectV2)
    {
        this.CommontInit(BoxRangeObjectInitialization, RangeObjectV2);
        var boxCollider = this.rangeGameObject.AddComponent<BoxCollider>();
        boxCollider.center = BoxRangeObjectInitialization.BoxRangeTypeDefinition.Center;
        boxCollider.size = BoxRangeObjectInitialization.BoxRangeTypeDefinition.Size;
        this.boundingBoxCollider = boxCollider;
        this.boundingBoxCollider.isTrigger = true;
    }

    public void Init(FrustumRangeObjectInitialization FrustumRangeObjectInitialization, RangeObjectV2 RangeObjectV2)
    {
        this.CommontInit(FrustumRangeObjectInitialization, RangeObjectV2);
        var boxCollider = this.rangeGameObject.AddComponent<BoxCollider>();
        var frustum = FrustumRangeObjectInitialization.FrustumRangeTypeDefinition.FrustumV2;
        boxCollider.center = new Vector3(0, 0, frustum.F2.FaceOffsetFromCenter.z / 4f);
        boxCollider.size = new Vector3(Mathf.Max(frustum.F1.Width, frustum.F2.Width), Math.Max(frustum.F1.Height, frustum.F2.Height), frustum.F2.FaceOffsetFromCenter.z / 2f);
        this.boundingBoxCollider = boxCollider;
        this.boundingBoxCollider.isTrigger = true;
    }

    public void Tick(float d)
    {
        this.assocaitedRangeObject.ExtractData(out RangeTypeObjectMovementSystemExtractedData RangeTypeObjectMovementSystemExtractedData);
        if (RangeTypeObjectMovementSystemExtractedData.LocalPositionChanged)
        {
            this.attachedGameObject.transform.localPosition = RangeTypeObjectMovementSystemExtractedData.LocalPosition;
        }
    }

    public void ReceiveEvent(out RangeObjectV2GetWorldToLocalMatrixEventReturn RangeObjectV2GetWorldToLocalMatrixEventReturn)
    {
        RangeObjectV2GetWorldToLocalMatrixEventReturn = new RangeObjectV2GetWorldToLocalMatrixEventReturn { WorldToLocalMatrix = this.attachedGameObject.transform.worldToLocalMatrix };
    }
    public void ExtractData(out RangeObjectV2GetWorldTransformEventReturn RangeObjectV2GetWorldTransformEventReturn)
    {
        RangeObjectV2GetWorldTransformEventReturn = new RangeObjectV2GetWorldTransformEventReturn
        {
            WorldPosition = this.rangeGameObject.transform.position,
            WorldRotation = this.rangeGameObject.transform.rotation,
            LossyScale = this.rangeGameObject.transform.lossyScale
        };
    }
}

public struct RangeObjectV2GetWorldToLocalMatrixEventReturn
{
    public Matrix4x4 WorldToLocalMatrix;
}

public struct RangeObjectV2GetWorldTransformEventReturn
{
    public Vector3 WorldPosition;
    public Quaternion WorldRotation;
    public Vector3 LossyScale;
}