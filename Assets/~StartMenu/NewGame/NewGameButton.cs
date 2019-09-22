using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace StartMenu
{
    public class NewGameButton : MonoBehaviour
    {
        public bool SwitchLevel;

        private void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(() =>
            {
       
            });
        }

        private void Update()
        {
            if (this.SwitchLevel)
            {
                this.SwitchLevel = false;
                var startLevelManager = CoreGameSingletonInstances.StartLevelManager;
                CoreGameSingletonInstances.LevelTransitionManager.OnStartMenuToLevel(startLevelManager.GetStartLevelID());
            }
        }
    }
}
