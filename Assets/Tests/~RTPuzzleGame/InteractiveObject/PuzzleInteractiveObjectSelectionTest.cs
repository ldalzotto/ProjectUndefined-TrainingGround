using GameConfigurationID;
using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class PuzzleInteractiveObjectSelectionTest : AbstractPuzzleSceneTest
    {
        [UnityTest]
        public IEnumerator PlayerSelection_NoSelectionWhenNoObject()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest);
            var InteractiveObjectSelectionManager = GameObject.FindObjectOfType<InteractiveObjectSelectionManager>();
            Assert.IsTrue(InteractiveObjectSelectionManager.GetCurrentSelectedObject() == null);
        }

        [UnityTest]
        public IEnumerator PlayerSelection_NothingHappensWhenThereIsOnlyOneObject()
        {
            InteractiveObjectType interactableObject1 = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, () =>
            {
                interactableObject1 = ActionInteractableObjectDefinition.ActionInteractableObjectOnly(InteractiveObjectTestID.TEST_1, new ActionInteractableObjectInitialization(
                           9999f, PlayerActionId.TEST_1, 0f, 1, PuzzleCutsceneID.TEST_1, ActionInteractableObjectDefinition.DoNothingCutsceneGraph()
                    )).Instanciate(Vector3.zero);
            });
            yield return new WaitForFixedUpdate();

            var mockedInput = GameObject.FindObjectOfType<GameTestMockedInputManager>();
            mockedInput.GetGameTestMockedXInput().switchSelectionButtonD = true;

            yield return null;
            yield return new WaitForEndOfFrame();

            var actionIntereactableObjectModule1 = interactableObject1.GetModule<ActionInteractableObjectModule>();


            var InteractiveObjectSelectionManager = GameObject.FindObjectOfType<InteractiveObjectSelectionManager>();

            Assert.IsTrue(InteractiveObjectSelectionManager.GetCurrentSelectedObject() != null);
            Assert.IsTrue(InteractiveObjectSelectionManager.GetCurrentSelectedObject().AssociatedPlayerAction == actionIntereactableObjectModule1.AssociatedPlayerAction);

            yield return null;
            Assert.IsTrue(InteractiveObjectSelectionManager.GetCurrentSelectedObject() != null);
            Assert.IsTrue(InteractiveObjectSelectionManager.GetCurrentSelectedObject().AssociatedPlayerAction == actionIntereactableObjectModule1.AssociatedPlayerAction);
        }

        [UnityTest]
        public IEnumerator PlayerSelection_AvailableContextActionsAreModifiedWhenSwitchSelectionIsPressed()
        {
            InteractiveObjectType interactableObject1 = null;
            InteractiveObjectType interactableObject2 = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, () =>
            {
                interactableObject1 = ActionInteractableObjectDefinition.ActionInteractableObjectOnly(InteractiveObjectTestID.TEST_1, new ActionInteractableObjectInitialization(
                           9999f, PlayerActionId.TEST_1, 0f, 1, PuzzleCutsceneID.TEST_1, ActionInteractableObjectDefinition.DoNothingCutsceneGraph()
                    )).Instanciate(Vector3.zero);
                interactableObject2 = ActionInteractableObjectDefinition.ActionInteractableObjectOnly(InteractiveObjectTestID.TEST_2, new ActionInteractableObjectInitialization(
                         9999f, PlayerActionId.TEST_2, 0f, 1, PuzzleCutsceneID.TEST_2, ActionInteractableObjectDefinition.DoNothingCutsceneGraph()
                  )).Instanciate(Vector3.zero);
            });
            yield return new WaitForFixedUpdate();

            var mockedInput = GameObject.FindObjectOfType<GameTestMockedInputManager>();
            mockedInput.GetGameTestMockedXInput().switchSelectionButtonD = true;

            yield return null;
            yield return new WaitForEndOfFrame();

            var actionIntereactableObjectModule1 = interactableObject1.GetModule<ActionInteractableObjectModule>();
            var actionIntereactableObjectModule2 = interactableObject2.GetModule<ActionInteractableObjectModule>();


            var InteractiveObjectSelectionManager = GameObject.FindObjectOfType<InteractiveObjectSelectionManager>();

            Assert.IsTrue(InteractiveObjectSelectionManager.GetCurrentSelectedObject() != null);
            Assert.IsTrue(InteractiveObjectSelectionManager.GetCurrentSelectedObject().AssociatedPlayerAction == actionIntereactableObjectModule1.AssociatedPlayerAction);

            yield return null;
            Assert.IsTrue(InteractiveObjectSelectionManager.GetCurrentSelectedObject().AssociatedPlayerAction == actionIntereactableObjectModule2.AssociatedPlayerAction);

            yield return null;
            Assert.IsTrue(InteractiveObjectSelectionManager.GetCurrentSelectedObject().AssociatedPlayerAction == actionIntereactableObjectModule1.AssociatedPlayerAction);
        }

        [UnityTest]
        public IEnumerator PlayerSelection_WheelOpenWithSelectedObject()
        {
            InteractiveObjectType interactableObject1 = null;
            InteractiveObjectType interactableObject2 = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, () =>
            {
                interactableObject1 = ActionInteractableObjectDefinition.ActionInteractableObjectOnly(InteractiveObjectTestID.TEST_1, new ActionInteractableObjectInitialization(
                           9999f, PlayerActionId.TEST_1, 0f, 1, PuzzleCutsceneID.TEST_1, ActionInteractableObjectDefinition.DoNothingCutsceneGraph()
                    )).Instanciate(Vector3.zero);
                interactableObject2 = ActionInteractableObjectDefinition.ActionInteractableObjectOnly(InteractiveObjectTestID.TEST_2, new ActionInteractableObjectInitialization(
                        9999f, PlayerActionId.TEST_2, 0f, 1, PuzzleCutsceneID.TEST_2, ActionInteractableObjectDefinition.DoNothingCutsceneGraph()
                 )).Instanciate(Vector3.zero);
            });
            yield return new WaitForFixedUpdate();

            var mockedInput = GameObject.FindObjectOfType<GameTestMockedInputManager>();
            mockedInput.GetGameTestMockedXInput().actionButtonD = true;

            yield return null;
            yield return new WaitForEndOfFrame();

            var actionIntereactableObjectModule1 = interactableObject1.GetModule<ActionInteractableObjectModule>();

            var selectionWheelNodes = GameObject.FindObjectsOfType<SelectionWheelNode>();
            Assert.IsTrue(selectionWheelNodes != null);
            Assert.IsTrue(selectionWheelNodes.Length == 1);


            var wheelNodeContextAction = ((PlayerSelectionWheelNodeData)selectionWheelNodes[0].WheelNodeData).Data as RTPPlayerAction;
            Assert.IsTrue(wheelNodeContextAction == actionIntereactableObjectModule1.AssociatedPlayerAction);
            mockedInput.GetGameTestMockedXInput().actionButtonD = false;
        }


        [UnityTest]
        public IEnumerator PlayerSelection_WheelNodesChangeWhenSwitchSelectionIsPressed()
        {
            InteractiveObjectType interactableObject1 = null;
            InteractiveObjectType interactableObject2 = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, () =>
            {
                interactableObject1 = ActionInteractableObjectDefinition.ActionInteractableObjectOnly(InteractiveObjectTestID.TEST_1, new ActionInteractableObjectInitialization(
                           9999f, PlayerActionId.TEST_1, 0f, 1, PuzzleCutsceneID.TEST_1, ActionInteractableObjectDefinition.DoNothingCutsceneGraph()
                    )).Instanciate(Vector3.zero);
                interactableObject2 = ActionInteractableObjectDefinition.ActionInteractableObjectOnly(InteractiveObjectTestID.TEST_2, new ActionInteractableObjectInitialization(
                         9999f, PlayerActionId.TEST_2, 0f, 1, PuzzleCutsceneID.TEST_2, ActionInteractableObjectDefinition.DoNothingCutsceneGraph()
                  )).Instanciate(Vector3.zero);
            });
            yield return new WaitForFixedUpdate();

            var mockedInput = GameObject.FindObjectOfType<GameTestMockedInputManager>();
            mockedInput.GetGameTestMockedXInput().actionButtonD = true;
            yield return new WaitForEndOfFrame();
            mockedInput.GetGameTestMockedXInput().actionButtonD = false;
            mockedInput.GetGameTestMockedXInput().switchSelectionButtonD = true;
            yield return null;
            yield return new WaitForEndOfFrame(); //Wait for destroy to be taken into account
            
            var actionIntereactableObjectModule2 = interactableObject2.GetModule<ActionInteractableObjectModule>();

            var selectionWheelNodes = GameObject.FindObjectsOfType<SelectionWheelNode>();

            Assert.IsTrue(selectionWheelNodes != null);
            Assert.IsTrue(selectionWheelNodes.Length == 1);

            var wheelNodeContextAction = ((PlayerSelectionWheelNodeData)selectionWheelNodes[0].WheelNodeData).Data as RTPPlayerAction;
            Assert.IsTrue(wheelNodeContextAction == actionIntereactableObjectModule2.AssociatedPlayerAction);
            mockedInput.GetGameTestMockedXInput().actionButtonD = false;
        }
    }

}
