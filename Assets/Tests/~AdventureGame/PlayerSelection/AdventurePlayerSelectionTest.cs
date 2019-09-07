using AdventureGame;
using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class AdventurePlayerSelectionTest : MonoBehaviour
    {
        private Action objectDynamicInstancesCreation;
        private Action ghostPOIStateSetters;

        private IEnumerator Before(Action objectDynamicInstancesCreation = null, Action ghostPOIStateSetters = null)
        {
            var dontDestroyOnLoadObject = new GameObject("go");
            DontDestroyOnLoad(dontDestroyOnLoadObject);
            foreach (var root in dontDestroyOnLoadObject.scene.GetRootGameObjects())
            {
                Destroy(root);
            }
            this.objectDynamicInstancesCreation = objectDynamicInstancesCreation;
            this.ghostPOIStateSetters = ghostPOIStateSetters;
            SceneManager.sceneLoaded += this.OnSceneLoadCallBack;
            SceneManager.LoadScene(SceneConstants.ADVENTURE_TEST, LoadSceneMode.Single);
            yield return new WaitForFixedUpdate();
            SceneManager.sceneLoaded -= this.OnSceneLoadCallBack;
            yield return null;
            if (this.ghostPOIStateSetters != null) { this.ghostPOIStateSetters.Invoke(); }
        }

        private void OnSceneLoadCallBack(Scene arg0, LoadSceneMode arg1)
        {
            if (this.objectDynamicInstancesCreation != null) { this.objectDynamicInstancesCreation.Invoke(); }
        }

        #region Pure Selection Test
        [UnityTest]
        public IEnumerator PlayerSelection_NoSelectionWhenNoObject()
        {
            yield return this.Before();

            var playerPointOfInterestSelectionManager = GameObject.FindObjectOfType<PlayerPointOfInterestSelectionManager>();
            Assert.IsTrue(playerPointOfInterestSelectionManager.GetCurrentSelectedPOI() == null);
        }

        [UnityTest]
        public IEnumerator PlayerSelection_NothingHappensWhenThereIsOnlyOneObject()
        {
            PointOfInterestType pointOfInterest1 = null;
            var test1DummyContextAction = new DummyContextAction(null);
            yield return this.Before(() =>
            {
                pointOfInterest1 = ContextActionOnlyDefinition.SingleDummyPOI(PointOfInterestTestID.TEST_1).Instanciate(Vector3.zero);
            }, () =>
            {
                var GhostsPOIManager = GameObject.FindObjectOfType<GhostsPOIManager>();
                var test1Ghost = GhostsPOIManager.GetGhostPOI(PointOfInterestId.TEST_1);
                test1Ghost.OnContextActionAdd(test1DummyContextAction);
            });

            var playerPointOfInterestSelectionManager = GameObject.FindObjectOfType<PlayerPointOfInterestSelectionManager>();
            Assert.IsTrue(playerPointOfInterestSelectionManager.GetCurrentSelectedPOI() == pointOfInterest1);

            var mockedInput = GameObject.FindObjectOfType<AdventureTstMockedInputManager>();
            mockedInput.GetAdventureTestMockedXInput().switchSelectionButtonD = true;

            yield return null;
            Assert.IsTrue(playerPointOfInterestSelectionManager.GetCurrentSelectedPOI() == pointOfInterest1);

            mockedInput.GetAdventureTestMockedXInput().switchSelectionButtonD = false;
        }

        [UnityTest]
        public IEnumerator PlayerSelection_AvailableContextActionsAreModifiedWhenSwitchSelectionIsPressed()
        {
            PointOfInterestType pointOfInterest1 = null;
            PointOfInterestType pointOfInterest2 = null;
            var test1DummyContextAction = new DummyContextAction(null);
            var test2DummyContextAction_1 = new DummyContextAction(null);
            var test2DummyContextAction_2 = new DummyContextAction(null);
            yield return this.Before(() =>
            {
                pointOfInterest1 = ContextActionOnlyDefinition.SingleDummyPOI(PointOfInterestTestID.TEST_1).Instanciate(Vector3.zero);
                pointOfInterest2 = ContextActionOnlyDefinition.SingleDummyPOI(PointOfInterestTestID.TEST_2).Instanciate(Vector3.zero);
            }, () =>
            {
                var GhostsPOIManager = GameObject.FindObjectOfType<GhostsPOIManager>();
                var test1Ghost = GhostsPOIManager.GetGhostPOI(PointOfInterestId.TEST_1);
                var test2Ghost = GhostsPOIManager.GetGhostPOI(PointOfInterestId.TEST_2);
                test1Ghost.OnContextActionAdd(test1DummyContextAction);
                test2Ghost.OnContextActionAdd(test2DummyContextAction_1);
                test2Ghost.OnContextActionAdd(test2DummyContextAction_2);
            });

            var playerPointOfInterestSelectionManager = GameObject.FindObjectOfType<PlayerPointOfInterestSelectionManager>();
            Assert.IsTrue(playerPointOfInterestSelectionManager.GetCurrentSelectedPOI() == pointOfInterest2);

            var mockedInput = GameObject.FindObjectOfType<AdventureTstMockedInputManager>();
            mockedInput.GetAdventureTestMockedXInput().switchSelectionButtonD = true;

            yield return null;
            Assert.IsTrue(playerPointOfInterestSelectionManager.GetCurrentSelectedPOI() == pointOfInterest1);
            yield return null;
            Assert.IsTrue(playerPointOfInterestSelectionManager.GetCurrentSelectedPOI() == pointOfInterest2);

            mockedInput.GetAdventureTestMockedXInput().switchSelectionButtonD = false;
        }
        #endregion

        #region Selection with wheel Test
        [UnityTest]
        public IEnumerator PlayerSelection_WheelOpenWithSelectedObject()
        {
            PointOfInterestType pointOfInterest1 = null;
            var test1DummyContextAction = new DummyContextAction(null);
            yield return this.Before(() =>
            {
                pointOfInterest1 = ContextActionOnlyDefinition.SingleDummyPOI(PointOfInterestTestID.TEST_1).Instanciate(Vector3.zero);
            }, () =>
            {
                var GhostsPOIManager = GameObject.FindObjectOfType<GhostsPOIManager>();
                var test1Ghost = GhostsPOIManager.GetGhostPOI(PointOfInterestId.TEST_1);
                test1Ghost.OnContextActionAdd(test1DummyContextAction);
            });


            var mockedInput = GameObject.FindObjectOfType<AdventureTstMockedInputManager>();
            mockedInput.GetAdventureTestMockedXInput().actionButtonD = true;

            yield return null;

            var selectionWheelNodes = GameObject.FindObjectsOfType<SelectionWheelNode>();
            Assert.IsTrue(selectionWheelNodes != null);
            Assert.IsTrue(selectionWheelNodes.Length == 1);

            var wheelNodeContextAction = ((ContextActionSelectionWheelNodeData)selectionWheelNodes[0].WheelNodeData).Data as AContextAction;
            Assert.IsTrue(wheelNodeContextAction == test1DummyContextAction);
            mockedInput.GetAdventureTestMockedXInput().actionButtonD = false;

        }

        [UnityTest]
        public IEnumerator PlayerSelection_WheelNodesChangeWhenSwitchSelectionIsPressed()
        {
            PointOfInterestType pointOfInterest1 = null;
            PointOfInterestType pointOfInterest2 = null;
            var test1DummyContextAction = new DummyContextAction(null);
            var test2DummyContextAction_1 = new DummyContextAction(null);
            var test2DummyContextAction_2 = new DummyContextAction(null);
            yield return this.Before(() =>
            {
                pointOfInterest1 = ContextActionOnlyDefinition.SingleDummyPOI(PointOfInterestTestID.TEST_1).Instanciate(Vector3.zero);
                pointOfInterest2 = ContextActionOnlyDefinition.SingleDummyPOI(PointOfInterestTestID.TEST_2).Instanciate(Vector3.zero);
            }, () =>
            {
                var GhostsPOIManager = GameObject.FindObjectOfType<GhostsPOIManager>();
                var test1Ghost = GhostsPOIManager.GetGhostPOI(PointOfInterestId.TEST_1);
                var test2Ghost = GhostsPOIManager.GetGhostPOI(PointOfInterestId.TEST_2);
                test1Ghost.OnContextActionAdd(test1DummyContextAction);
                test2Ghost.OnContextActionAdd(test2DummyContextAction_1);
                test2Ghost.OnContextActionAdd(test2DummyContextAction_2);
            });

            var playerPointOfInterestSelectionManager = GameObject.FindObjectOfType<PlayerPointOfInterestSelectionManager>();
            Assert.IsTrue(playerPointOfInterestSelectionManager.GetCurrentSelectedPOI() == pointOfInterest2);

            var mockedInput = GameObject.FindObjectOfType<AdventureTstMockedInputManager>();
            mockedInput.GetAdventureTestMockedXInput().actionButtonD = true;
            yield return null;

            mockedInput.GetAdventureTestMockedXInput().actionButtonD = false;
            mockedInput.GetAdventureTestMockedXInput().switchSelectionButtonD = true;

            yield return null;
            yield return new WaitForEndOfFrame(); //Wait for destroy to be taken into account
            var selectionWheelNodes = GameObject.FindObjectsOfType<SelectionWheelNode>();
            Assert.IsTrue(selectionWheelNodes != null);
            Debug.Log(selectionWheelNodes.Length);
            Assert.IsTrue(selectionWheelNodes.Length == 1);

            var wheelNodeContextAction1 = ((ContextActionSelectionWheelNodeData)selectionWheelNodes[0].WheelNodeData).Data as AContextAction;
            Assert.IsTrue(wheelNodeContextAction1 == test1DummyContextAction);

            mockedInput.GetAdventureTestMockedXInput().actionButtonD = false;
            mockedInput.GetAdventureTestMockedXInput().switchSelectionButtonD = false;
        }
        #endregion
    }
}
