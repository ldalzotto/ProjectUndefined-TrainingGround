using System.Collections.Generic;
using UnityEngine;

public class CameraObstructManager : MonoBehaviour
{
    public ObstructionMaterialComponent ObstructionMaterialComponent;

    private CameraObstructPositionerManager CameraObstructPositionerManager;
    private ObstructionTrackerManager ObstructionTrackerManager;

    // Use this for initialization
    public void Init()
    {
        #region External Dependecies
        var playerManager = GameObject.FindObjectOfType<PlayerManager>();
        var cameraObstructTracker = playerManager.GetCameraObstructTrackerType();
        #endregion

        this.CameraObstructPositionerManager = new CameraObstructPositionerManager(cameraObstructTracker, Camera.main);
        this.ObstructionTrackerManager = new ObstructionTrackerManager(ObstructionMaterialComponent);
    }

    public void Tick(float d)
    {
        CameraObstructPositionerManager.Tick(d);
    }

    #region External Events
    public void TriggerEnter(Collider other, CollisionType collisionType)
    {
        if (collisionType.IsCameraObstructable)
        {
            Debug.Log("OBS");
            ObstructionTrackerManager.OnObjectEnter(other.gameObject);
        }
    }
    public void TriggerExit(Collider other, CollisionType collisionType)
    {
        if (collisionType.IsCameraObstructable)
        {
            ObstructionTrackerManager.OnObjectExit(other.gameObject);
        }
    }
    #endregion

}

#region Obstruction Position Manager
class CameraObstructPositionerManager
{
    private CameraObstructTrackerType CameraObstructTrackerType;
    private Camera MainCamera;

    public CameraObstructPositionerManager(CameraObstructTrackerType cameraObstructTrackerType, Camera mainCamera)
    {
        CameraObstructTrackerType = cameraObstructTrackerType;
        MainCamera = mainCamera;
    }

    public void Tick(float d)
    {
        CameraObstructTrackerType.transform.LookAt(MainCamera.transform);
        CameraObstructTrackerType.transform.localEulerAngles = new Vector3(-CameraObstructTrackerType.transform.localEulerAngles.x, CameraObstructTrackerType.transform.localEulerAngles.y, -CameraObstructTrackerType.transform.localEulerAngles.z);
    }
}
#endregion

#region Obstruction Tracker Manager
[System.Serializable]
public class ObstructionMaterialComponent
{
    public Material ObstructionMaterial;
}

class ObstructionTrackerManager
{

    private ObstructionMaterialComponent ObstructionMaterialComponent;
    private Dictionary<int, Material> OriginalMaterials = new Dictionary<int, Material>();

    public ObstructionTrackerManager(ObstructionMaterialComponent obstructionMaterialComponent)
    {
        ObstructionMaterialComponent = obstructionMaterialComponent;
    }

    public void OnObjectEnter(GameObject obj)
    {
        var meshRenderer = obj.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            var newMaterial = new Material(ObstructionMaterialComponent.ObstructionMaterial);
            newMaterial.CopyPropertiesFromMaterial(meshRenderer.material);
            newMaterial.SetFloat("_CircleRadius", 0.25f);

            OriginalMaterials[obj.GetInstanceID()] = meshRenderer.material;
            meshRenderer.material = newMaterial;
        }
    }
    public void OnObjectExit(GameObject obj)
    {
        var meshRenderer = obj.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material = OriginalMaterials[obj.GetInstanceID()];
            OriginalMaterials[obj.GetInstanceID()] = null;
        }
    }
}
#endregion