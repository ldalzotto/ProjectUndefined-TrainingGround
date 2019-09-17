using CoreGame;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class LocalPuzzleCutsceneModule : InteractiveObjectModule, ILocalPuzzleCutsceneModuleEvent
    {
        #region External Dependencies
        private InteractiveObjectContainer InteractiveObjectContainer;
        #endregion

        private Dictionary<int, SequencedActionPlayer> ConcurrentPlayingCutscenes;

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;

            this.ConcurrentPlayingCutscenes = new Dictionary<int, SequencedActionPlayer>();
        }

        #region ILocalPuzzleCutsceneModuleEvent
        public void PlayLocalCutscene(int cutscenePlayerId, PuzzleCutsceneGraph PuzzleCutsceneGraph, Dictionary<CutsceneParametersName, object> PuzzleCutsceneActionInputParameters)
        {
            this.ConcurrentPlayingCutscenes[cutscenePlayerId] = new SequencedActionPlayer(PuzzleCutsceneGraph.GetRootActions(), new PuzzleCutsceneActionInput(this.InteractiveObjectContainer, PuzzleCutsceneActionInputParameters));
            this.ConcurrentPlayingCutscenes[cutscenePlayerId].Play();
        }

        public void StopLocalCutscene(int cutscenePlayerId)
        {
            this.ConcurrentPlayingCutscenes.TryGetValue(cutscenePlayerId, out SequencedActionPlayer playedCutscene);
            if (playedCutscene != null)
            {
                playedCutscene.Kill();
                this.ConcurrentPlayingCutscenes.Remove(cutscenePlayerId);
            }
        }
        #endregion

        public void TickAlways(float d)
        {
            List<int> endedCutscenes = null;

            foreach (var PlayingCutscene in this.ConcurrentPlayingCutscenes)
            {
                PlayingCutscene.Value.Tick(d);
                if (!PlayingCutscene.Value.IsPlaying())
                {
                    if (endedCutscenes == null) { endedCutscenes = new List<int>(); }
                    endedCutscenes.Add(PlayingCutscene.Key);
                }
            }

            if (endedCutscenes != null)
            {
                foreach (var endedCutsceneId in endedCutscenes)
                {
                    this.ConcurrentPlayingCutscenes.Remove(endedCutsceneId);
                }
            }
        }

    }

}
