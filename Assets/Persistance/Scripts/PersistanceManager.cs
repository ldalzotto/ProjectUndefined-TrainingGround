using System.Collections.Generic;
using UnityEngine;

public class PersistanceManager : MonoBehaviour
{

    private Dictionary<string, PersistanceData> persistedData = new Dictionary<string, PersistanceData>();

    #region External Events
    public void SavePersistantable(IPersistantable persistData)
    {
        persistedData[persistData.PersistanceID()] = persistData.PersistanceData();
    }

    public void SavePersistantables(List<IPersistantable> persistDatas)
    {
        foreach (var persistData in persistDatas)
        {
            SavePersistantable(persistData);
        }
    }

    public void Load(IPersistantable persistantableLoad)
    {
        if (persistedData.ContainsKey(persistantableLoad.PersistanceID()))
        {
            persistantableLoad.Load(persistedData[persistantableLoad.PersistanceID()]);
        }
    }

    public void LoadAll(List<IPersistantable> persistantablesLoad)
    {
        foreach (var persistantableLoad in persistantablesLoad)
        {
            Load(persistantableLoad);
        }
    }
    #endregion
}

public interface IPersistantable
{
    PersistanceData PersistanceData();
    void Load(PersistanceData persistanceData);
    string PersistanceID();
}