using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    private Dictionary<int, CarManager> Cars = new Dictionary<int, CarManager>();

    #region External Events
    public void AddCar(CarManager carManager)
    {
        Cars.Add(carManager.GetInstanceID(), carManager);
    }
    public void RemoveCar(CarManager carManager)
    {
        Cars.Remove(carManager.GetInstanceID());
    }
    #endregion

    public void Tick(float d)
    {
        foreach (var car in Cars)
        {
            car.Value.Tick(d);
        }
    }

    public void FixedTick(float d)
    {
        foreach (var car in Cars)
        {
            car.Value.FixedTick(d);
        }
    }
}
