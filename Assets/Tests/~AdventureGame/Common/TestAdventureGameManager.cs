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
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            var gameInputManagerGO = gameInputManager.gameObject;
            MonoBehaviour.DestroyImmediate(gameInputManager);
            gameInputManagerGO.AddComponent<AdventureTstMockedInputManager>();
        }
    }

    class AdventureTstMockedInputManager : GameInputManager
    {
        public override void Init()
        {
            this.currentInput = new AdventureTestMockedXInput();
        }

        public override Dictionary<Key, KeyControl> GetKeyToKeyControlLookup()
        {
            return new Dictionary<Key, KeyControl>();
        }

        public AdventureTestMockedXInput GetAdventureTestMockedXInput()
        {
            return (AdventureTestMockedXInput)this.currentInput;
        }
    }

    class AdventureTestMockedXInput : XInput
    {
        public bool switchSelectionButtonD = false;
        public bool actionButtonD = false;

        #region Interface implementation
        public bool ActionButtonD()
        {
            return actionButtonD;
        }

        public bool CancelButtonD()
        {
            return false;
        }

        public bool CancelButtonDH()
        {
            return false;
        }

        public Vector3 CursorDisplacement()
        {
            return Vector3.zero;
        }

        public bool InventoryButtonD()
        {
            return false;
        }

        public float LeftRotationCameraDH()
        {
            return 0f;
        }

        public Vector3 LocomotionAxis()
        {
            return Vector3.zero;
        }

        public bool PuzzleResetButton()
        {
            return false;
        }

        public float RightRotationCameraDH()
        {
            return 0f;
        }

        public bool SwitchSelectionButtonD()
        {
            return this.switchSelectionButtonD;
        }

        public bool TimeForwardButtonDH()
        {
            return false;
        }
        #endregion
    }
}
