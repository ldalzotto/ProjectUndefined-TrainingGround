using UnityEngine;
using System.Collections;
using CoreGame;

namespace Tests
{
    public class TestAdventureGameManager : AdventureGame.GameManager
    {
        protected override void AfterGameManagerPersistanceInstanceInitialization()
        {
            //No persistance
            var persistanceManager = GameObject.FindObjectOfType<PersistanceManager>();
            var persistanceManagerGO = persistanceManager.gameObject;
            MonoBehaviour.DestroyImmediate(persistanceManager);
            persistanceManagerGO.AddComponent<MockPersistanceManager>();
        }
    }
}
