using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    private List<CarManager> Cars = new List<CarManager>();

    #region External Events
    public void AddCar(CarManager carManager)
    {
        Cars.Add(carManager);
    }
    #endregion

    public void Tick(float d)
    {
        foreach (var car in Cars)
        {
            car.Tick(d);
        }
    }

    public void FixedTick(float d)
    {
        foreach (var car in Cars)
        {
            car.FixedTick(d);
        }
    }
}
