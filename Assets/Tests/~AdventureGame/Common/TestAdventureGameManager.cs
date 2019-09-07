using CoreGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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

            //No input
            GameTestMockedInputManager.SetupForTestScene();
        }
    }

}
