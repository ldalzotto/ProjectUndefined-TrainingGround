using UnityEngine;
using System.Collections;
using CoreGame;
using static Tests.AbstractPuzzleSceneTest;
using RTPuzzle;

namespace Tests
{
    public class TestPuzzleGameManager : RTPuzzle.GameManager
    {

        protected override void AfterGameManagerPersistanceInstanceInitialization()
        {
            //No persistance
            var persistanceManager = GameObject.FindObjectOfType<PersistanceManager>();
            var persistanceManagerGO = persistanceManager.gameObject;
            MonoBehaviour.DestroyImmediate(persistanceManager);
            persistanceManagerGO.AddComponent<MockPersistanceManager>();

            //No dotted line thread
            var DottedLineRendererManager = GameObject.FindObjectOfType<DottedLineRendererManager>();
            var DottedLineRendererManagerGO = DottedLineRendererManager.gameObject;
            MonoBehaviour.DestroyImmediate(DottedLineRendererManager);
            DottedLineRendererManagerGO.AddComponent<MockDottedLineRendererManager>();
        }
    }

}
