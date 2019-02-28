using UnityEngine;

public class CarSpawnerManager : MonoBehaviour
{

    #region External Dependencies
    private NPCManager NPCManager;
    #endregion

    private void Start()
    {
        NPCManager = GameObject.FindObjectOfType<NPCManager>();
        Spawn();
    }

    #region External Events
    public void Spawn()
    {
        var newCarManager = Instantiate(AdventureGamePrefabContainer.Instance.CarManagerPrefab, this.transform);
        var randomPathId = RandomHelper.RandomBetweenEnumValue<WaypointPathId>();
        newCarManager.PrefabInit(randomPathId);
        NPCManager.AddCar(newCarManager);
    }

    internal void DestroyCar(CarManager carManager)
    {
        NPCManager.RemoveCar(carManager);
    }
    #endregion

}
