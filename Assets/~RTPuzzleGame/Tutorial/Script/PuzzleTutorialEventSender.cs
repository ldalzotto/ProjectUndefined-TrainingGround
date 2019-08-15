﻿using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleTutorialEventSender : MonoBehaviour
    {
        private TutorialManager tutorialManager;

        public void Init()
        {
            this.tutorialManager = GameObject.FindObjectOfType<TutorialManager>();
        }

        public void Tick(float d)
        {
            if (!this.tutorialManager.GetTutorialCurrentState(TutorialStepID.PUZZLE_TIME_ELAPSING) && !this.tutorialManager.IsTutorialStepPlaying())
            {
                this.tutorialManager.PlayTutorialStep(TutorialStepID.PUZZLE_TIME_ELAPSING);
            }
        }
    }
}
